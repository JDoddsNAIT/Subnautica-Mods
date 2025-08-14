namespace FrootLuips.ChaosMod.Effects;
internal class Fly : BaseChaosEffect
{
	public Fly() : base(ChaosEffect.Fly, attributesExpected: 0, duration: (float)ModOptions.FrequencyEnum.FiveMinutes) { }

	public override void OnStart()
	{
		EnsurePlayerExists();
		global::Player.main.groundMotor.flyCheatEnabled = true;
	}

	public override void OnStop()
	{
		EnsurePlayerExists();
		global::Player.main.groundMotor.flyCheatEnabled = false;
	}

	protected override void ParseAttribute(Effect.Attribute attribute) { }
	protected override bool GetSuccess() => true;
}
