using BepInEx.Logging;

namespace FrootLuips.Subnautica.Logging;
/// <summary>
/// Provides an implementation of the <see cref="ILogger"/> interface that logs messages to the <see cref="BepInEx"/> console.
/// </summary>
public class ConsoleLogger : ILogger
{
	/// <summary>
	/// The <see cref="ManualLogSource"/> used to log messages.
	/// </summary>
	public ManualLogSource Source { get; }

	/// <summary/>
	/// <param name="source"></param>
	public ConsoleLogger(ManualLogSource source)
	{
		this.Source = source;
	}

	/// <inheritdoc/>
	public void LogDebug(string message) => Source.LogDebug(message);
	/// <inheritdoc/>
	public void LogInfo(string message) => Source.LogInfo(message);
	/// <inheritdoc/>
	public void LogMessage(string message) => Source.LogMessage(message);
	/// <inheritdoc/>
	public void LogWarning(string message) => Source.LogWarning(message);
	/// <inheritdoc/>
	public void LogError(string message) => Source.LogError(message);
	/// <inheritdoc/>
	public void LogFatal(string message) => Source.LogFatal(message);
}
