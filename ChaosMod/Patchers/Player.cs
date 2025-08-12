#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable IDE0060 // Remove unused parameter

using HarmonyLib;
using UnityEngine;

namespace FrootLuips.ChaosMod.Patchers;

[HarmonyPatch(typeof(global::Player))]
internal class Player
{
	[HarmonyPatch(nameof(global::Player.Awake)), HarmonyPrefix]
	public static void Awake_Prefix(global::Player __instance)
	{
		new GameObject().AddComponent<ConsoleCommandListener>();
	}
}
