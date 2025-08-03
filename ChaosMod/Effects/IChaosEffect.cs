using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrootLuips.ChaosMod.Logging;
using FrootLuips.ChaosMod.Objects;

namespace FrootLuips.ChaosMod.Effects;
internal interface IChaosEffect : IDistributable
{
	string Id { get; }
	string Description { get; set; }
	float Duration { get; set; }

	void Activate();

	EffectData ToData();
	void FromData(EffectData data, StatusCallback callback);
}

[Serializable]
internal class EffectData
{
	public string Id { get; private set; } = "";
	public string Description { get; private set; } = "";
	public float Duration { get; private set; }
	public int Weight { get; private set; }
	public string[] Data { get; private set; } = Array.Empty<string>();
}
