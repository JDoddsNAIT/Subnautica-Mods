#pragma warning disable IDE1006 // Naming Styles
using HarmonyLib;

namespace FrootLuips.ChaosMod.Patchers;

[HarmonyPatch(typeof(global::Creature))]
internal class Creature
{
	[HarmonyPatch(nameof(global::Creature.OnEnable)), HarmonyPostfix]
	public static void OnEnable_PostFix(global::Creature __instance)
	{
		EntityDB<global::Creature>.Register(__instance);
	}

	[HarmonyPatch(nameof(global::Creature.OnDisable)), HarmonyPostfix]
	public static void OnDisable_PostFix(global::Creature __instance)
	{
		EntityDB<global::Creature>.Deregister(__instance);
	}
}
