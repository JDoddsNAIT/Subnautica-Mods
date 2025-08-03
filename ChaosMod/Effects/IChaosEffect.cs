using System;
using System.Collections;
using FrootLuips.ChaosMod.Logging;
using FrootLuips.ChaosMod.Objects;

namespace FrootLuips.ChaosMod.Effects;
internal interface IChaosEffect : IDistributable
{
	ChaosEffect Id { get; }
	string Description { get; }
	float Duration { get; }

	IEnumerator Activate();

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


