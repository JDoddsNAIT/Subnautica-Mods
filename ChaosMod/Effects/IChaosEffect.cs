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
	public string Id { get; set; } = "";
	public string Description { get; set; } = "";
	public float Duration { get; set; }
	public int Weight { get; set; }
	public Tag[] Tags { get; set; } = Array.Empty<Tag>();

	[Serializable]
	internal record class Tag(string Name, string Value);
}


