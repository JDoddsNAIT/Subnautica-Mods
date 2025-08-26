using System.Collections.Generic;
using FrootLuips.Subnautica.Extensions;

namespace FrootLuips.Subnautica.Helpers;
#nullable disable
/// <summary>
/// Compares two lists by value instead of by reference.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ListComparer<T> :
	IEqualityComparer<List<T>>, IEqualityComparer<T[]>, IEqualityComparer<IReadOnlyList<T>>,
	IEqualityComparer<IReadOnlyCollection<T>>, IEqualityComparer<IEnumerable<T>>
{
	/// <summary>
	/// The <see cref="IEqualityComparer{T}"/> used to compare two <typeparamref name="T"/> values.
	/// </summary>
	public IEqualityComparer<T> ValueComparer { get; set; }

	/// <summary>
	/// Constructs a <see cref="ListComparer{T}"/> with the default comparer.
	/// </summary>
	public ListComparer() : this(null) { }

	/// <summary>
	/// Constructs a <see cref="ListComparer{T}"/> with the given comparer.
	/// </summary>
	/// <param name="valueComparer"></param>
	public ListComparer(IEqualityComparer<T> valueComparer)
	{
		ValueComparer = valueComparer;
	}

	/// <inheritdoc/>
	public bool Equals(List<T> x, List<T> y)
	{
		return (this as IEqualityComparer<IReadOnlyList<T>>).Equals(x, y);
	}

	/// <inheritdoc/>
	public bool Equals(T[] x, T[] y)
	{
		return (this as IEqualityComparer<IReadOnlyList<T>>).Equals(x, y);
	}

	/// <inheritdoc/>
	public bool Equals(IReadOnlyList<T> x, IReadOnlyList<T> y)
	{
		if (x is null && y is null)
			return true;
		else if (x is null || y is null)
			return false;
		else if (x.Count != y.Count)
			return false;

		int length = x.Count;
		bool equals = true;

		for (int i = 0; i < length && equals; i++)
		{
			T a = x[i], b = y[i];
			equals = (a, b) switch {
				(_, _) when ValueComparer is not null => ValueComparer.Equals(a, b),
				(not null, _) => a.Equals(b),
				(_, not null) => b.Equals(a),
				(null, null) => true,
			};
		}
		return equals;
	}

	/// <inheritdoc/>
	public bool Equals(IReadOnlyCollection<T> x, IReadOnlyCollection<T> y)
	{
		if (x is null || y is null || x.Count != y.Count)
			return false;
		return (this as IEqualityComparer<IEnumerable<T>>).Equals(x, y);
	}

	/// <inheritdoc/>
	public bool Equals(IEnumerable<T> x, IEnumerable<T> y)
	{
		if (x is null && y is null)
			return true;
		else if (x is null || y is null)
			return false;

		bool equals = true;
		foreach ((T a, T b) in (x, y))
		{
			equals = (a, b) switch {
				(_, _) when ValueComparer is not null => ValueComparer.Equals(a, b),
				(null, null) => true,
				(not null, _) => a.Equals(b),
				(_, not null) => b.Equals(a),
			};
			if (!equals)
				break;
		}
		return equals;
	}
	
	/// <inheritdoc/>
	public int GetHashCode(List<T> obj) => obj.GetHashCode();
	/// <inheritdoc/>
	public int GetHashCode(T[] obj) => obj.GetHashCode();
	/// <inheritdoc/>
	public int GetHashCode(IReadOnlyList<T> obj) => obj.GetHashCode();
	/// <inheritdoc/>
	public int GetHashCode(IReadOnlyCollection<T> obj) => obj.GetHashCode();
	/// <inheritdoc/>
	public int GetHashCode(IEnumerable<T> obj) => obj.GetHashCode();
}
