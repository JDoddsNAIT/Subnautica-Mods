using FrootLuips.ChaosMod.Utilities;
using UnityEngine;

using Random = UnityEngine.Random;

namespace FrootLuips.ChaosMod.Effects;
internal class ReaperRain : BaseChaosEffect
{
	public ReaperRain() : base(ChaosEffect.ReaperRain, duation: 30f) { }

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
		base.FromData(data, callback);

		Height = null;
		SpawnsPerSecond = null;

		List<string> errors = new();
		try
		{
			Validate(ValidateAttributes(data.Attributes));
		}
		catch (AggregateException agg)
		{
			foreach (var ex in agg)
			{
				errors.Add(ex.Message);
			}
		}
		finally
		{
			bool success = Height != null && SpawnsPerSecond != null && SpawnRadius != null;
			callback(errors, success);
		}
	}

	private IEnumerator<Exception> ValidateAttributes(Effect.Attribute[] attributes)
	{
		ExpectAttributeCount(attributes, count: 2);

		for (int i = 0; i < attributes.Length; i++)
		{
			Exception? exception = null;

			try
			{
				switch (attributes[i].Name)
				{
					case nameof(Height):
						attributes[i].ParseAttribute(int.Parse, out int height);
						Height = height;
						break;
					case nameof(SpawnsPerSecond):
						attributes[i].ParseAttribute(float.Parse, out float spawns);
						SpawnsPerSecond = spawns;
						break;
					case nameof(SpawnRadius):
						attributes[i].ParseAttribute(float.Parse, out float radius);
						SpawnRadius = radius;
						break;

					default:
						throw attributes[i].Invalid();
				}
			}
			catch (Exception ex)
			{
				exception = ex;
			}

			if (exception != null)
				yield return exception;

			continue;
		}
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
