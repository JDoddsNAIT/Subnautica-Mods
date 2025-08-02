using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrootLuips.ChaosMod.Logging;

namespace FrootLuips.ChaosMod.Effects;
internal class ReaperRain : IChaosEffect
{
	public string Id { get; } = nameof(ChaosEffect.ReaperRain);
	public string Description { get; set; } = "";
	public float Duration { get; set; } = 30f;
	public int Weight { get; set; } = 1;

	public void Activate()
	{
		throw new NotImplementedException();
	}

	public void FromData(EffectData data, StatusCallback callback)
	{
		throw new NotImplementedException();
	}

	public EffectData ToData()
	{
		throw new NotImplementedException();
	}
}
