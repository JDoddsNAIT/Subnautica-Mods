using BepInEx.Logging;

namespace FrootLuips.Subnautica.Logging;
/// <summary>
/// Defines how messages are logged.
/// </summary>
public interface ILogger
{
	/// <summary>
	/// Logs a <paramref name="message"/> at the Debug level.
	/// </summary>
	/// <param name="message"></param>
	void LogDebug(string message);
	/// <summary>
	/// Logs a <paramref name="message"/> at the Info level.
	/// </summary>
	/// <param name="message"></param>
	void LogInfo(string message);
	/// <summary>
	/// Logs a <paramref name="message"/> at the Message level.
	/// </summary>
	/// <param name="message"></param>
	void LogMessage(string message);
	/// <summary>
	/// Logs a <paramref name="message"/> at the Warning level.
	/// </summary>
	/// <param name="message"></param>
	void LogWarning(string message);
	/// <summary>
	/// Logs a <paramref name="message"/> at the Error level.
	/// </summary>
	/// <param name="message"></param>
	void LogError(string message);
	/// <summary>
	/// Logs a <paramref name="message"/> at the Fatal level.
	/// </summary>
	/// <param name="message"></param>
	void LogFatal(string message);
}

/// <summary>
/// Extension methods for classes that implement the <see cref="ILogger"/> interface.
/// </summary>
public static class LoggerExtensions
{
	/// <summary>
	/// Logs a <paramref name="message"/> at the specified <paramref name="level"/>.
	/// </summary>
	/// <param name="logger"></param>
	/// <param name="message"></param>
	/// <param name="level"></param>
	/// <exception cref="System.ArgumentOutOfRangeException"></exception>
	public static void Log(this ILogger logger, string message, LogLevel level)
	{
		switch (level.GetHighestLevel())
		{
			case LogLevel.Fatal:
				logger.LogFatal(message);
				break;
			case LogLevel.Error:
				logger.LogError(message);
				break;
			case LogLevel.Warning:
				logger.LogWarning(message);
				break;
			case LogLevel.Message:
				logger.LogMessage(message);
				break;
			case LogLevel.Info:
				logger.LogInfo(message);
				break;
			case LogLevel.Debug:
				logger.LogDebug(message);
				break;
			default:
				throw new System.ArgumentOutOfRangeException(nameof(level));
		}
	}
}
