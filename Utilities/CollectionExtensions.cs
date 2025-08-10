using System.Collections.Generic;

namespace FrootLuips.Subnautica;
public partial class Extensions
{
	public static bool TryDequeue<T>(this Queue<T> queue, out T? value)
	{
		value = default;
		bool any = queue.Count > 0;
		if (any)
			value = queue.Dequeue();
		return any;
	}

	public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
	{
		bool contains = dict.ContainsKey(key);
		if (!contains)
		{
			dict.Add(key, value);
		}
		return contains;
	}

	/// <summary>
	/// Compares two lists of <typeparamref name="T"/> vaules by each element.
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
	/// Compares two lists of <typeparamref name="T"/> vaules by each element using a <paramref name="comparer"/>.
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
