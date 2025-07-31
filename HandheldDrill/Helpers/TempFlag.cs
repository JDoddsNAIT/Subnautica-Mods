using System;

namespace HandheldDrill.Helpers;

public delegate void Setter<in T>(T value);

internal struct TempFlag<T> : IDisposable
{
	public Setter<T> Setter { get; }

	public T finalValue;

	public TempFlag(Setter<T> setter, T newValue, T finalValue)
	{
		this.Setter = setter;
		this.finalValue = finalValue;
		Setter(newValue);
	}

	public readonly void Dispose()
	{
		Setter(finalValue);
	}
}
