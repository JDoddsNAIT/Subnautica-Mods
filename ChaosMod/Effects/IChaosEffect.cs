using System;
using System.Collections;
using FrootLuips.ChaosMod.Logging;
using FrootLuips.ChaosMod.Objects;

namespace FrootLuips.ChaosMod.Effects;
internal interface IChaosEffect : IDistributable
{
	ChaosEffect Id { get; }
	string? Description { get; set; }
	float Duration { get; }

	IEnumerator Activate();

	Effect ToData();
	void FromData(Effect data, StatusCallback callback);

}

[Serializable]
internal class Effect
{
	public string Id { get; set; } = "";
	public float Duration { get; set; }
	public int Weight { get; set; }
	public Attribute[] Attributes { get; set; } = Array.Empty<Attribute>();

	[Serializable]
	internal record class Attribute(string Name, string Value);
}


