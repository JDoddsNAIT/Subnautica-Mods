using FrootLuips.ChaosMod.Utilities;
using UnityEngine;

namespace FrootLuips.ChaosMod.Effects;
internal class SpawnGhost : BaseChaosEffect
{
	public SpawnGhost() : base(ChaosEffect.SpawnGhost, attributesExpected: 1) { }

	public float? SpawnDistance { get; set; } = null;

	public override void OnStart()
	{
		UWE.CoroutineHost.StartCoroutine(SpawnGhostLeviathan());
	}

	private System.Collections.IEnumerator SpawnGhostLeviathan()
	{
		var techType = TechType.GhostLeviathanJuvenile;
		CoroutineTask<GameObject> request = CraftData.GetPrefabForTechTypeAsync(techType);
		yield return request;
		GameObject result = request.GetResult();

		Assertions.Assert(result != null, $"No prefab found for techType '{techType}'");

		GameObject clone = global::Utils.CreatePrefab(result, (float)SpawnDistance!, randomizeDirection: true);
		global::LargeWorldEntity.Register(clone);
		global::CrafterLogic.NotifyCraftEnd(clone, techType);
		clone.SendMessage("StartConstruction", SendMessageOptions.DontRequireReceiver);
	}

	public override void FromData(Effect data, StatusCallback callback)
	{
		SpawnDistance = null;
		base.FromData(data, callback);
	}

	protected override void ParseAttribute(Effect.Attribute attribute)
	{
		switch (attribute.Name)
		{
			case nameof(SpawnDistance):
				attribute.ParseAttribute(float.Parse, out float distance);
				if (distance <= 0)
					throw new Exception($"Attribute '{attribute.Name}' must have a float value greater than 0.");
				SpawnDistance = distance;
				break;
			default:
				throw attribute.Invalid();
		}
	}

	protected override bool GetSuccess() => SpawnDistance != null;

	public override Effect ToData() => new() {
		Id = this.Id.ToString(),
		Duration = this.Duration,
		Weight = this.Weight,
		Attributes = new[] {
			new Effect.Attribute(nameof(SpawnDistance), SpawnDistance.ToString()),
		}
	};
}
