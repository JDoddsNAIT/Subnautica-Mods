using System.Reflection;
using BepInEx;
using FrootLuips.ChaosMod.Effects;
using FrootLuips.ChaosMod.Logging;
using HarmonyLib;
using Nautilus.Handlers;

namespace FrootLuips.ChaosMod;
[BepInPlugin(GUID, NAME, VERSION)]
[BepInDependency("com.snmodding.nautilus")]
public sealed class Plugin : BaseUnityPlugin
{
	public const string GUID = "com.github.frootluips.ChaosMod";
	public const string NAME = "FrootLuips' Chaos Mod";
	public const string VERSION = "1.0.0";

	private static ILogger? _logger;
	private static ModOptions? _options;

	public static new ILogger Logger { get => _logger!; private set => _logger = value; }

	internal static ModOptions Options { get => _options!; private set => _options = value; }
	internal static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

	internal void Awake()
	{
		Logger = new Logger(base.Logger);
		Options = OptionsPanelHandler.RegisterModOptions<ModOptions>();

		if (!System.IO.File.Exists(ChaosMod.effectsFilePath))
		{
			Logger.LogDebug(new LogMessage(
				notice: $"{ChaosMod.EFFECTS_CONFIG} is missing.",
				message: "Restoring..."));
			ChaosEffects.ResetEffects();

			try
			{
				ChaosEffects.Save(ChaosMod.effectsFilePath);
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
