using System.Collections;
using System.Collections.Generic;

namespace FrootLuips.Subnautica;
public class ArrayBuilder<T>
{
	private readonly List<T[]> _values;

	public ArrayBuilder(int capacity) => _values = new List<T[]>(capacity);
	public ArrayBuilder() => _values = new List<T[]>();

	public int Count { get; private set; } = 0;

	public ArrayBuilder<T> Append(params T[] values)
	{
		_values.Add(values);
		Count += values.Length;
		return this;
	}

	public void Clear()
	{
		_values.Clear();
		Count = 0;
	}

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

	public List<T> ToList()
	{
		var result = new List<T>();
		for (int i = 0; i < _values.Count; i++)
		{
			for (int j = 0; j < _values[i].Length; j++)
			{
				result.Add(_values[i][j]);
			}
		}
		return result;
	}

	public IEnumerable<T> ToEnumerable()
	{
		for (int i = 0; i < _values.Count; i++)
		{
			for (int j = 0; j < _values[i].Length; j++)
			{
				yield return _values[i][j];
			}
		}
	}
}

public class ArrayBuilder : ArrayBuilder<object> { }

public class FilePathBuilder : ArrayBuilder<string>
{
	public override string ToString()
	{
		return System.IO.Path.Combine(this.ToArray());
	}
}
