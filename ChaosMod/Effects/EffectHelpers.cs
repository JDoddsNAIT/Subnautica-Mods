using System;
using FrootLuips.ChaosMod.Objects;
using FrootLuips.ChaosMod.Utilities;

namespace FrootLuips.ChaosMod.Effects;
internal static class EffectHelpers
{
	public static Exception Invalid(this Effect.Attribute attribute)
	{
		return new InvalidAttributeException(attribute);
	}

	public static void ParseAttribute<T>(this Effect.Attribute attribute, Func<string, T> parseFunc, out T value)
	{
		try
		{
			value = parseFunc(attribute.Value);
		}
		catch (Exception ex)
		{
			throw new InvalidAttributeException(attribute, ex);
		}
	}

	public static void ExpectAttributeCount(Effect.Attribute[] attributes, int count)
	{
		if (attributes.Length != count)
			throw new UnexpectedAttributesException(count);
	}

	public static ActiveEffect Activate(this IChaosEffect effect) => new(effect);
}
