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
}
