using System;
using System.Collections;
using System.Collections.Generic;
using FrootLuips.ChaosMod.Logging;
using FrootLuips.ChaosMod.Utilities;
using static FrootLuips.ChaosMod.Utilities.Utilities;

namespace FrootLuips.ChaosMod.Effects;
internal class ReaperRain : IChaosEffect
{
	public ChaosEffect Id { get; } = ChaosEffect.ReaperRain;
	public string Description { get; set; } = "";
	public float Duration { get; set; } = 30f;
	public int Weight { get; set; } = 1;

	/// <summary>
	/// Height in metres above sea level where the reapers will spawn.
	/// </summary>
	public int? Height { get; set; } = 100;
	/// <summary>
	/// Amount of reapers spawned every second over the duration.
	/// </summary>
	public float? SpawnsPerSecond { get; set; } = 1;

	public IEnumerator Activate()
	{
		// TODO: Summon an amount of reapers in the sky around the player.
		throw new NotImplementedException();
	}

	public void FromData(EffectData data, StatusCallback callback)
	{
		this.Description = data.Description;
		this.Duration = data.Duration;
		this.Weight = data.Weight;

		Height = null;
		SpawnsPerSecond = null;

		List<string> errors = new();
		try
		{
			Assertions.Validate(ValidateTags(data.Tags));
		}
		catch (AggregateException ex)
		{
			foreach (var item in ex.InnerExceptions)
			{
				errors.Add(item.Message);
			}
		}
		finally
		{
			bool success = Height != null && SpawnsPerSecond != null;
			callback(errors, success);
		}
	}

	private IEnumerator<Exception> ValidateTags(EffectData.Tag[] tags)
	{
		for (int i = 0; i < tags.Length; i++)
		{
			Exception? exception = null;

			try
			{
				switch (tags[i].Name)
				{
					case nameof(Height):
						tags[i].ParseTag(int.TryParse, out int height);
						SetProperty(nameof(Height), GetHeight, SetHeight, height);
						break;
					case nameof(SpawnsPerSecond):
						tags[i].ParseTag(float.TryParse, out float spawns);
						SetProperty(nameof(SpawnsPerSecond), GetSpawns, SetSpawns, spawns);
						break;
					default:
						throw tags[i].Invalid();
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

	private int? GetHeight() => Height;
	private void SetHeight(int value) => Height = value;
	private float? GetSpawns() => SpawnsPerSecond;
	private void SetSpawns(float value) => SpawnsPerSecond = value;

	public EffectData ToData() => new() {
		Id = this.Id.ToString(),
		Description = this.Description,
		Duration = this.Duration,
		Weight = this.Weight,
		Tags = new[] {
			new EffectData.Tag(nameof(Height), Height.ToString()),
			new EffectData.Tag(nameof(SpawnsPerSecond), SpawnsPerSecond.ToString()),
		},
	};
}
