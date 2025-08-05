using System.Reflection;
using BepInEx;
using FrootLuips.ChaosMod.Effects;
using FrootLuips.ChaosMod.Logging;
using HarmonyLib;
using Nautilus.Handlers;

namespace FrootLuips.ChaosMod;
[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus")]
public sealed class Plugin : BaseUnityPlugin
{
	public const string GUID = PluginInfo.PLUGIN_GUID;
	public const string NAME = PluginInfo.PLUGIN_NAME;
	public const string VERSION = PluginInfo.PLUGIN_VERSION;

	private static ILogger? _logger;
	private static ModOptions? _options;

	public static new ILogger Logger { get => _logger!; private set => _logger = value; }

	internal static ModOptions Options { get => _options!; private set => _options = value; }
	internal static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

	internal void Awake()
	{
		Logger = new Logger(base.Logger);
		Options = OptionsPanelHandler.RegisterModOptions<ModOptions>();

		if (!System.IO.File.Exists(EffectManager.effectsFilePath))
		{
			Logger.LogDebug(new LogMessage(
				notice: $"{EffectManager.EFFECTS_CONFIG} is missing.",
				message: "Restoring..."));
			ChaosEffects.ResetEffects();

			try
			{
				ChaosEffects.Save(EffectManager.effectsFilePath);
			}
			catch (System.Exception ex)
			{
				Logger.LogError(LogMessage.FromException(ex).WithNotice("Failed to generate effect data."));
				return;
			}
		}

		ConsoleCommandsHandler.RegisterConsoleCommands(typeof(ConsoleCommands));
		Harmony.CreateAndPatchAll(Assembly, GUID);

		Logger.LogInfo(new LogMessage(context: "Init", notice: "Finished loading plugin", message: GUID));
	}
}
