using System;
using System.Collections.Generic;

namespace FrootLuips.RPGElements.Utilities;
/// <summary>
/// Provides alternatives to Linq methods that uses <see langword="for"/> instead of <see langword="foreach"/> to reduce allocations.
/// </summary>
internal static class SimpleQueries
{
	public static TResult[] SimpleSelect<TSource, TResult>(this IReadOnlyList<TSource> values, Func<TSource, TResult> selector)
	{
		if (selector is null)
			throw new ArgumentNullException(nameof(selector));
		var results = new TResult[values?.Count ?? 0];
		for (int i = 0; i < values.Count; i++)
		{
			results[i] = selector(values[i]);
		}
		return results;
	}

	public static List<T> SimpleWhere<T>(this IReadOnlyList<T> values, Func<T, bool> predicate)
	{
		if (predicate is null)
			throw new ArgumentNullException(nameof(predicate));
		var results = new List<T>(capacity: values?.Count ?? 0);
		for (int i = 0; i < values.Count; i++)
		{
			if (predicate(values[i]))
				results.Add(values[i]);
		}
		return results;
	}
	public static List<TResult> SimpleWhere<TSource, TResult>(this IReadOnlyList<TSource> values, Func<TSource, bool> predicate, Func<TSource, TResult> selector)
	{
		if (predicate is null)
			throw new ArgumentNullException(nameof(predicate));
		if (selector is null)
			throw new ArgumentNullException(nameof(selector));
		var results = new List<TResult>(capacity: values?.Count ?? 0);
		for (int i = 0; i < values.Count; i++)
		{
			if (predicate(values[i]))
				results.Add(selector(values[i]));
		}
		return results;
	}

	public delegate bool TryFunc<in T, TResult>(T input, out TResult result);
	public static List<TResult> TrySelect<TSource, TResult>(this IReadOnlyList<TSource> values, TryFunc<TSource, TResult> trySelector)
	{
		if (trySelector is null)
			throw new ArgumentNullException(nameof(trySelector));
		var results = new List<TResult>(capacity: values?.Count ?? 0);
		for (int i = 0; i < values.Count; i++)
		{
			if (trySelector(values[i], out TResult result))
				results.Add(result);
		}
		return results;
	}
}
