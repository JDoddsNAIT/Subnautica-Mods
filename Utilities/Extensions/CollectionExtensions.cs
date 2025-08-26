using System;
using System.Collections.Generic;
using System.Linq;
using FrootLuips.Subnautica.Helpers;

namespace FrootLuips.Subnautica.Extensions;
/// <summary>
/// Extension methods for collection types.
/// </summary>
public static class CollectionExtensions
{
	/// <summary>
	/// Dequeues an item from the <paramref name="queue"/>, if there are any.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="queue"></param>
	/// <param name="item"></param>
	/// <returns><see langword="true"/> if an <paramref name="item"/> was dequeued, otherwise <see langword="false"/>.</returns>
	public static bool TryDequeue<T>(this Queue<T> queue, out T? item)
	{
		bool any = queue.Count > 0;
		item = any ? queue.Dequeue() : default;
		return any;
	}

	/// <summary>
	/// Pops an item off the <paramref name="stack"/>, if there are any.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="stack"></param>
	/// <param name="item"></param>
	/// <returns><see langword="true"/> if an <paramref name="item"/> was popped, otherwise <see langword="false"/>.</returns>
	public static bool TryPop<T>(this Stack<T> stack, out T? item)
	{
		bool any = stack.Count > 0;
		item = any ? stack.Pop() : default;
		return any;
	}

	/// <summary>
	/// Attempts to add a <paramref name="key"/> and it's associated <paramref name="value"/> to the <paramref name="dictionary"/>.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	/// <param name="dictionary"></param>
	/// <param name="key"></param>
	/// <param name="value"></param>
	/// <returns><see langword="true"/> if the <paramref name="key"/> was not present in the <paramref name="dictionary"/>, meaning the <paramref name="key"/> was added.</returns>
	public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
	{
		bool missing = !dictionary.ContainsKey(key);
		if (missing)
		{
			dictionary.Add(key, value);
		}
		return missing;
	}

	/// <summary>
	/// Attempts to add an <paramref name="item"/> to the <paramref name="list"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
	/// <param name="item"></param>
	/// <returns><see langword="true"/> if the <paramref name="item"/> was not present in the <paramref name="list"/>, meaning the <paramref name="item"/> was added.</returns>
	public static bool TryAdd<T>(this List<T> list, T item)
	{
		bool missing = !list.Contains(item);
		if (missing)
		{
			list.Add(item);
		}
		return missing;
	}

	/// <summary>
	/// Compares two lists of <typeparamref name="T"/> values by each element.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <returns><see langword="true"/> if both lists have all the same elements.</returns>
	public static bool CompareValues<T>(this IReadOnlyList<T> a, IReadOnlyList<T> b)
	{
		return new ListComparer<T>().Equals(a, b);
	}

	/// <inheritdoc cref="CompareValues{T}(IReadOnlyList{T}, IReadOnlyList{T})"/>
	public static bool CompareValues<T>(this IEnumerable<T> a, IEnumerable<T> b)
	{
		return new ListComparer<T>().Equals(a, b);
	}

	/// <summary>
	/// Compares two lists of <typeparamref name="T"/> values by each element using a <paramref name="comparer"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <param name="comparer"></param>
	/// <returns><see langword="true"/> if both lists have all the same elements.</returns>
	public static bool CompareValues<T>(this IReadOnlyList<T> a, IReadOnlyList<T> b, IEqualityComparer<T> comparer)
	{
		return new ListComparer<T>(comparer).Equals(a, b);
	}

	/// <inheritdoc cref="CompareValues{T}(IReadOnlyList{T}, IReadOnlyList{T}, IEqualityComparer{T})"/>
	public static bool CompareValues<T>(this IEnumerable<T> a, IEnumerable<T> b, IEqualityComparer<T> comparer)
	{
		return new ListComparer<T>(comparer).Equals(a, b);
	}

	/// <summary>
	/// Creates an enumerator for a pair of <see cref="IEnumerable{T}"/>s.
	/// </summary>
	/// <remarks>
	/// Values that are outside the bounds of either collection will be replaced with the default value
	/// in the final result.
	/// </remarks>
	/// <typeparam name="T"></typeparam>
	/// <param name="tuple"></param>
	/// <returns></returns>
	public static IEnumerator<(T?, T?)> GetEnumerator<T>(this (IEnumerable<T> x, IEnumerable<T> y) tuple)
	{
		IEnumerator<T> xEnumerator = tuple.x.GetEnumerator(), yEnumerator = tuple.y.GetEnumerator();
		bool xMove = xEnumerator.MoveNext(), yMove = yEnumerator.MoveNext();

		while (xMove | yMove)
		{
			yield return (xEnumerator.Current, yEnumerator.Current);
			xMove = xEnumerator.MoveNext();
			yMove = yEnumerator.MoveNext();
		}
	}

	/// <summary>
	/// Evaluates whether the <paramref name="collection"/> is null or empty.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="collection"></param>
	/// <returns></returns>
	public static bool IsNullOrEmpty<T>(this IReadOnlyCollection<T> collection)
	{
		return collection is null || collection.Count == 0;
	}
}
