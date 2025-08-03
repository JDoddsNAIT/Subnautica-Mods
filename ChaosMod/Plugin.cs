using System.Reflection;
using BepInEx;
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

	//public const string StartupMessage = "Welcome to " + NAME + "! open the console an"

	private static ILogger? _logger;
	private static ModOptions? _options;

	public static new ILogger Logger { get => _logger!; private set => _logger = value; }

	internal static ModOptions Options { get => _options!; private set => _options = value; }
	internal static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

	private void Awake()
	{
		Logger = new Logger(base.Logger);
		Options = OptionsPanelHandler.RegisterModOptions<ModOptions>();

		Harmony.CreateAndPatchAll(Assembly, GUID);

		Logger.LogInfo(new LogMessage(context: "Init", notice: "Finished loading plugin", message: GUID));
	}

	// TODO: Load all chaos effects when the game starts
	// TODO: Reset all chaos effect when the game ends
}
