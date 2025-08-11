namespace FrootLuips.ChaosMod.Effects;
internal class ScalePlayer : BaseChaosEffect
{
	public ScalePlayer() : base(ChaosEffect.ScalePlayer, attributesExpected: 1, duration: 60f) { }

	public ScaleData[]? Scales { get; set; } = null;
	
	private RandomDistribution<ScaleData>? RandomDistribution { get; set; }

	private float _activeScale = 1;

	public override string BeforeStart()
	{
		_activeScale = RandomDistribution!.GetRandomItem().Scale;
		return string.Format(Description, _activeScale < 1 ? "Shrink" : "Grow");
	}

	public override void OnStart()
	{
		EnsurePlayerExists();

		Player.mainObject.transform.localScale *= _activeScale;
	}

	public override void OnStop()
	{
		EnsurePlayerExists();

		if (Duration > 0)
		{
			Player.mainObject.transform.localScale /= _activeScale;
		}
		else
		{
			Plugin.Logger.LogWarn("Player will not automatically return to the default size.");
		}
	}

	public override void FromData(Effect data, StatusCallback callback)
	{
		Scales = null;
		base.FromData(data, callback);
	}

	protected override void ParseAttribute(Effect.Attribute attribute)
	{
		switch (attribute.Name)
		{
			case nameof(Scales):
				attribute.ParseAttribute(ScaleData.ParseMany, out var scales);
				Scales = scales;
				RandomDistribution = new RandomDistribution<ScaleData>(Scales);
				break;
			default:
				throw attribute.Invalid();
		}
	}

	protected override bool GetSuccess() => Scales != null && RandomDistribution != null;

	public override Effect ToData()
	{
		return new Effect() {
			Id = this.Id.ToString(),
			Duration = this.Duration,
			Weight = this.Weight,
			Attributes = new[] {
				new Effect.Attribute(nameof(Scales), string.Join(ScaleData.DELIMITER.ToString(), (object[])Scales!)),
			}
		};
	}
}
