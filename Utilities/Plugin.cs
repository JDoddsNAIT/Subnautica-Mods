using System.IO;
using System.Text;
using BepInEx;
using FrootLuips.Subnautica.Logging;
using FrootLuips.Subnautica.Tests;

namespace FrootLuips.Subnautica;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
internal sealed class Plugin : BaseUnityPlugin
{
	private const string _FILENAME = "test_results.txt";

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

		DevConsole.RegisterConsoleCommand(this, "utilsruntests");
	}

	public static string OnConsoleCommand_utilsruntests()
	{
		var sb = new StringBuilder();
		foreach (var container in Tests)
		{
			var enumerator = container.GetResults();

			while (enumerator.MoveNext())
			{
				var result = enumerator.Current;
				sb.AppendLine(result.ToString());
			}
		}

		var path = Path.Combine(Paths.PluginPath, PluginInfo.PLUGIN_GUID, _FILENAME);
		File.WriteAllText(path, sb.ToString());

		return $"Test results output to {_FILENAME}.";
	}
}
