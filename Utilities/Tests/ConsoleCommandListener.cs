using System.IO;
using System.Text;
using BepInEx;
using UnityEngine;

namespace FrootLuips.Subnautica.Tests;
internal class ConsoleCommandListener : MonoBehaviour
{
	private const string _FILENAME = "test_results.txt";

	internal static ITestContainer[] Tests { get; } = new ITestContainer[] {
		new LogMessage_Tests(),
		new Queries_Tests(),
		new StringExtensions_Tests(),
		new Trees_Tests(),
	};

	private Logging.Logger? _logger;

	internal void Awake()
	{
		_logger = Plugin.Logger;
		DevConsole.RegisterConsoleCommand("runtests", OnConsoleCommand_runtests);
	}

	public void OnConsoleCommand_runtests(NotificationCenter.Notification n)
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

		try
		{
			var path = Path.Combine(Paths.PluginPath, PluginInfo.PLUGIN_GUID, _FILENAME);
			File.WriteAllText(path, sb.ToString());
			_logger!.LogMessage($"Test results output to {_FILENAME}.", inGame: true);
		}
		catch (System.Exception ex)
		{
			_logger!.LogError($"{ex}", inGame: true);
		}
	}
}
