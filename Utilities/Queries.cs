using System;
using System.Collections.Generic;

namespace FrootLuips.Subnautica;
/// <summary>
/// Provides alternatives to Linq methods to reduce memory allocations.
/// </summary>
public static class Queries
{
	public static bool NotNull<T>(T? value) => value != null;

	public static void Convert<T1, T2>(IReadOnlyList<T1> source, Func<T1, T2> selector, ref T2[] destination)
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
			destination[i] = selector(source[i]);
		}
	}

	public static void Copy<T>(IReadOnlyList<T> source, ref T[] destination)
	{
		Queries.Convert<T, T>(source, selector: static obj => obj, ref destination);
	}

	public static void Copy<T>(IReadOnlyList<T> source, List<T> destination)
	{
		Queries.Convert<T, T>(source, selector: static obj => obj, destination);
	}

	public static void Filter<T>(IList<T> list, Predicate<T> predicate)
	{
		if (list is T[] arr)
		{
			var newList = new List<T>(capacity: arr.Length);
			for (int i = 0; i < newList.Capacity; i++)
			{
				if (predicate(arr[i]))
					newList.Add(arr[i]);
			}
			Queries.Copy(newList, ref arr);
		}
		else
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (!predicate(list[i]))
					list.RemoveAt(i);
			}
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
