using System.Collections.Generic;

namespace FrootLuips.RPGElements.Utilities;
internal class ArrayBuilder<T>
{
	private readonly List<T[]> _values = new();

	public ArrayBuilder<T> Append(params T[] values)
	{
		_values.Add(values);
		return this;
	}

	public T[] ToArray()
	{
		int length = 0;
		for (int i = 0; i < _values.Count; i++)
		{
			length += _values[i].Length;
		}

		var result = new T[length];
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
