using FrootLuips.ChaosMod.Utilities;
using UnityEngine;

namespace FrootLuips.ChaosMod.Effects;
internal class ScaleCreatures : BaseChaosEffect
{
	public ScaleCreatures() : base(ChaosEffect.ScaleCreatures, attributesExpected: 2) { }

	public ScaleData[]? Scales { get; set; } = null;
	public float? Range { get; set; } = null;

	private RandomDistribution<ScaleData>? RandomDistribution { get; set; } = null;
	private float _activeScale = 1;

	private List<Creature> _creatures = new();

	public override string BeforeStart()
	{
		_activeScale = RandomDistribution!.GetRandomItem().Scale;
		return string.Format(Description, _activeScale < 1 ? "Shrink" : "Grow");
	}

	public override void OnStart()
	{
		EnsurePlayerExists();

		// TODO: Improve efficiency
		bool withinRange(Creature c) => Vector3.Distance(c.transform.position, GetPlayerPosition()) < Range;
		EntityDB<Creature>.Entities.CopyItems(to: ref _creatures);
		_creatures = _creatures.SimpleWhere(withinRange);

		for (int i = 0; i < _creatures.Count; i++)
		{
			_creatures[i].transform.localScale *= _activeScale;
		}
	}

	public override void OnStop()
	{
		if (Duration <= 0)
			return;

		EnsurePlayerExists();

		for (int i = 0; i < _creatures.Count; i++)
		{
			_creatures[i].transform.localScale /= _activeScale;
		}
	}

	private static Vector3 GetPlayerPosition() => Player.main.transform.position;

	public override void FromData(Effect data, StatusCallback callback)
	{
		Scales = null;
		Range = null;
		RandomDistribution = null;
		base.FromData(data, callback);
	}

	protected override void ParseAttribute(Effect.Attribute attribute)
	{
		switch (attribute.Name)
		{
			case nameof(Scales):
				attribute.ParseAttribute(ScaleData.ParseMany, out var scales);
				Scales = scales;
				RandomDistribution = new RandomDistribution<ScaleData>(Scales);
				break;
			case nameof(Range):
				attribute.ParseAttribute(float.Parse, out var range);
				Range = range;
				break;
			default:
				throw attribute.Invalid();
		}
	}

	protected override bool GetSuccess()
	{
		return Scales != null && Range != null && RandomDistribution != null;
	}

	public override Effect ToData()
	{
		return new Effect() {
			Id = this.Id.ToString(),
			Duration = this.Duration,
			Weight = this.Weight,
			Attributes = new[] {
				new Effect.Attribute(nameof(Scales), string.Join(ScaleData.DELIMITER.ToString(), (object[])Scales!)),
				new Effect.Attribute(nameof(Range), Range.ToString())
			}
		};
	}
}
