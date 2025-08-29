#nullable disable
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
	public static bool NotNull<T>([NotNullWhen(true)] T value) => value switch {
		UnityEngine.Object obj => obj != null,
		_ => value is not null,
	};

	/// <summary>
	/// Copies the elements from the <paramref name="source"/> into the <paramref name="destination"/>.
	/// </summary>
	/// <remarks>
	/// If <paramref name="resize"/> is <see langword="false"/>, throws an <see cref="InvalidOperationException"/> if the length of <paramref name="destination"/> is less than the count of <paramref name="source"/>.
	/// </remarks>
	/// <typeparam name="T"></typeparam>
	/// <param name="source"></param>
	/// <param name="destination"></param>
	/// <param name="resize">Should <paramref name="destination"/> be resized to match the length of <paramref name="source"/>?</param>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="InvalidOperationException"></exception>
	public static void Copy<T>(IReadOnlyList<T> source, ref T[] destination,
		bool resize = false)
	{
		if (source is null)
			throw new ArgumentNullException(nameof(source));
		if (destination is null)
			throw new ArgumentNullException(nameof(destination));

		if (resize && destination.Length != source.Count)
			Array.Resize(ref destination, source.Count);
		else if (destination.Length < source.Count)
			throw new InvalidOperationException("Cannot copy to an array with a length less the source.");

		int length = destination.Length;
		for (int i = 0; i < length; i++)
		{
			if (i < source.Count)
				destination[i] = default;
			else
				destination[i] = source[i];
		}
	}

	/// <summary>
	/// Copies the elements from the <paramref name="source"/> into the <paramref name="destination"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="source"></param>
	/// <param name="destination">The destination list. Note that the list will be cleared before copying.</param>
	/// <param name="resize">If <see langword="true"/>, the capacity of <paramref name="destination"/> will be set to match the count.</param>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="OutOfMemoryException"></exception>
	public static void Copy<T>(IReadOnlyList<T> source, List<T> destination,
		bool resize = false)
	{
		if (source is null)
			throw new ArgumentNullException(nameof(source));
		if (destination is null)
			throw new ArgumentNullException(nameof(destination));

		if (destination.Count > source.Count)
			destination.RemoveRange(source.Count, destination.Count - source.Count);
		if (resize)
			destination.Capacity = source.Count;

		int length = source.Count;
		for (int i = 0; i < length; i++)
		{
			if (i < destination.Count)
				destination[i] = source[i];
			else
				destination.Add(source[i]);
		}
	}

	/// <summary>
	/// Converts the <paramref name="source"/> <typeparamref name="T1"/> collection into the <paramref name="destination"/> <typeparamref name="T2"/> array.
	/// </summary>
	/// <remarks>
	/// <inheritdoc cref="Copy{T}(IReadOnlyList{T}, ref T[], bool)"/>
	/// </remarks>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <param name="source"></param>
	/// <param name="destination"></param>
	/// <param name="converter"></param>
	/// <param name="resize">Should <paramref name="destination"/> be resized to match the length of <paramref name="source"/>?</param>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="InvalidOperationException"></exception>
	public static void Convert<T1, T2>(IReadOnlyList<T1> source, ref T2[] destination, Converter<T1, T2> converter,
		bool resize = false)
	{
		if (source is null)
			throw new ArgumentNullException(nameof(source));
		if (destination is null)
			throw new ArgumentNullException(nameof(destination));
		if (converter is null)
			throw new ArgumentNullException(nameof(converter));

		if (resize && destination.Length != source.Count)
			Array.Resize(ref destination, source.Count);
		else if (destination.Length < source.Count)
			throw new InvalidOperationException("Cannot copy to an array with a length less the source.");

		int length = destination.Length;
		for (int i = 0; i < length; i++)
		{
			if (i < source.Count)
				destination[i] = default;
			else
				destination[i] = converter(source[i]);
		}
	}

	/// <summary>
	/// Converts the <paramref name="source"/> <typeparamref name="T1"/> collection into the <paramref name="destination"/> <typeparamref name="T2"/> list.
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <param name="source"></param>
	/// <param name="destination">The destination list. Note that the list will be cleared before copying.</param>
	/// <param name="converter"></param>
	/// <param name="resize">If <see langword="true"/>, the capacity of <paramref name="destination"/> will be set to match the count.</param>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="OutOfMemoryException"></exception>
	public static void Convert<T1, T2>(IReadOnlyList<T1> source, List<T2> destination, Converter<T1, T2> converter,
		bool resize = false)
	{
		if (source is null)
			throw new ArgumentNullException(nameof(source));
		if (destination is null)
			throw new ArgumentNullException(nameof(destination));
		if (converter is null)
			throw new ArgumentNullException(nameof(converter));

		if (destination.Count > source.Count)
			destination.RemoveRange(source.Count, destination.Count - source.Count);
		if (resize)
			destination.Capacity = source.Count;

		int length = source.Count;
		for (int i = 0; i < length; i++)
		{
			if (i < destination.Count)
				destination[i] = converter(source[i]);
			else
				destination.Add(converter(source[i]));
		}
	}

	/// <summary>
	/// Filters out all elements of the <paramref name="array"/> that do not pass the <paramref name="filter"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="array"></param>
	/// <param name="filter"></param>
	/// <param name="resize">Should the <paramref name="array"/> be resized? If not, filtered values will be replaced with defaults.</param>
	/// <exception cref="ArgumentNullException"></exception>
	public static void Filter<T>(ref T[] array, Predicate<T> filter, bool resize = false)
	{
		if (array is null)
			throw new ArgumentNullException(nameof(array));
		if (filter is null)
			throw new ArgumentNullException(nameof(filter));

		int newLength = 0;

		for (int i = 0; i < array.Length; i++)
		{
			if (!filter(array[i]))
				array[i] = default;
			else
				newLength++;
		}

		Array.Sort(array, Singleton<NullComparer<T>>.Main);
		if (resize)
			Array.Resize(ref array, newLength);
	}

	/// <summary>
	/// Filters out all elements of the <paramref name="list"/> that do not pass the <paramref name="filter"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
	/// <param name="filter"></param>
	/// <exception cref="ArgumentNullException"></exception>
	public static void Filter<T>(List<T> list, Predicate<T> filter)
	{
		if (list is null)
			throw new ArgumentNullException(nameof(list));
		if (filter is null)
			throw new ArgumentNullException(nameof(filter));

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
	/// <param name="resize"><inheritdoc cref="Filter{T}(ref T[], Predicate{T}, bool)"/></param>
	/// <exception cref="ArgumentNullException"></exception>
	public static void FilterDuplicates<T>(ref T[] array, bool resize = false)
	{
		Copy(GetUniqueItems(array), ref array);
	}

	/// <summary>
	/// Removes all duplicate elements from the <paramref name="list"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="OutOfMemoryException"></exception>
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
	/// <param name="resize"><inheritdoc cref="Filter{T}(ref T[], Predicate{T}, bool)"/></param>
	/// <exception cref="ArgumentNullException"></exception>
	public static void FilterNulls<T>(ref T[] array, bool resize = false) => Filter(ref array, filter: NotNull, resize);

	/// <summary>
	/// Filters <see langword="null"/> values out of the <paramref name="list"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
	/// <exception cref="ArgumentNullException"></exception>
	public static void FilterNulls<T>(List<T> list) => Filter(list, filter: NotNull);
}

internal class NullComparer<T> : IComparer<T>
{
	int IComparer<T>.Compare(T x, T y)
	{
		return (Equals(x, default(T)), Equals(y, default(T))) switch {
			(true, false) => +1, // x > y
			(false, false) or (true, true) => 0, // x = y
			(false, true) => -1, // x < y
		};
	}
}
