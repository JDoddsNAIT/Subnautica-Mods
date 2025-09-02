#nullable disable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FrootLuips.Subnautica.Extensions;
using UnityObject = UnityEngine.Object;

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
	/// If <paramref name="resize"/> is <see langword="false"/>, throws an <see cref="ArgumentException"/> if the length of <paramref name="destination"/> is less than the count of <paramref name="source"/>.
	/// </remarks>
	/// <typeparam name="T"></typeparam>
	/// <param name="source"></param>
	/// <param name="destination"></param>
	/// <param name="resize">Should <paramref name="destination"/> be resized to match the length of <paramref name="source"/>?</param>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentException"></exception>
	public static void Copy<T>(IReadOnlyList<T> source, ref T[] destination, bool resize = false)
	{
		if (source is null)
			throw new ArgumentNullException(nameof(source));
		if (destination is null)
			throw new ArgumentNullException(nameof(destination));

		int length = source.Count;
		if (resize && destination.Length != length)
			Array.Resize(ref destination, length);
		else if (destination.Length < length)
			throw new ArgumentException("Cannot copy to an array that is smaller than the source.");

		for (int i = 0; i < destination.Length; i++)
		{
			destination[i] = i < length ? source[i] : default;
		}
	}

	/// <inheritdoc cref="Copy{T}(IReadOnlyList{T}, ref T[], bool)"/>
	public static void Copy<T>(IReadOnlyCollection<T> source, ref T[] destination, bool resize = false)
	{
		if (source is null)
			throw new ArgumentNullException(nameof(source));
		if (destination is null)
			throw new ArgumentNullException(nameof(destination));

		int length = source.Count;
		if (resize && destination.Length != length)
			Array.Resize(ref destination, length);
		else if (destination.Length < length)
			throw new ArgumentException("Cannot copy to an array that is smaller than the source.");

		int i = 0;
		foreach (var item in source)
		{
			destination[i++] = item;
		}
		for (; i < destination.Length; destination[i++] = default) ;
	}

	/// <summary>
	/// Copies the elements from the <paramref name="source"/> into the <paramref name="destination"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="source"></param>
	/// <param name="destination">The destination list. Note that the list will be cleared before copying.</param>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="OutOfMemoryException"></exception>
	public static void Copy<T>(IReadOnlyList<T> source, List<T> destination)
	{
		if (source is null)
			throw new ArgumentNullException(nameof(source));
		if (destination is null)
			throw new ArgumentNullException(nameof(destination));

		int length = source.Count;
		if (destination.Count > length)
			destination.RemoveRange(length, destination.Count - length);

		if (destination.Capacity < length)
			destination.Capacity = length;

		for (int i = 0; i < length; i++)
		{
			if (i < destination.Count)
				destination[i] = source[i];
			else
				destination.Add(source[i]);
		}
	}

	/// <inheritdoc cref="Copy{T}(IReadOnlyList{T}, List{T})"/>
	public static void Copy<T>(IReadOnlyCollection<T> source, List<T> destination)
	{
		if (source is null)
			throw new ArgumentNullException(nameof(source));
		if (destination is null)
			throw new ArgumentNullException(nameof(destination));

		int length = source.Count;
		if (destination.Count > length)
			destination.RemoveRange(length, destination.Count - length);

		if (destination.Capacity < length)
			destination.Capacity = length;

		int i = 0;
		foreach (var item in source)
		{
			if (i < destination.Count)
				destination[i] = item;
			else
				destination.Add(item);
			i++;
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
	/// <exception cref="ArgumentException"></exception>
	public static void Convert<T1, T2>(IReadOnlyList<T1> source, ref T2[] destination, Func<T1, T2> converter,
		bool resize = false)
	{
		if (source is null)
			throw new ArgumentNullException(nameof(source));
		if (destination is null)
			throw new ArgumentNullException(nameof(destination));
		if (converter is null)
			throw new ArgumentNullException(nameof(converter));

		int length = source.Count;
		if (resize && destination.Length != length)
			Array.Resize(ref destination, length);
		else if (destination.Length < length)
			throw new ArgumentException("Cannot copy to an array that is smaller than the source.");

		for (int i = 0; i < destination.Length; i++)
		{
			destination[i] = i < length ? converter(source[i]) : default;
		}
	}

	/// <inheritdoc cref="Convert{T1, T2}(IReadOnlyList{T1}, ref T2[], Func{T1, T2}, bool)"/>
	public static void Convert<T1, T2>(IReadOnlyCollection<T1> source, ref T2[] destination, Func<T1, T2> converter,
		bool resize = false)
	{
		if (source is null)
			throw new ArgumentNullException(nameof(source));
		if (destination is null)
			throw new ArgumentNullException(nameof(destination));
		if (converter is null)
			throw new ArgumentNullException(nameof(converter));

		int length = source.Count;
		if (resize && destination.Length != length)
			Array.Resize(ref destination, length);
		else if (destination.Length < length)
			throw new ArgumentException("Cannot copy to an array that is smaller than the source.");

		int i = 0;
		foreach (var item in source)
		{
			destination[i] = converter(item);
		}
		for (; i < destination.Length; destination[i++] = default) ;
	}

	/// <summary>
	/// Converts the <paramref name="source"/> <typeparamref name="T1"/> collection into the <paramref name="destination"/> <typeparamref name="T2"/> list.
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <param name="source"></param>
	/// <param name="destination">The destination list. Note that the list will be cleared before copying.</param>
	/// <param name="converter"></param>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="OutOfMemoryException"></exception>
	public static void Convert<T1, T2>(IReadOnlyList<T1> source, List<T2> destination, Func<T1, T2> converter)
	{
		if (source is null)
			throw new ArgumentNullException(nameof(source));
		if (destination is null)
			throw new ArgumentNullException(nameof(destination));
		if (converter is null)
			throw new ArgumentNullException(nameof(converter));

		int length = source.Count;
		if (destination.Count > length)
			destination.RemoveRange(length, destination.Count - length);

		if (destination.Capacity < length)
			destination.Capacity = length;

		for (int i = 0; i < length; i++)
		{
			if (i < destination.Count)
				destination[i] = converter(source[i]);
			else
				destination.Add(converter(source[i]));
		}
	}

	/// <inheritdoc cref="Convert{T1, T2}(IReadOnlyList{T1}, List{T2}, Func{T1, T2})"/>
	public static void Convert<T1, T2>(IReadOnlyCollection<T1> source, List<T2> destination, Func<T1, T2> converter)
	{
		if (source is null)
			throw new ArgumentNullException(nameof(source));
		if (destination is null)
			throw new ArgumentNullException(nameof(destination));
		if (converter is null)
			throw new ArgumentNullException(nameof(converter));

		int length = source.Count;
		if (destination.Count > length)
			destination.RemoveRange(length, destination.Count - length);

		if (destination.Capacity < length)
			destination.Capacity = length;

		int i = 0;
		foreach (var item in source)
		{
			if (i < destination.Count)
				destination[i] = converter(item);
			else
				destination.Add(converter(item));
			i++;
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

		int removed = 0;
		for (int i = 0; i < array.Length; i++)
		{
			if (!filter(array[i]))
			{
				array[i] = default;
				removed++;
				MoveToEnd(array, i);
			}
		}

		if (resize && removed > 0)
			Array.Resize(ref array, array.Length - removed);
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

	/// <summary>
	/// Compares two lists by their values.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="valueComparer">The comparer used to compare the values of the two lists.</param>
	/// <returns><see langword="true"/> if both lists have the same elements.</returns>
	public static bool ValueEquals<T>(IReadOnlyList<T> x, IReadOnlyList<T> y, IEqualityComparer<T> valueComparer = null)
	{
		switch (x, y)
		{
			case (null, null): return true;
			case (object, null) or (null, object): return false;
		}

		bool equals = x.Count == y.Count;
		T a, b;
		for (int i = 0; i < x.Count && equals; i++)
		{
			(a, b) = (x[i], y[i]);
			equals = valueComparer?.Equals(a, b) ?? CompareValues(a, b);
		}
		return equals;
	}

	/// <summary>
	/// Compares two collections by their values.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="valueComparer">The comparer used to compare the values.</param>
	/// <returns><see langword="true"/> if both collections have the same elements.</returns>
	public static bool ValueEquals<T>(IReadOnlyCollection<T> x, IReadOnlyCollection<T> y, IEqualityComparer<T> valueComparer = null)
	{
		switch (x, y)
		{
			case (null, null): return true;
			case (object, null) or (null, object): return false;
		}

		bool equals = x.Count == y.Count;
		return equals && ValueEquals((IEnumerable<T>)x, y, valueComparer);
	}

	/// <summary>
	/// Compares two <see cref="IEnumerable{T}"/> by their values.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="valueComparer">The comparer used to compare the values.</param>
	/// <returns><see langword="true"/> if both <see cref="IEnumerable{T}"/> have the same elements.</returns>
	public static bool ValueEquals<T>(IEnumerable<T> x, IEnumerable<T> y, IEqualityComparer<T> valueComparer = null)
	{
		switch (x, y)
		{
			case (null, null): return true;
			case (object, null) or (null, object): return false;
		}

		bool equals = true;
		var enumerator = (x, y).GetEnumerator();
		T a, b;
		while (enumerator.MoveNext() && equals)
		{
			(a, b) = enumerator.Current;
			equals = valueComparer?.Equals(a, b) ?? CompareValues(a, b);
		}
		return equals;
	}

	private static bool CompareValues<T>(T a, T b)
	{
		return (a, b) switch {
			(UnityObject obj, null) => obj == null,
			(null, UnityObject obj) => obj == null,
			(UnityObject objX, UnityObject objY) => objX == objY,

			(IEquatable<T> eq, _) => eq.Equals(b),
			(_, IEquatable<T> eq) => eq.Equals(a),

			_ => object.Equals(a, b),
		};
	}

	/// <summary>
	/// Shifts the value at the given <paramref name="index"/> to the end of the list of <paramref name="values"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="values"></param>
	/// <param name="index"></param>
	public static void MoveToEnd<T>(IList<T> values, int index)
	{
		int length = values.Count - 1;
		if (index >= length)
			return;

		for (int i = index; i < length; i++)
		{
			(values[i], values[i + 1]) = (values[i + 1], values[i]);
		}
	}
}
