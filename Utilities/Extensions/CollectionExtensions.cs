using System.Collections.Generic;

namespace FrootLuips.Subnautica.Extensions;
/// <summary>
/// Extension methods for collection types.
/// </summary>
public static class CollectionExtensions
{
	/// <summary>
	/// Dequeues and item from the <paramref name="queue"/>, if there are any.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="queue"></param>
	/// <param name="item"></param>
	/// <returns><see langword="true"/> if an <paramref name="item"/> was dequeued, otherwise <see langword="false"/>.</returns>
	public static bool TryDequeue<T>(this Queue<T> queue, out T? item)
	{
		item = default;
		bool any = queue.Count > 0;
		if (any)
			item = queue.Dequeue();
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
	/// <returns><see langword="true"/> if both list have all the same elements.</returns>
	public static bool CompareValues<T>(this IReadOnlyList<T> a, IReadOnlyList<T> b)
	{
		if (a is null || b is null)
			return false;

		if (a.Count != b.Count)
			return false;

		for (int i = 0; i < a.Count; i++)
		{
			if (!Equals(a[i], b[i]))
				return false;
		}
		return true;
	}

	/// <summary>
	/// Compares two lists of <typeparamref name="T"/> values by each element using a <paramref name="comparer"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <param name="comparer"></param>
	/// <returns><see langword="true"/> if both list have all the same elements.</returns>
	public static bool CompareValues<T>(this IReadOnlyList<T> a, IReadOnlyList<T> b, IEqualityComparer<T> comparer)
	{
		if (a.Count != b.Count)
			return false;

		for (int i = 0; i < a.Count; i++)
		{
			if (!comparer.Equals(a[i], b[i]))
				return false;
		}
		return true;
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
