using System;
using System.Collections;
using System.Collections.Generic;

using FrootLuips.ChaosMod.Logging;
using FrootLuips.ChaosMod.Objects;
using FrootLuips.ChaosMod.Utilities;

namespace FrootLuips.ChaosMod.Effects;
internal class RandomTeleport : IChaosEffect
{
	public const char BIOME_SEPARATOR = ',';

	public ChaosEffect Id { get; } = ChaosEffect.RandomTeleport;
	public string? Description { get; set; } = "";
	public float Duration { get; set; } = 0;
	public int Weight { get; set; } = 100;

	public BiomeType[]? Biomes { get; set; } = null;

	public IEnumerator Activate()
	{
		// TODO: Teleport the player to a randomly selected biome.
		yield return null;
	}

	public void FromData(Effect data, StatusCallback callback)
	{
		this.Duration = data.Duration;
		this.Weight = data.Weight;

		Biomes = null;
		List<string> errors = new();
		try
		{
			Assertions.Validate(ValidateAttributes(data.Attributes));
		}
		catch (AggregateException ex)
		{
			for (int i = 0; i < ex.InnerExceptions.Count; i++)
			{
				errors.Add(ex.InnerExceptions[i].Message);
			}
		}
		finally
		{
			bool success = Biomes != null && Biomes.Length > 0;
			callback(errors, success);
		}
	}

	private IEnumerator<Exception> ValidateAttributes(Effect.Attribute[] attributes)
	{
		EffectHelpers.ExpectAttributeCount(attributes, count: 1);

		var attribute = attributes[0];
		Exception? exception = null;
		try
		{
			List<BiomeType> biomes = new();
			List<Exception> errors = new();
			string[] parts = attribute.Value.Split(BIOME_SEPARATOR);

			for (int i = 0; i < parts.Length; i++)
			{
				try
				{
					var biome = (BiomeType)Enum.Parse(typeof(BiomeType), parts[i]);
					biomes.Add(biome);
				}
				catch (Exception ex)
				{
					errors.Add(ex);
					continue;
				}
			}

			if (biomes.Count == 0)
				throw new InvalidAttributeException(attribute);

			if (errors.Count > 0)
				throw new AggregateException(errors);
		}
		catch (Exception ex)
		{
			exception = ex;
		}

		if (exception is AggregateException agg)
		{
			for (int i = 0; i < agg.InnerExceptions.Count; i++)
			{
				yield return agg.InnerExceptions[i];
			}
		}
		else if (exception is not null)
			yield return exception;
	}

	public Effect ToData()
	{
		return new Effect() {
			Id = this.Id.ToString(),
			Duration = this.Duration,
			Weight = this.Weight,
			Attributes = new[] {
				new Effect.Attribute(nameof(Biomes), string.Join(BIOME_SEPARATOR.ToString(), Biomes))
			}
		};
	}
}
