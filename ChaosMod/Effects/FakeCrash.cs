namespace FrootLuips.ChaosMod.Effects;
internal class FakeCrash : BaseChaosEffect
{
	public FakeCrash() : base(ChaosEffect.FakeCrash, attributesExpected: 0, duration: 3f) { }

	public override string BeforeStart()
	{
		Plugin.Logger.LogDebug($"Initiating fake crash, duration {Duration} seconds.");
		var timer = System.Diagnostics.Stopwatch.StartNew();
		while (timer.Elapsed.TotalSeconds < Duration)
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
}
