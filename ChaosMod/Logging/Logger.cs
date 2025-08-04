using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Logging;

namespace FrootLuips.ChaosMod.Logging;

public class Logger : ILogger
{
	private const float _STARTUP_DELAY = 3.0f;
	private const string _MAINMENU_SCENE_NAME = "XMenu";

	public ManualLogSource BepInLogger { get; }

	public bool MainMenuLoaded { get; private set; }
	private readonly Queue<(string, float)> _inGameMessageQueue;

	private ErrorMessage? _errorMessageInstance;
	private readonly FieldInfo _timeDelay;

	public Logger(ManualLogSource bepInLogger)
	{
		this.BepInLogger = bepInLogger;
		_inGameMessageQueue = new();
		MainMenuLoaded = false;

		_timeDelay = HarmonyLib.AccessTools.Field(typeof(ErrorMessage), nameof(ErrorMessage.timeFadeOut));

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

		_errorMessageInstance ??= UnityEngine.Object.FindObjectOfType<ErrorMessage>();
		UnityEngine.SceneManagement.SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
		_errorMessageInstance.StartCoroutine(ShowMessageQueue());
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
		float initialDuration = (float)_timeDelay.GetValue(obj: _errorMessageInstance);
		_timeDelay.SetValue(obj: _errorMessageInstance, value: duration);
		ErrorMessage.AddMessage(message);
		_timeDelay.SetValue(obj: _errorMessageInstance, value: initialDuration);
	}

	private IEnumerator ShowMessageQueue()
	{
		yield return new UnityEngine.WaitForSeconds(_STARTUP_DELAY);

		while (_inGameMessageQueue.Count > 0)
		{
			(string message, float duration) = _inGameMessageQueue.Dequeue();
			AddInGameMessage(message, duration);
		}
		MainMenuLoaded = true;
	}
}
