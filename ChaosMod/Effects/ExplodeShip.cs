using ScalarMonitor = global::Utils.ScalarMonitor;

namespace FrootLuips.ChaosMod.Effects;
internal class ExplodeShip : BaseChaosEffect
{
	public ExplodeShip() : base(ChaosEffect.ExplodeShip, attributesExpected: 0) { }

	private static readonly System.Reflection.FieldInfo _monitor = HarmonyLib.AccessTools.Field(typeof(CrashedShipExploder), "timeMonitor");

	public override void OnStart()
	{
		var obj = CrashedShipExploder.main;
		obj.timeToStartCountdown = ((ScalarMonitor)_monitor.GetValue(obj)).Get() - 25f + 1f;
		obj.timeToStartWarning = obj.timeToStartCountdown - 1f;
	}

	protected override void ParseAttribute(Effect.Attribute attribute) { }
	protected override bool GetSuccess() => true;
}
