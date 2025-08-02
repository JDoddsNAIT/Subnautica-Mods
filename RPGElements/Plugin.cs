using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;

namespace FrootLuips.RPGElements;
#nullable disable
[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus")]
public class Plugin : BaseUnityPlugin
{
	public const string DISPLAY_NAME = "FrootLuips' RPG Elements";

	public static ModOptions Options { get; private set; }
	public new static ManualLogSource Logger { get; private set; }

	private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

	private void Awake()
	{
		Options = OptionsPanelHandler.RegisterModOptions<ModOptions>();
		Logger = base.Logger;

		// register harmony patches, if there are any
		Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_GUID}");
		Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
	}
}