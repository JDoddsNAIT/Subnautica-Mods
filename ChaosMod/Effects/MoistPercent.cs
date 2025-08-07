namespace FrootLuips.ChaosMod.Effects;
internal class MoistPercent : BaseChaosEffect
{
	public MoistPercent() : base(ChaosEffect.MoistPercent) { }

	public override void OnStart()
	{
		EnsurePlayerExists();
		Player.main.SetMotorMode(Player.MotorMode.Walk);
	}
}
