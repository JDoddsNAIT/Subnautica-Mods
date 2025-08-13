namespace FrootLuips.ChaosMod.Utilities;
public interface IArrayBuilder<out TSelf, TValue>
{
	int Count { get; }

	TSelf Append(params TValue[] values);
	void Clear();
	TValue[] ToArray();
}

public class ArrayBuilder<T> : IArrayBuilder<ArrayBuilder<T>, T>
{
	private readonly List<T[]> _values;

	public int Count { get; private set; } = 0;

	public ArrayBuilder(int capacity, params T[] values) => _values = new(capacity) { values };
	public ArrayBuilder(int capacity) => _values = new(capacity);
	public ArrayBuilder(params T[] values) : this(capacity: 1, values) { }
	public ArrayBuilder() : this(capacity: 1) { }

	public ArrayBuilder<T> Append(params T[] values)
	{
		if (values.Length > 0)
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

public class ArrayBuilder : ArrayBuilder<object>
{
	public ArrayBuilder(int capacity, params object[] values) : base(capacity, values) { }
	public ArrayBuilder(int capacity) : base(capacity) { }
	public ArrayBuilder(params object[] values) : base(capacity: 1, values) { }
	public ArrayBuilder() : base(capacity: 1) { }
}

public class PathBuilder : ArrayBuilder<string>, IArrayBuilder<PathBuilder, string>
{
	public PathBuilder(int capacity, params string[] root) : base(capacity, root) { }
	public PathBuilder(int capacity) : base(capacity) { }
	public PathBuilder(params string[] root) : base(root) { }
	public PathBuilder() : base() { }

	public new PathBuilder Append(params string[] path) => (PathBuilder)base.Append(path);

	public override string ToString()
	{
		return this.Combine(removeWhiteSpace: false);
	}

	public string Combine(bool removeWhiteSpace = true)
	{
		var path = base.ToArray();
		if (removeWhiteSpace)
			SimpleQueries.Filter(ref path, SimpleQueries.NotNullOrWhiteSpace);
		return Path.Combine(path);
	}
}