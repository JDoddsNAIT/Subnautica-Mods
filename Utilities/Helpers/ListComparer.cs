using System.Collections.Generic;

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
		return Queries.ValueEquals(x, y, ValueComparer);
	}

	/// <inheritdoc/>
	public bool Equals(T[] x, T[] y)
	{
		return Queries.ValueEquals(x, y, ValueComparer);
	}

	/// <inheritdoc/>
	public bool Equals(IReadOnlyList<T> x, IReadOnlyList<T> y)
	{
		return Queries.ValueEquals(x, y, ValueComparer);
	}

	/// <inheritdoc/>
	public bool Equals(IReadOnlyCollection<T> x, IReadOnlyCollection<T> y)
	{
		return Queries.ValueEquals(x, y, ValueComparer);
	}

	/// <inheritdoc/>
	public bool Equals(IEnumerable<T> x, IEnumerable<T> y)
	{
		return Queries.ValueEquals(x, y, ValueComparer);
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
