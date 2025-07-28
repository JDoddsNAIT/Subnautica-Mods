using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;

namespace FrootLuips.KnifeDamageMod
{
	[BepInPlugin(MY_GUID, PLUGIN_NAME, VERSION)]
	[BepInDependency("com.snmodding.nautilus")] // marks Nautilus as a dependency for this mod
	public class KnifeDamagePlugin : BaseUnityPlugin
	{
		public const string MY_GUID = "com.frootluips.knifedamagemod";
		public const string PLUGIN_NAME = "Knife Damage Mod";
		public const string VERSION = "1.0.0";

		private static readonly Harmony _harmony = new Harmony(MY_GUID);

		public static ModOptions ModOptions;

		public static ManualLogSource Log;

		private void Awake()
		{
			ModOptions = OptionsPanelHandler.RegisterModOptions<ModOptions>();

			_harmony.PatchAll();
			Logger.LogInfo(PLUGIN_NAME + " " + VERSION + " " + "loaded. We're in.");
			Log = Logger;
		}
	}
}
