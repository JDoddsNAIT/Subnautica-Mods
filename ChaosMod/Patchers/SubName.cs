using HarmonyLib;

namespace FrootLuips.ChaosMod.Patchers;

[HarmonyPatch(typeof(global::SubName))]
internal class SubName
{
	[HarmonyPatch(nameof(global::SubName.Awake)), HarmonyPostfix]
	public static void Awake_Postfix(global::SubName __instance)
	{
		EntityDB.Register(__instance);
	}
}
