using BepInEx.Logging;

namespace FrootLuips.Subnautica.Logging;
public interface ILogger
{
	void LogDebug(string message, bool showInGame = false);
	void LogInfo(string message, bool showInGame = false);
	void LogMessage(string message, bool showInGame = false);
	void LogWarning(string message, bool showInGame = false);
	void LogError(string message, bool showInGame = false);
	void LogFatal(string message, bool showInGame = false);
}

public static class LoggerExtensions
{
	public static void Log(this ILogger logger, string message, LogLevel level, bool showInGame = false)
	{
		switch (level.GetHighestLevel())
		{
			case LogLevel.Fatal:
				logger.LogFatal(message, showInGame);
				break;
			case LogLevel.Error:
				logger.LogError(message, showInGame);
				break;
			case LogLevel.Warning:
				logger.LogWarning(message, showInGame);
				break;
			case LogLevel.Message:
				logger.LogMessage(message, showInGame);
				break;
			case LogLevel.Info:
				logger.LogInfo(message, showInGame);
				break;
			case LogLevel.Debug:
				logger.LogDebug(message, showInGame);
				break;
			default:
				throw new System.ArgumentOutOfRangeException(nameof(level));
		}
	}
}
