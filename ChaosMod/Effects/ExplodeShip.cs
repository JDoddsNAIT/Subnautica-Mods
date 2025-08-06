using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrootLuips.ChaosMod.Effects;
internal class ExplodeShip : BaseChaosEffect
{
	private const string _EXPLODE_SHIP = "OnConsoleCommand_explodeship";

	public override ChaosEffect Id { get; } = ChaosEffect.ExplodeShip;
	public override string? Description { get; set; } = "";
	public override float Duration { get; set; } = 0;
	public override int Weight { get; set; } = 100;

	public override void OnStart()
	{
		var info = HarmonyLib.AccessTools.Method(typeof(CrashedShipExploder), name: _EXPLODE_SHIP);
		HarmonyLib.AccessTools.MethodDelegate<Action>(info, CrashedShipExploder.main).Invoke();
	}
}
