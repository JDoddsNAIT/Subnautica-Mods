using HarmonyLib;
using UnityEngine;

namespace FrootLuips.ChaosMod.Patchers;

[HarmonyPatch(declaringType: typeof(global::Player), methodName: "Awake")]
internal class Player_Awake
{
	[HarmonyPrefix]
	public static void Prefix(Player __instance)
	{
		new GameObject().AddComponent<ConsoleCommandListener>();
	}
}
