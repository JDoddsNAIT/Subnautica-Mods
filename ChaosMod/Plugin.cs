using System.Reflection;
using BepInEx;
using FrootLuips.ChaosMod.Effects;
using FrootLuips.ChaosMod.Logging;
using HarmonyLib;
using Nautilus.Handlers;

namespace FrootLuips.ChaosMod;
[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID)]
public sealed class Plugin : BaseUnityPlugin
{
	private static ILogger? _logger;
	private static ModOptions? _options;

	public static new ILogger Logger { get => _logger!; private set => _logger = value; }

	internal static ModOptions Options { get => _options!; private set => _options = value; }
	internal static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

	internal void Awake()
	{
		Logger = new Logger(base.Logger);
		Options = OptionsPanelHandler.RegisterModOptions<ModOptions>();

		if (!File.Exists(RandomTeleport.teleportsPath))
		{
			Logger.LogDebug(new LogMessage(
				notice: $"{RandomTeleport.TELEPORTS} is missing.",
				message: "Restoring..."));
			ConsoleCommands.GetTeleports();
		}

		if (!File.Exists(EffectManager.effectsFilePath))
		{
			Logger.LogDebug(new LogMessage(
				notice: $"{EffectManager.EFFECTS_CONFIG} is missing.",
				message: "Restoring..."));
			ChaosEffects.ResetEffects();

			try
			{
				ChaosEffects.Save(EffectManager.effectsFilePath);
			}
			catch (Exception ex)
			{
				Logger.LogError(LogMessage.FromException(ex).WithNotice("Failed to generate effect data."));
				return;
			}
		}

		ConsoleCommandsHandler.RegisterConsoleCommands(typeof(ConsoleCommands));
		Harmony.CreateAndPatchAll(Assembly, PluginInfo.PLUGIN_GUID);

		Logger.LogInfo(new LogMessage(context: "Init", notice: "Finished loading plugin", message: PluginInfo.PLUGIN_GUID));
	}
}
