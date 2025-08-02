using System.Collections.Generic;

namespace FrootLuips.RPGElements.Utilities;
internal class ArrayBuilder<T>
{
	private readonly List<T[]> _values = new();

	public int Count { get; private set; }

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
		var result = new T[this.Count];
		int index = 0;
		for (int i = 0; i < _values.Count; i++)
		{
			for (int j = 0; j < _values[i].Length; j++)
			{
				result[index] = _values[i][j];
				index++;
			}
		}
		return result;
	}
}
