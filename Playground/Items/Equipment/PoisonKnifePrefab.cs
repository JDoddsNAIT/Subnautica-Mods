using System.Collections;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Extensions;
using Nautilus.Handlers;
using UnityEngine;
using Ingredient = CraftData.Ingredient;

namespace FrootLuips.Playground.Items.Equipment;

public static class PoisonKnifePrefab
{
	public static PrefabInfo Info { get; } = PrefabInfo
		.WithTechType("PoisonKnife", "Poison Knife", "Poisons creatures afflicting damage over time.")
		.WithIcon(SpriteManager.Get(TechType.HeatBlade));

	public static RecipeData Recipe { get; } = new RecipeData() {
		craftAmount = 0,
		Ingredients = {
			TechType.Knife.AsIngredient(),
			TechType.GasPod.AsIngredient(amount: 2),
			TechType.AcidMushroom.AsIngredient(amount: 1)
		}
	};

	public static void Register()
	{
		var customPrefab = new CustomPrefab(Info);

		var poisonKnifeObj = new CloneTemplate(Info, TechType.HeatBlade);
		poisonKnifeObj.ModifyPrefab += obj => {
			HeatBlade heatBlade = obj.GetComponent<HeatBlade>();
			PoisonKnife poisonKnife = obj.AddComponent<PoisonKnife>().CopyComponent(heatBlade);
			Object.DestroyImmediate(heatBlade);
			poisonKnife.damage = Plugin.ModOptions.PoisonKnifeBaseDamage;
		};

		customPrefab.SetGameObject(poisonKnifeObj);
		customPrefab.SetRecipe(Recipe)
			.WithFabricatorType(CraftTree.Type.Workbench)
			.WithCraftingTime(2);
		customPrefab.SetEquipment(EquipmentType.Hand);
		customPrefab.SetPdaGroupCategory(TechGroup.Workbench, TechCategory.Workbench);
		
		customPrefab.Register();
	}

	public static Ingredient AsIngredient(this TechType techType, int amount = 1) => new(techType, amount);
}

public class PoisonKnife : HeatBlade
{
	public override string animToolName => TechType.HeatBlade.AsString(true);

	public override void OnToolUseAnim(GUIHand hand)
	{
		base.OnToolUseAnim(hand);

		GameObject target = null;
		Vector3 hitPosition = default;
		UWE.Utils.TraceFPSTargetPosition(Player.main.gameObject, attackDist, ref target, ref hitPosition);

		if (target == null)
			return; // no valid target

		LiveMixin liveMixin = target.GetComponentInParent<LiveMixin>();
		if (liveMixin != null && IsValidTarget(liveMixin))
		{
			UWE.CoroutineHost.StartCoroutine(DamageOverTime(liveMixin, Plugin.ModOptions.PoisonEffectDamage, Plugin.ModOptions.PoisonEffectDuration));
		}
	}

	private IEnumerator DamageOverTime(LiveMixin target, float damage, float duration)
	{
		float timer = 0;

		Plugin.LogInfo($"{this}: {target} is taking damage over time.");

		while (timer < duration && target != null)
		{
			yield return null;
			timer += Time.deltaTime;

			float time = Time.deltaTime;
			if (timer > duration)
				time -= duration - timer;

			target.TakeDamage(damage * time, type: DamageType.Poison);
		}

		Plugin.LogInfo($"{this}: {target} is no longer taking damage over time.");
	}
}
