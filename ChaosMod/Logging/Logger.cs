using System.Collections;
using System.Collections.Generic;
using BepInEx.Logging;

namespace FrootLuips.ChaosMod.Logging;

public class Logger : ILogger
{
	private const float _STARTUP_DELAY = 3.0f;
	private const string _MAINMENU_SCENE_NAME = "XMenu";

	public ManualLogSource BepInLogger { get; }
	public ErrorMessage? ErrorMessage { get; private set; }

	public bool MainMenuLoaded { get; private set; }
	private readonly Queue<(string, float)> _inGameMessageQueue;

	public Logger(ManualLogSource bepInLogger)
	{
		this.BepInLogger = bepInLogger;
		_inGameMessageQueue = new();
		MainMenuLoaded = false;

		UnityEngine.SceneManagement.SceneManager.sceneLoaded += this.SceneManager_sceneLoaded;
	}

	public void LogDebug(string message) => BepInLogger.LogDebug(message);

	public void LogFatal(string message) => BepInLogger.LogFatal(message);

	public void LogInfo(string message) => BepInLogger.LogInfo(message);

	public void LogWarn(string message) => BepInLogger.LogWarning(message);

	public void LogError(string message) => BepInLogger.LogError(message);

	private void SceneManager_sceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
	{
		if (scene.name != _MAINMENU_SCENE_NAME)
			return;
		UWE.CoroutineHost.StartCoroutine(FindErrorMessageInstance());
	}

	public void LogInGame(string message, LogLevel level = LogLevel.Info, float duration = 5f)
	{
		BepInLogger.Log(level, message);

		message = level.GetHighestLevel() switch {
			LogLevel.Warning => $"<color=yellow>{message}</color>",
			LogLevel.Error or LogLevel.Fatal => $"<color=red>{message}</color>",
			_ => message,
		};

		if (!MainMenuLoaded)
		{
			_inGameMessageQueue.Enqueue((message, duration));
		}
		else
		{
			AddInGameMessage(message, duration);
		}
	}

	private void AddInGameMessage(string message, float duration)
	{
		float initialDuration = ErrorMessage!.timeDelay;
		ErrorMessage!.timeDelay = duration;
		ErrorMessage.AddMessage(message);
		ErrorMessage!.timeDelay = initialDuration;
	}

	private IEnumerator ShowMessageQueue()
	{
		yield return new UnityEngine.WaitForSeconds(_STARTUP_DELAY);
		MainMenuLoaded = true;

		while (_inGameMessageQueue.Count > 0)
		{
			(string message, float duration) = _inGameMessageQueue.Dequeue();
			AddInGameMessage(message, duration);
		}
	}

	private IEnumerator FindErrorMessageInstance()
	{
		while (ErrorMessage == null)
		{
			LogWarn("Searching for instance of ErrorMessage...");
			ErrorMessage = UnityEngine.Object.FindObjectOfType<ErrorMessage>();
			if (ErrorMessage != null)
			{
				LogInfo("ErrorMessage Instance found.");
				UnityEngine.SceneManagement.SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
				ErrorMessage!.StartCoroutine(ShowMessageQueue());
				yield break;
			}
			else
			{
				LogWarn("No instance found. Will try again on the next frame.");
				yield return null;
			}
		}
	}
}
