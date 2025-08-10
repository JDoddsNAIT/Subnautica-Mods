using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Logging;
using FrootLuips.Subnautica.Logging;
using FrootLuips.Subnautica.Tests;
using Nautilus;

namespace FrootLuips.Subnautica;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus")]
public sealed class Plugin : BaseUnityPlugin
{
	private static ILogger? _logger;

	public static new ILogger Logger { get => _logger!; private set => _logger = value; }

	internal static ITestContainer[] Tests { get; } = new[] {
		new Queries_Tests(),
	};

	private void Awake()
	{
		Logger = new Logging.Logger(base.Logger);

		foreach (var container in Tests)
		{
			RunTests(container);
		}
	}

	internal static void RunTests(ITestContainer container)
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
}
