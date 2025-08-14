using BepInEx;
using FrootLuips.Subnautica.Logging;
using FrootLuips.Subnautica.Tests;

namespace FrootLuips.Subnautica;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
internal sealed class Plugin : BaseUnityPlugin
{
	private static Logger? _logger;
	public static new Logger Logger { get => _logger!; private set => _logger = value; }

	internal static ITestContainer[] Tests { get; } = new ITestContainer[] {
		new LogMessage_Tests(),
		new Queries_Tests(),
		new StringExtensions_Tests(),
	};

	internal void Awake()
	{
		Logger = new Logger(base.Logger);

		DevConsole.RegisterConsoleCommand(this, "runutilstests");
	}

	internal static string OnConsoleCommand_runutilstests()
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
