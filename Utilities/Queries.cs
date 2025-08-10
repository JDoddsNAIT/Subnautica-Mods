using System;
using System.Collections.Generic;

namespace FrootLuips.Subnautica;
/// <summary>
/// Provides alternatives to Linq methods to reduce memory allocations.
/// </summary>
public static class Queries
{
	public static bool NotNull<T>(T? value) => value != null;

	public static void Convert<T1, T2>(T1[] source, Func<T1, T2> selector, T2[] destination)
	{
		if (destination.Length != source.Length)
			destination = new T2[source.Length];

		for (int i = 0; i < source.Length; i++)
		{
			destination[i] = selector(source[i]);
		}
	}

	public static void Convert<T1, T2>(List<T1> source, Func<T1, T2> selector, List<T2> destination)
	{
		destination.Capacity = source.Count;
		for (int i = 0; i < source.Count; i++)
		{
			destination[i] = selector(source[i]);
		}
	}

	public static void Copy<T>(T[] source, T[] destination)
	{
		Queries.Convert<T, T>(source, selector: static obj => obj, destination);
	}

	public static void Copy<T>(List<T> source, List<T> destination)
	{
		Queries.Convert<T, T>(source, selector: static obj => obj, destination);
	}

	public static void Filter<T>(ref T[] array, Predicate<T> filter)
	{
		List<T> result = new(array);
		Queries.Filter(result, filter);
		array = result.ToArray();
	}

	public static void Filter<T>(List<T> list, Predicate<T> filter)
	{
		for (int i = list.Count - 1; i >= 0; i--)
		{
			if (!filter(list[i]))
				list.RemoveAt(i);
		}
	}

	public static void FilterDuplicates<T>(List<T> list)
	{
		var duplicates = new List<T>(capacity: list.Count);
		bool isUnique(T value)
		{
			if (duplicates.Contains(value))
			{
				return false;
			}
			else
			{
				duplicates.Add(value);
				return true;
			}
		}
		Queries.Filter<T>(list, filter: isUnique);
	}
}
