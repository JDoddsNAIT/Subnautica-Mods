using BepInEx.Logging;
namespace FrootLuips.ChaosMod.Logging;

public interface ILogger
{
	void LogDebug(string message);
	void LogInfo(string message);
	void LogMessage(string message);
	void LogWarning(string message);
	void LogError(string message);
	void LogFatal(string message);
}

public static class LoggerExtensions
{
	public static void Log(this ILogger logger, LogLevel level, string message)
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
			default:
				logger.LogDebug(message);
				break;
		}
	}
}
