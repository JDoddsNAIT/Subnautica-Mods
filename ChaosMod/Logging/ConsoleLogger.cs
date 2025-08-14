using BepInEx.Logging;

namespace FrootLuips.ChaosMod.Logging;
public class ConsoleLogger : ILogger
{
	public ManualLogSource LogSource { get; }

	public ConsoleLogger(ManualLogSource logSource)
	{
		LogSource = logSource;
	}

	public void LogDebug(string message) => LogSource.LogDebug(message);
	public void LogInfo(string message) => LogSource.LogInfo(message);
	public void LogMessage(string message) => LogSource.LogMessage(message);
	public void LogWarning(string message) => LogSource.LogWarning(message);
	public void LogError(string message) => LogSource.LogError(message);
	public void LogFatal(string message) => LogSource.LogFatal(message);
}
