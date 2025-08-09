using System;
using System.Collections.Generic;

namespace FrootLuips.Subnautica;
/// <summary>
/// Provides alternatives to Linq methods to reduce memory allocations.
/// </summary>
public static class Queries
{
	public static void Convert<T1, T2>(IReadOnlyList<T1> source, Func<T1, T2> selector, T2[] destination)
	{
		if (destination.Length != source.Count)
			destination = new T2[source.Count];

		for (int i = 0; i < source.Count; i++)
		{
			destination[i] = selector(source[i]);
		}
	}

	public static void Convert<T1, T2>(IReadOnlyList<T1> source, Func<T1, T2> selector, List<T2> destination)
	{
		destination.Capacity = source.Count;
		for (int i = 0; i < source.Count; i++)
		{
			destination.Add(selector(source[i]));
		}
	}

	public static void Copy<T>(IReadOnlyList<T> source, T[] destination)
	{
		Queries.Convert<T, T>(source, selector: obj => obj, destination);
	}

	public static void Copy<T>(IReadOnlyList<T> source, List<T> destination)
	{
		Queries.Convert<T, T>(source, selector: obj => obj, destination);
	}

	public static void Filter<T>(IList<T> list, Predicate<T> predicate)
	{
		for (int i = list.Count - 1; i >= 0; i--)
		{
			if (!predicate(list[i]))
				list.RemoveAt(i);
		}
	}

	public static void FilterDuplicates<T>(IList<T> list)
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
		Queries.Filter<T>(list, predicate: isUnique);
	}
}
