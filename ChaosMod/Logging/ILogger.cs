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
