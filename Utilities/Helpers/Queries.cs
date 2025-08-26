using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FrootLuips.Subnautica.Helpers;
/// <summary>
/// Provides alternatives to Linq methods to reduce memory allocations.
/// </summary>
public static class Queries
{
	/// <summary>
	/// Evaluates if a <paramref name="value"/> is not <see langword="null"/>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="value"></param>
	/// <returns></returns>
	public static bool NotNull<T>([NotNullWhen(true)] T? value) => value != null;

	/// <summary>
	/// Converts a <typeparamref name="T1"/> collection into a <typeparamref name="T2"/> array.
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <param name="source"></param>
	/// <param name="converter"></param>
	/// <param name="destination"></param>
	public static void Convert<T1, T2>(IReadOnlyList<T1> source, Converter<T1, T2> converter, ref T2[] destination)
	{
		int length = source.Count;
		if (destination.Length != length)
			Array.Resize(ref destination, length);

		for (int i = 0; i < length; i++)
		{
			destination[i] = converter(source[i]);
		}
	}

	/// <summary>
	/// Converts a <typeparamref name="T1"/> collection into a list of <typeparamref name="T2"/>s.
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <param name="source"></param>
	/// <param name="converter"></param>
	/// <param name="destination"></param>
	public static void Convert<T1, T2>(IReadOnlyList<T1> source, Converter<T1, T2> converter, List<T2> destination)
	{
		for (int i = 0; i < Math.Max(source.Count, destination.Count); i++)
		{
			T2? item = converter(source[i]);

			if (i >= source.Count && i < destination.Count)
				destination.RemoveAt(i);
			else if (i < destination.Count)
				destination[i] = item;
			else
				destination.Add(item);
		}
	}

	/// <summary>
	/// Copies the elements from the <paramref name="source"/> into the <paramref name="destination"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="source"></param>
	/// <param name="destination"></param>
	public static void Copy<T>(IReadOnlyList<T> source, ref T[] destination)
	{
		Convert(source, converter: static obj => obj, ref destination);
	}

	/// <summary>
	/// Copies the elements from the <paramref name="source"/> into the <paramref name="destination"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="source"></param>
	/// <param name="destination"></param>
	public static void Copy<T>(IReadOnlyList<T> source, List<T> destination)
	{
		Convert(source, converter: static obj => obj, destination);
	}

	/// <summary>
	/// Filters out all elements of the <paramref name="array"/> that do not pass the <paramref name="filter"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="array"></param>
	/// <param name="filter"></param>
	public static void Filter<T>(ref T[] array, Predicate<T> filter)
	{
		List<T> result = new(array);
		Filter(result, filter);
		Copy(result, ref array);
	}

	/// <summary>
	/// Filters out all elements of the <paramref name="list"/> that do not pass the <paramref name="filter"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
	/// <param name="filter"></param>
	public static void Filter<T>(List<T> list, Predicate<T> filter)
	{
		for (int i = list.Count - 1; i >= 0; i--)
		{
			if (!filter(list[i]))
				list.RemoveAt(i);
		}
	}

	/// <summary>
	/// Removes all duplicate elements from the <paramref name="array"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="array"></param>
	public static void FilterDuplicates<T>(ref T[] array)
	{
		Copy(GetUniqueItems(array), ref array);
	}

	/// <summary>
	/// Removes all duplicate elements from the <paramref name="list"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
	public static void FilterDuplicates<T>(List<T> list)
	{
		Copy(GetUniqueItems(list), list);
	}

	private static IReadOnlyList<T> GetUniqueItems<T>(IReadOnlyList<T> values)
	{
		var uniqueItems = new List<T>(capacity: values.Count);
		for (int i = 0; i < values.Count; i++)
		{
			if (!uniqueItems.Contains(values[i]))
				uniqueItems.Add(values[i]);
			else continue;
		}
		return uniqueItems;
	}

	/// <summary>
	/// Filters <see langword="null"/> values out of the <paramref name="array"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="array"></param>
	public static void FilterNulls<T>(ref T[] array) => Filter(ref array, filter: NotNull);

	/// <summary>
	/// Filters <see langword="null"/> values out of the <paramref name="list"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
	public static void FilterNulls<T>(List<T> list) => Filter(list, filter: NotNull);
}
