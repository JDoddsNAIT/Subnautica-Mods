using System.Collections;
using FrootLuips.ChaosMod.Utilities;
using UnityEngine;

namespace FrootLuips.ChaosMod.Effects;
internal class Mushrooms : BaseChaosEffect
{
	public Mushrooms() : base(ChaosEffect.Mushrooms, attributesExpected: 0) { }

	private const TechType _MUSHROOMS = TechType.AcidMushroom;

	public override void OnStart()
	{
		EnsurePlayerExists();
		UWE.CoroutineHost.StartCoroutine(GiveItems());
	}

	private IEnumerator GiveItems()
	{
		TaskResult<GameObject> task = new();

		global::Pickupable? item;

		while (Inventory.main.HasRoomFor(_MUSHROOMS))
		{
			yield return global::CraftData.InstantiateFromPrefabAsync(_MUSHROOMS, task, false);
			GameObject clone = task.Get();
			Assertions.Assert(clone != null, $"No prefab found for techType '{_MUSHROOMS}'");
			global::CrafterLogic.NotifyCraftEnd(clone, _MUSHROOMS);
			item = clone!.GetComponent<Pickupable>();
			Inventory.main.Pickup(item);
		}
	}

	protected override void ParseAttribute(Effect.Attribute attribute) { }
	protected override bool GetSuccess() => true;
}
