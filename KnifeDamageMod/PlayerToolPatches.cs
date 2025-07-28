using HarmonyLib;

namespace FrootLuips.KnifeDamageMod
{
	[HarmonyPatch(typeof(PlayerTool))]
	internal class PlayerToolPatches
	{
		[HarmonyPatch(nameof(PlayerTool.Awake))]
		[HarmonyPostfix]
		public static void Awake_Postfix(PlayerTool __instance)
		{
			if (KnifeDamagePlugin.ModOptions.ModEnabled && __instance is Knife knife)
			{
				KnifeDamagePlugin.Log.LogInfo($"Knife damage is {knife.damage}");
				knife.damage *= KnifeDamagePlugin.ModOptions.DamageMultiplier;
				KnifeDamagePlugin.Log.LogInfo($"Knife damage is now {knife.damage}");
			}
		}
	}
}
