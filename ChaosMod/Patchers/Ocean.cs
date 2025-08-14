#pragma warning disable IDE1006 // Naming Styles
using HarmonyLib;

namespace FrootLuips.ChaosMod.Patchers;

[HarmonyPatch(typeof(global::Ocean))]
internal class Ocean
{
	[HarmonyPatch(nameof(global::Ocean.Awake)), HarmonyPostfix]
	public static void Awake_PostFix(global::Ocean __instance)
	{
		Effects.MoistPercent.Ocean = __instance;
	}
}
