using System.Reflection;
using BepInEx;
using FrootLuips.ChaosMod.Logging;
using HarmonyLib;
using Nautilus.Handlers;

namespace FrootLuips.ChaosMod;
[BepInPlugin(GUID, NAME, VERSION)]
[BepInDependency("com.snmodding.nautilus")]
public class Plugin : BaseUnityPlugin
{
	public const string GUID = "com.github.frootluips.ChaosMod";
	public const string NAME = "FrootLuips' Chaos Mod";
	public const string VERSION = "1.0.0";

	public new static ILogger? Logger { get; private set; }
	internal static ModOptions? Options { get; private set; }

	internal static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

	private void Awake()
	{
		Logger = new Logger(base.Logger);
		Options = OptionsPanelHandler.RegisterModOptions<ModOptions>();

		Harmony.CreateAndPatchAll(Assembly, GUID);

		Logger.LogInfo(new LogMessage(context: "Init", notice: "Finished loading plugin", message: GUID));
	}
}
