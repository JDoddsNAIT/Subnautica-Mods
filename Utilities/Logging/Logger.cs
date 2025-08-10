using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using BepInEx.Logging;

namespace FrootLuips.Subnautica.Logging;
/// <summary>
/// Provides a default implementation of the <see cref="ILogger"/> interface.
/// </summary>
public class Logger : ILogger
{
	private const float _STARTUP_DELAY = 3.0f;
	private const string _MAIN_MENU = "XMenu";

	private ManualLogSource _logSource;
	private readonly Queue<Message> _messageQueue = new();
	private bool _initialized = false;

	public ManualLogSource LogSource {
		get => _logSource;
		set => _logSource = value ?? throw new ArgumentNullException(nameof(value));
	}

	public Logger(ManualLogSource logger)
	{
		_logSource = logger;
		_messageQueue = new();
		_initialized = false;

		SceneManager.sceneLoaded += this.SceneManager_sceneLoaded;
	}

	private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.name != _MAIN_MENU)
			return;

		SceneManager.sceneLoaded -= this.SceneManager_sceneLoaded;
		UWE.CoroutineHost.StartCoroutine(ShowMessageQueue());
	}

	private System.Collections.IEnumerator ShowMessageQueue()
	{
		// Wait for the menu to become visible to the player
		yield return new UnityEngine.WaitForSeconds(_STARTUP_DELAY);

		while (_messageQueue.TryDequeue(out var message))
		{
			AddInGameMessage(message.Text, message.Level);
			yield return UWE.CoroutineUtils.waitForNextFrame;
		}

		_initialized = true;
	}

	public void LogDebug(string message, bool showInGame = false)
	{
		this.LogSource.LogDebug(message);
		if (showInGame)
			AddInGameMessage(message, LogLevel.Debug);
	}

	public void LogInfo(string message, bool showInGame = false)
	{
		this.LogSource.LogInfo(message);
		if (showInGame)
			AddInGameMessage(message, LogLevel.Info);
	}

	public void LogMessage(string message, bool showInGame = false)
	{
		this.LogSource.LogMessage(message);
		if (showInGame)
			AddInGameMessage(message, LogLevel.Message);
	}

	public void LogWarning(string message, bool showInGame = false)
	{
		this.LogSource.LogWarning(message);
		if (showInGame)
			AddInGameMessage(message, LogLevel.Warning);
	}

	public void LogError(string message, bool showInGame = false)
	{
		this.LogSource.LogError(message);
		if (showInGame)
			AddInGameMessage(message, LogLevel.Error);
	}

	public void LogFatal(string message, bool showInGame = false)
	{
		this.LogSource.LogFatal(message);
		if (showInGame)
			AddInGameMessage(message, LogLevel.Fatal);
	}

	private void AddInGameMessage(string message, LogLevel level)
	{
		if (!_initialized)
		{
			_messageQueue.Enqueue(new Message(message, level));
		}
		else
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
	}

	public readonly record struct Message(string Text, LogLevel Level);
}
