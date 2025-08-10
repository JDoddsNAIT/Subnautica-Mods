using HarmonyLib;

namespace FrootLuips.ChaosMod.Patchers;

[HarmonyPatch(typeof(global::Ocean), methodName: "Awake")]
internal class Ocean_Awake
{
	[HarmonyPostfix]
	public static void PostFix(Ocean __instance)
	{
		Effects.MoistPercent.Ocean = __instance;
	}
}
