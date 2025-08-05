namespace FrootLuips.ChaosMod.Utilities;
public class ArrayBuilder<T>
{
	private readonly List<T[]> _values = new();

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
}

public class ArrayBuilder : ArrayBuilder<object> { }
