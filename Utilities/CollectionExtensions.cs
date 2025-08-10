using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		if (contains)
		{
			dict.Add(key, value);
		}
		return contains;
	}

	public static bool CompareValues<T>(this IReadOnlyList<T> a, IReadOnlyList<T> b)
		where T : IEquatable<T>
	{
		bool equals = a.Count == b.Count;
		
		if (!equals)
			return false;

		for (int i = 0; i < a.Count && equals; i++)
		{
			equals &= a[i] != null && b[i] != null && a[i]!.Equals(b[i]!);
		}
		return equals;
	}
}
