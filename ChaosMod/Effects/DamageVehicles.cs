using FrootLuips.ChaosMod.Utilities;

namespace FrootLuips.ChaosMod.Effects;
internal class DamageVehicles : BaseChaosEffect
{
	public DamageVehicles() : base(ChaosEffect.DamageVehicles) { }

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
		base.FromData(data, callback);

		List<string> errors = new();
		try
		{
			Validate(ValidateAttributes(data.Attributes));
		}
		catch (AggregateException agg)
		{
			foreach (var error in agg)
			{
				errors.Add(error.Message);
			}
		}
		catch (Exception ex)
		{
			errors.Add(ex.Message);
		}
		finally
		{
			bool success = DamageDealt != null;
			callback(errors, success);
		}
	}

	private IEnumerator<Exception> ValidateAttributes(Effect.Attribute[] attributes)
	{
		ExpectAttributeCount(attributes, count: 1);

		var attribute = attributes[0];
		Exception? exception = null;
		try
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
		catch (Exception ex)
		{
			exception = ex;
		}

		if (exception != null)
			yield return exception;
	}

	public override Effect ToData() => new() {
		Id = this.Id.ToString(),
		Duration = this.Duration,
		Weight = this.Weight,
		Attributes = new[] {
			new Effect.Attribute(nameof(DamageDealt), DamageDealt.ToString())
		}
	};
}
