namespace FrootLuips.ChaosMod.Effects;
internal class ExplodeShip : BaseChaosEffect
{
	public ExplodeShip() : base(ChaosEffect.ExplodeShip) { }

	private static readonly System.Reflection.FieldInfo _monitor = HarmonyLib.AccessTools.Field(typeof(CrashedShipExploder), "timeMonitor");

	public override void OnStart()
	{
		var obj = CrashedShipExploder.main;
		obj.timeToStartCountdown = ((global::Utils.ScalarMonitor)_monitor.GetValue(obj)).Get() - 25f + 1f;
		obj.timeToStartWarning = obj.timeToStartCountdown - 1f;
	}
}
