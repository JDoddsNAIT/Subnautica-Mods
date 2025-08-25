using System.Collections;
using System.Collections.Generic;
using BepInEx.Logging;
using FrootLuips.Subnautica.Extensions;
using FrootLuips.Subnautica.Helpers;
using UnityEngine.SceneManagement;

namespace FrootLuips.Subnautica.Logging;
/// <summary>
/// Provides an implementation of the <see cref="ILogger"/> interface that logs messages to the in-game message system.
/// </summary>
public class GameLogger : ILogger
{
	private const string _MAINMENU_SCENE = "XMenu";
	private const float _STARTUP_DELAY = 3.0f;

	/// <summary>
	/// Is <see langword="true"/> if the main menu has been loaded and there are no queued messages.
	/// </summary>
	public bool Initialized { get; private set; }
	private readonly Queue<Message> _messageQueue;

	/// <summary>
	/// Constructs a new <see cref="GameLogger"/>.
	/// </summary>
	public GameLogger()
	{
		Initialized = UnityEngine.Object.FindObjectOfType<ErrorMessage>() != null;
		_messageQueue = new();

		if (!Initialized)
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
		// Wait for the menu to become visible
		yield return new UnityEngine.WaitForSeconds(_STARTUP_DELAY);

		while (_messageQueue.TryDequeue(out Message msg))
		{
			AddMessage(msg.Level, msg.Text);
			yield return UWE.CoroutineUtils.waitForNextFrame;
		}
		Initialized = true;
	}

	/// <inheritdoc/>
	public void LogDebug(string message) => QueueMessage(LogLevel.Debug, message);
	/// <inheritdoc/>
	public void LogInfo(string message) => QueueMessage(LogLevel.Info, message);
	/// <inheritdoc/>
	public void LogMessage(string message) => QueueMessage(LogLevel.Message, message);
	/// <inheritdoc/>
	public void LogWarning(string message) => QueueMessage(LogLevel.Warning, message);
	/// <inheritdoc/>
	public void LogError(string message) => QueueMessage(LogLevel.Error, message);
	/// <inheritdoc/>
	public void LogFatal(string message) => QueueMessage(LogLevel.Fatal, message);

	private void QueueMessage(LogLevel level, string message)
	{
		if (Initialized)
			AddMessage(level, message);
		else
			_messageQueue.Enqueue(new Message(message, level));
	}

	private void AddMessage(LogLevel level, string message)
	{
		// colour the message
		message = level.GetHighestLevel() switch {
			LogLevel.Fatal or LogLevel.Error => string.Format(RichTextFormatter.Main, "{0:c=red}", message),
			LogLevel.Warning => string.Format(RichTextFormatter.Main, "{0:c=yellow}", message),
			LogLevel.Debug => string.Format(RichTextFormatter.Main, "{0:c=silver}", message),
			_ => message,
		};
		ErrorMessage.AddError(message);
	}

	internal readonly record struct Message(string Text, LogLevel Level);
}
