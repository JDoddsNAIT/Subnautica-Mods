using System;
using System.Collections.Generic;

namespace FrootLuips.Subnautica;
/// <summary>
/// Provides alternatives to Linq methods to reduce memory allocations.
/// </summary>
public static class Queries
{
	public static bool NotNull<T>(T? value) => value != null;

	/// <summary>
	/// Converts a <typeparamref name="T1"/> collection into a <typeparamref name="T2"/> array.
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <param name="source"></param>
	/// <param name="selector"></param>
	/// <param name="destination"></param>
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

	/// <summary>
	/// Converts a <typeparamref name="T1"/> collection into a list of <typeparamref name="T2"/>s.
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <param name="source"></param>
	/// <param name="selector"></param>
	/// <param name="destination"></param>
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

	/// <summary>
	/// Copies the elements from the <paramref name="source"/> into the <paramref name="destination"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="source"></param>
	/// <param name="destination"></param>
	public static void Copy<T>(IReadOnlyList<T> source, ref T[] destination)
	{
		Convert(source, selector: static obj => obj, ref destination);
	}

	/// <summary>
	/// Copies the elements from the <paramref name="source"/> into the <paramref name="destination"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="source"></param>
	/// <param name="destination"></param>
	public static void Copy<T>(IReadOnlyList<T> source, List<T> destination)
	{
		Convert(source, selector: static obj => obj, destination);
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
		var duplicates = new List<T>(capacity: array.Length);
		for (int i = 0; i < array.Length; i++)
		{
			if (!duplicates.Contains(array[i]))
				duplicates.Add(array[i]);
			else
				continue;
		}
		Copy(duplicates, ref array);
	}

	/// <summary>
	/// Removes all duplicate elements from the <paramref name="list"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
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
		Filter<T>(list, filter: isUnique);
	}

	/// <summary>
	/// Filters <see langword="null"/> values out of the <paramref name="array"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="array"></param>
	public static void FilterNulls<T>(ref T[] array)
	{
		Filter(ref array, filter: NotNull);
	}

	/// <summary>
	/// Filters <see langword="null"/> values out of the <paramref name="list"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
	public static void FilterNulls<T>(List<T> list)
	{
		Filter(list, filter: NotNull);
	}
}
