using FrootLuips.ChaosMod.Utilities;
using UnityEngine;

using Random = UnityEngine.Random;

namespace FrootLuips.ChaosMod.Effects;
internal class ReaperRain : BaseChaosEffect
{
	public ReaperRain() : base(ChaosEffect.ReaperRain, attributesExpected: 3, duration: 30f) { }

	/// <summary>
	/// Height in metres above sea level where the reapers will spawn.
	/// </summary>
	public int? Height { get; set; } = null;
	/// <summary>
	/// Amount of reapers spawned every second over the duration.
	/// </summary>
	public float? SpawnsPerSecond { get; set; } = null;

	public float? SpawnRadius { get; set; } = null;

	private float SecondsPerSpawn => 1 / SpawnsPerSecond!.Value;
	private float _lastSpawnTime = 0;

	public override void OnStart()
	{
		_lastSpawnTime = 0;
	}

	public override void Update(float time)
	{
		if (time - _lastSpawnTime < SecondsPerSpawn)
			return;

		EnsurePlayerExists();

		_lastSpawnTime += SecondsPerSpawn;

		var spawnPosition = Random.insideUnitCircle;
		spawnPosition *= (float)SpawnRadius!;

		Vector3 trueSpawnPosition = new(
			x: spawnPosition.x,
			y: (float)Height!,
			z: spawnPosition.y);

		trueSpawnPosition += Player.main.transform.position with { y = 0 };

		InstantiateTechType(TechType.ReaperLeviathan, trueSpawnPosition);
	}

	public override void FromData(Effect data, StatusCallback callback)
	{
		Height = null;
		SpawnsPerSecond = null;
		SpawnRadius = null;
		base.FromData(data, callback);
	}

	protected override void ParseAttribute(Effect.Attribute attribute)
	{
		switch (attribute.Name)
		{
			case nameof(Height):
				attribute.ParseAttribute(int.Parse, out int height);
				Height = height;
				break;
			case nameof(SpawnsPerSecond):
				attribute.ParseAttribute(float.Parse, out float spawns);
				SpawnsPerSecond = spawns;
				break;
			case nameof(SpawnRadius):
				attribute.ParseAttribute(float.Parse, out float radius);
				SpawnRadius = radius;
				break;

			default:
				throw attribute.Invalid();
		}
	}

	protected override bool GetSuccess()
	{
		return Height != null && SpawnsPerSecond != null && SpawnRadius != null;
	}

	public override Effect ToData() => new() {
		Id = this.Id.ToString(),
		Duration = this.Duration,
		Weight = this.Weight,
		Attributes = new[] {
			new Effect.Attribute(nameof(Height), Height.ToString()),
			new Effect.Attribute(nameof(SpawnsPerSecond), SpawnsPerSecond.ToString()),
		},
	};
}
