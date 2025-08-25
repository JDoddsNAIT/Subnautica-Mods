using System.Collections.Generic;

namespace FrootLuips.Subnautica.Helpers;
/// <summary>
/// Interface for types used to construct <typeparamref name="T"/> arrays.
/// </summary>
/// <typeparam name="TBuilder">The builder type implementing this interface.</typeparam>
/// <typeparam name="T">The value type of the array.</typeparam>
public interface IArrayBuilder<out TBuilder, T> where TBuilder : IArrayBuilder<TBuilder, T>
{
	/// <summary>
	/// The total number of elements in the array.
	/// </summary>
	int Count { get; }

	/// <summary>
	/// Appends a set of <paramref name="values"/> to the final array.
	/// </summary>
	/// <param name="values"></param>
	/// <returns></returns>
	TBuilder Append(params T[] values);
	/// <summary>
	/// Removes all values from the <typeparamref name="TBuilder"/>
	/// </summary>
	void Clear();
	/// <summary>
	/// Constructs a new <typeparamref name="T"/> array.
	/// </summary>
	/// <returns></returns>
	T[] ToArray();
}

/// <summary>
/// Helper class for constructing arrays of <typeparamref name="T"/> values.
/// </summary>
/// <typeparam name="T"><inheritdoc cref="IArrayBuilder{TBuilder, T}"/></typeparam>
public class ArrayBuilder<T> : IArrayBuilder<ArrayBuilder<T>, T>
{
	private readonly List<T[]> _values;

	/// <summary>
	/// Constructs a new <see cref="ArrayBuilder{T}"/> with the given <paramref name="capacity"/> and a set of <paramref name="initialValues"/>.
	/// </summary>
	/// <param name="capacity"></param>
	/// <param name="initialValues"></param>
	public ArrayBuilder(int capacity, params T[] initialValues) => _values = new(capacity) { initialValues };
	/// <summary>
	/// Constructs a new <see cref="ArrayBuilder{T}"/> with the given <paramref name="capacity"/>.
	/// </summary>
	/// <param name="capacity"></param>
	public ArrayBuilder(int capacity) => _values = new(capacity);
	/// <summary>
	/// Constructs a new <see cref="ArrayBuilder{T}"/> with a set of <paramref name="initialValues"/>.
	/// </summary>
	/// <param name="initialValues"></param>
	public ArrayBuilder(params T[] initialValues) : this(capacity: 1, initialValues) { }
	/// <summary>
	/// Constructs a new <see cref="ArrayBuilder{T}"/>.
	/// </summary>
	public ArrayBuilder() : this(capacity: 1) { }

	/// <inheritdoc/>
	public int Count { get; private set; } = 0;

	/// <inheritdoc/>
	public ArrayBuilder<T> Append(params T[] values)
	{
		if (values.Length > 0)
			_values.Add(values);
		Count += values.Length;
		return this;
	}

	/// <inheritdoc/>
	public void Clear()
	{
		_values.Clear();
		Count = 0;
	}

	/// <inheritdoc/>
	public T[] ToArray()
	{
		int index = 0;
		var result = new T[this.Count];
		for (int i = 0; i < _values.Count; i++)
		{
			for (int j = 0; j < _values[i].Length; j++)
			{
				result[index++] = _values[i][j];
			}
		}
		return result;
	}
}

/// <summary>
/// Helper class for constructing arrays.
/// </summary>
public class ArrayBuilder : ArrayBuilder<object> { }
