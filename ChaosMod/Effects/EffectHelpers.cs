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

	public delegate T? Getter<T>();

	/// <summary>
	/// Used when setting the property value of a <see cref="IChaosEffect"/> from an attribute.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="name"></param>
	/// <param name="getter"></param>
	/// <param name="setter"></param>
	/// <param name="value"></param>
	/// <exception cref="Exception"></exception>
	public static void SetProperty<T>(string name, Getter<T> getter, Action<T> setter, T value)
	{
		var val = getter();
		if (val != null)
		{
			setter(value);
			throw new Exception($"	{name}' has been overwritten by another.");
		}
		else
		{
			setter(value);
		}
	}
}
