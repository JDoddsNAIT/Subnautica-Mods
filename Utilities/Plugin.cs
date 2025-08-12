using BepInEx;
using FrootLuips.Subnautica.Logging;
using FrootLuips.Subnautica.Tests;
using Nautilus.Handlers;

namespace FrootLuips.Subnautica;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus")]
public sealed class Plugin : BaseUnityPlugin
{
	private static ILogger? _logger;
	public static new ILogger Logger { get => _logger!; private set => _logger = value; }

	internal static ITestContainer[] Tests { get; } = new ITestContainer[] {
		new LogMessage_Tests(),
		new Queries_Tests(),
		new StringExtensions_Tests(),
	};

	private void Awake()
	{
		Logger = new Logging.Logger(base.Logger);

		ConsoleCommandsHandler.RegisterConsoleCommand<System.Func<string>>("testutils", RunTests);
	}

	internal static string RunTests()
	{
		foreach (var container in Tests)
		{
			var enumerator = container.GetResults();

			while (enumerator.MoveNext())
			{
				var result = enumerator.Current;
				if (result)
					Logger.LogMessage(result.ToString());
				else
					Logger.LogError(result.ToString());
			}
		}
		return "Test results output to log.";
	}
}
