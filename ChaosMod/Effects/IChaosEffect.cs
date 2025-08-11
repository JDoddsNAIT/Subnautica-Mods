namespace FrootLuips.ChaosMod.Effects;
internal interface IChaosEffect : IDistributable
{
	ChaosEffect Id { get; }
	string? Description { get; set; }
	float Duration { get; }

	string BeforeStart();
	void OnStart();
	void Update(float time);
	void OnStop();

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


