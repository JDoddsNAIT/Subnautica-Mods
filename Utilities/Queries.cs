using System;
using System.Collections.Generic;
using static UnityEngine.UIElements.UIRAtlasManager;

namespace FrootLuips.Subnautica;
/// <summary>
/// Provides alternatives to Linq methods to reduce memory allocations.
/// </summary>
public static class Queries
{
	public static bool NotNull<T>(T? value) => value != null;

	public static void Convert<T1, T2>(IReadOnlyList<T1> source, Func<T1, T2> selector, ref T2[] destination)
	{
		int length = source.Count;
		if (destination.Length != length)
			Array.Resize(ref destination, length);

		for (int i = 0; i < length; i++)
		{
			destination[i] = selector(source[i]);
		}
	}

	public static void Convert<T1, T2>(IReadOnlyList<T1> source, Func<T1, T2> selector, List<T2> destination)
	{
		destination.Capacity = source.Count;
		for (int i = 0; i < source.Count; i++)
		{
			T2? item = selector(source[i]);
			if (i < destination.Count)
				destination[i] = item;
			else
				destination.Add(item);
		}
	}

	public static void Copy<T>(T[] source, ref T[] destination)
	{
		Queries.Convert<T, T>(source, selector: static obj => obj, ref destination);
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

	public static void FilterDuplicates<T>(ref T[] array)
	{
		var duplicates = new List<T>(capacity: array.Length);
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
		Queries.Filter<T>(ref array, filter: isUnique);
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
