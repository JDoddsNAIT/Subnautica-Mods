using BepInEx.Logging;

namespace FrootLuips.Subnautica.Logging;
/// <summary>
/// Provides an implementation of the <see cref="ILogger"/> interface that logs messages to both the <see cref="BepInEx"/> console and the in-game message system.
/// </summary>
public class Logger : ILogger
{
	private readonly ConsoleLogger _console;
	private readonly GameLogger _game;

	/// <inheritdoc cref="GameLogger.Initialized"/>
	public bool Initialized => _game.Initialized;

	/// <summary>
	/// <inheritdoc cref="ConsoleLogger"/>
	/// </summary>
	public ConsoleLogger Console => _console;
	/// <summary>
	/// <inheritdoc cref="GameLogger"/>
	/// </summary>
	public GameLogger Game => _game;

	/// <summary/>
	/// <param name="source"></param>
	public Logger(ManualLogSource source)
	{
		_console = new(source);
		_game = new();
	}

	/// <summary>
	/// <inheritdoc cref="ILogger.LogDebug(string)"/>
	/// </summary>
	/// <param name="message"></param>
	/// <param name="inGame">Should the message also be shown in-game?</param>
	public void LogDebug(string message, bool inGame = false)
	{
		_console.LogDebug(message);
		if (inGame)
			_game.LogDebug(message);
	}

	/// <summary>
	/// <inheritdoc cref="ILogger.LogInfo(string)"/>
	/// </summary>
	/// <param name="message"></param>
	/// <param name="inGame">Should the message also be shown in-game?</param>
	public void LogInfo(string message, bool inGame = false)
	{
		_console.LogInfo(message);
		if (inGame)
			_game.LogInfo(message);
	}

	/// <summary>
	/// <inheritdoc cref="ILogger.LogMessage(string)"/>
	/// </summary>
	/// <param name="message"></param>
	/// <param name="inGame">Should the message also be shown in-game?</param>
	public void LogMessage(string message, bool inGame = false)
	{
		_console.LogMessage(message);
		if (inGame)
			_game.LogMessage(message);
	}

	/// <summary>
	/// <inheritdoc cref="ILogger.LogWarning(string)"/>
	/// </summary>
	/// <param name="message"></param>
	/// <param name="inGame">Should the message also be shown in-game?</param>
	public void LogWarning(string message, bool inGame = false)
	{
		_console.LogWarning(message);
		if (inGame)
			_game.LogWarning(message);
	}

	/// <summary>
	/// <inheritdoc cref="ILogger.LogError(string)"/>
	/// </summary>
	/// <param name="message"></param>
	/// <param name="inGame">Should the message also be shown in-game?</param>
	public void LogError(string message, bool inGame = false)
	{
		_console.LogError(message);
		if (inGame)
			_game.LogError(message);
	}

	/// <summary>
	/// <inheritdoc cref="ILogger.LogFatal(string)"/>
	/// </summary>
	/// <param name="message"></param>
	/// <param name="inGame">Should the message also be shown in-game?</param>
	public void LogFatal(string message, bool inGame = false)
	{
		_console.LogFatal(message);
		if (inGame)
			_game.LogFatal(message);
	}

	#region ILogger
	void ILogger.LogDebug(string message)
	{
		_console.LogDebug(message);
		_game.LogDebug(message);
	}

	void ILogger.LogError(string message)
	{
		_console.LogError(message);
		_game.LogError(message);
	}

	void ILogger.LogFatal(string message)
	{
		_console.LogFatal(message);
		_game.LogFatal(message);
	}

	void ILogger.LogInfo(string message)
	{
		_console.LogInfo(message);
		_game.LogInfo(message);
	}

	void ILogger.LogMessage(string message)
	{
		_console.LogMessage(message);
		_game.LogMessage(message);
	}

	void ILogger.LogWarning(string message)
	{
		_console.LogWarning(message);
		_game.LogWarning(message);
	}
	#endregion
}
