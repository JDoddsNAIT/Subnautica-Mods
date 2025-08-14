
namespace FrootLuips.ChaosMod.Effects;
internal class FakeCrash : BaseChaosEffect
{
	public FakeCrash() : base(ChaosEffect.FakeCrash, attributesExpected: 2) { }

	public float? MinDuration { get; set; } = null;
	public float? MaxDuration { get; set; } = null;

	public override string BeforeStart()
	{
		float duration = UnityEngine.Random.Range((float)MinDuration!, (float)MaxDuration!);
		Plugin.Logger.LogDebug($"Initiating fake crash, duration {duration} seconds.");
		var timer = System.Diagnostics.Stopwatch.StartNew();
		while (timer.Elapsed.TotalSeconds < duration)
		{
			continue; // Freeze the game
		}
		timer.Stop();
		return base.BeforeStart();
	}

	public override void OnStart()
	{
		InstantiateTechType(TechType.Crash, Player.main.transform.position);
	}

	public override void FromData(Effect data, StatusCallback callback)
	{
		MinDuration = null;
		MaxDuration = null;
		base.FromData(data, callback);
	}

	protected override void ParseAttribute(Effect.Attribute attribute)
	{
		switch (attribute.Name)
		{
			case nameof(MinDuration):
				attribute.ParseValue(float.Parse, out var min);
				MinDuration = min;
				break;
			case nameof(MaxDuration):
				attribute.ParseValue(float.Parse, out var max);
				MaxDuration = max;
				break;
			default:
				throw attribute.Invalid();
		}
	}

	protected override bool GetSuccess()
	{
		return MinDuration != null && MaxDuration != null;
	}

	public override Effect ToData()
	{
		return new Effect() {
			Id = this.Id.ToString(),
			Duration = this.Duration,
			Weight = this.Weight,
			Attributes = new[] {
				new Effect.Attribute(nameof(MinDuration), MinDuration.ToString()),
				new Effect.Attribute(nameof(MaxDuration), MaxDuration.ToString()),
			},
		};
	}
}
