using FrootLuips.ChaosMod.Utilities;

namespace FrootLuips.ChaosMod.Effects;
internal class DamageVehicles : BaseChaosEffect
{
	public DamageVehicles() : base(ChaosEffect.DamageVehicles, attributesExpected: 1) { }

	public float? DamageDealt { get; set; } = null;

	public override void OnStart()
	{
		var vehicles = UnityEngine.Object.FindObjectsOfType<global::Vehicle>();
		for (int i = 0; i < vehicles.Length; i++)
		{
			var live = vehicles[i].liveMixin;
			float damage = live.maxHealth * (float)(DamageDealt! / 100f);
			live.TakeDamage(damage, vehicles[i].transform.position, type: DamageType.Pressure, null);
			vehicles[i].crushDamage.soundOnDamage.Play();
		}
	}

	public override void FromData(Effect data, StatusCallback callback)
	{
		DamageDealt = null;
		base.FromData(data, callback);
	}

	protected override void ParseAttribute(Effect.Attribute attribute)
	{
		switch (attribute.Name)
		{
			case nameof(DamageDealt):
				attribute.ParseAttribute(float.Parse, out float damage);
				if (damage is not >= 0 and <= 100)
					throw new Exception($"Attribute '{attribute.Name}' must have a float value between 0 and 100.");
				DamageDealt = damage;
				break;
			default:
				throw attribute.Invalid();
		}
	}

	protected override bool GetSuccess() => DamageDealt != null;

	public override Effect ToData() => new() {
		Id = this.Id.ToString(),
		Duration = this.Duration,
		Weight = this.Weight,
		Attributes = new[] {
			new Effect.Attribute(nameof(DamageDealt), DamageDealt.ToString())
		}
	};
}
