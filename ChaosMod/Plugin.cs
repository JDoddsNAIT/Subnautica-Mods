using System.Collections;
using System.Reflection;
using BepInEx;
using FrootLuips.ChaosMod.Effects;
using FrootLuips.ChaosMod.Logging;
using FrootLuips.ChaosMod.Utilities;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Json.ExtensionMethods;

namespace FrootLuips.ChaosMod;
[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID)]
public sealed class Plugin : BaseUnityPlugin
{
	private static ILogger? _console, _game;
	private static ModOptions? _options;

	public static ILogger Game { get => _game!; private set => _game = value; }
	public static ILogger Console { get => _console!; private set => _console = value; }

	internal static ModOptions Options { get => _options!; private set => _options = value; }
	internal static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

	internal void Awake()
	{
		Game = new InGameLogger(base.Logger);
		Console = new ConsoleLogger(base.Logger);
		Options = OptionsPanelHandler.RegisterModOptions<ModOptions>();

		if (Options.DebugResetEffects || !File.Exists(RandomTeleport.teleportsPath))
		{
			Console.LogDebug(new LogMessage(
				notice: $"{RandomTeleport.TELEPORTS} is missing.",
				message: "Restoring..."));

			this.StartCoroutine(CreateTeleports());
		}

		if (Options.DebugResetEffects || !File.Exists(EffectManager.effectsFilePath))
		{
			Console.LogDebug(new LogMessage(
				notice: $"{EffectManager.EFFECTS_CONFIG} is missing.",
				message: "Restoring..."));
			ChaosEffects.ResetEffects();

			try
			{
				ChaosEffects.Save(EffectManager.effectsFilePath);
				Console.LogDebug("Saved effect data to " + EffectManager.effectsFilePath);
			}
			catch (Exception ex)
			{
				Console.LogError(LogMessage.FromException(ex).WithNotice("Failed to generate effect data."));
				return;
			}
		}

		Harmony.CreateAndPatchAll(Assembly, PluginInfo.PLUGIN_GUID);

		Console.LogMessage(new LogMessage(context: "Init", notice: "Finished loading plugin", message: PluginInfo.PLUGIN_GUID));
	}

	private IEnumerator CreateTeleports()
	{
		while (GotoConsoleCommand.main == null)
		{
			yield return UWE.CoroutineUtils.waitForNextFrame;
		}

		var locations = GotoConsoleCommand.main.data.locations;
		List<Objects.TeleportPosition> positions = new(capacity: locations.Length);
		SimpleQueries.Convert(locations, converter: Utilities.Utils.ToPosition, positions);
		positions.SaveJson(RandomTeleport.teleportsPath);
		Console.LogDebug("Saved teleport data to " + RandomTeleport.teleportsPath);
	}
}
