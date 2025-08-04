using System;
using System.Collections;
using System.Collections.Generic;

using FrootLuips.ChaosMod.Logging;
using FrootLuips.ChaosMod.Utilities;

namespace FrootLuips.ChaosMod.Effects;
internal class ReaperRain : IChaosEffect
{
	public ChaosEffect Id { get; } = ChaosEffect.ReaperRain;
	public string? Description { get; set; } = "";
	public float Duration { get; private set; } = 30f;
	public int Weight { get; private set; } = 100;

	/// <summary>
	/// Height in metres above sea level where the reapers will spawn.
	/// </summary>
	public int? Height { get; set; } = null;
	/// <summary>
	/// Amount of reapers spawned every second over the duration.
	/// </summary>
	public float? SpawnsPerSecond { get; set; } = null;

	public IEnumerator Activate()
	{
		// TODO: Summon an amount of reapers in the sky around the player.
		yield return null;
	}

	public void FromData(Effect data, StatusCallback callback)
	{
		this.Duration = data.Duration;
		this.Weight = data.Weight;

		Height = null;
		SpawnsPerSecond = null;

		List<string> errors = new();
		try
		{
			Assertions.Validate(ValidateAttributes(data.Attributes));
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

	private IEnumerator<Exception> ValidateAttributes(Effect.Attribute[] attributes)
	{
		EffectHelpers.ExpectAttributeCount(attributes, count: 2);

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

	public Effect ToData() => new() {
		Id = this.Id.ToString(),
		Duration = this.Duration,
		Weight = this.Weight,
		Attributes = new[] {
			new Effect.Attribute(nameof(Height), Height.ToString()),
			new Effect.Attribute(nameof(SpawnsPerSecond), SpawnsPerSecond.ToString()),
		},
	};
}
