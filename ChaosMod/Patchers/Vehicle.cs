#pragma warning disable IDE1006 // Naming Styles
using HarmonyLib;

namespace FrootLuips.ChaosMod.Patchers;

[HarmonyPatch(typeof(global::Vehicle))]
internal class Vehicle
{
	[HarmonyPatch(nameof(global::Vehicle.Awake)), HarmonyPostfix]
	public static void Awake_Postfix(global::Vehicle __instance)
	{
		EntityDB.Register(__instance);
	}
}
