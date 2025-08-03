using BepInEx.Logging;
namespace FrootLuips.ChaosMod.Logging;

public interface ILogger
{
	void LogDebug(string message);
	void LogInfo(string message);
	void LogWarn(string message);
	void LogError(string message);
	void LogFatal(string message);
	void LogInGame(string message, LogLevel level = LogLevel.Info, float duration = 5f);
}
