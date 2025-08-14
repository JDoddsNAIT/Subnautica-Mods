using System.Collections;
using BepInEx.Logging;
using UnityEngine.SceneManagement;

namespace FrootLuips.ChaosMod.Logging;
public class InGameLogger : ILogger
{
	private const float _STARTUP_DELAY = 3.0f;
	private const string _MAINMENU_SCENE = "XMenu";

	public ManualLogSource LogSource { get; }
	public bool Initialized { get; private set; }
	private readonly Queue<Message> _messageQueue;

	public InGameLogger(ManualLogSource logSource)
	{
		LogSource = logSource;
		_messageQueue = new();
		this.Initialized = false;

		SceneManager.sceneLoaded += this.SceneManager_sceneLoaded;
	}

	private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.name != _MAINMENU_SCENE)
			return;

		SceneManager.sceneLoaded -= this.SceneManager_sceneLoaded;
		UWE.CoroutineHost.StartCoroutine(ShowMessageQueue());
	}

	private IEnumerator ShowMessageQueue()
	{
		// Wait for the main menu to become visible
		yield return new UnityEngine.WaitForSeconds(_STARTUP_DELAY);

		while (_messageQueue.Count > 0)
		{
			_messageQueue.Dequeue().Deconstruct(out var text, out var level);
			AddMessage(text, level);
			yield return UWE.CoroutineUtils.waitForNextFrame;
		}
		Initialized = true;
	}

	public void LogDebug(string message) => QueueMessage(message, LogLevel.Debug);
	public void LogInfo(string message) => QueueMessage(message, LogLevel.Info);
	public void LogMessage(string message) => QueueMessage(message, LogLevel.Message);
	public void LogWarning(string message) => QueueMessage(message, LogLevel.Warning);
	public void LogError(string message) => QueueMessage(message, LogLevel.Error);
	public void LogFatal(string message) => QueueMessage(message, LogLevel.Fatal);

	private void QueueMessage(string message, LogLevel level)
	{
		LogSource.Log(level, message);
		if (!Initialized)
		{
			_messageQueue.Enqueue(new Message(message, level));
		}
		else
		{
			AddMessage(message, level);
		}
	}

	private void AddMessage(string message, LogLevel level)
	{
		// colour the message
		level = level.GetHighestLevel();
		message = level switch {
			LogLevel.Fatal or LogLevel.Error => $"<color=red>{message}</color>",
			LogLevel.Warning => $"<color=yellow>{message}</color>",
			_ => message,
		};
		ErrorMessage.AddError(message);
	}

	public readonly record struct Message(string Text, LogLevel Level);
}
