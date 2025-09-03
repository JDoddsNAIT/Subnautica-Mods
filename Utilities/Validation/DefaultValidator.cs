using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FrootLuips.Subnautica.Validation;

/// <summary>
/// Default <see cref="IValidator{T, TResult}"/> implementation for reference types.
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class DefaultValidator<T> : IValidator<T, T> where T : class
{
	private readonly Func<T, Exception?>[] _predicates;

	/// <inheritdoc cref="DefaultValidator{T}"/>
	public DefaultValidator(params Func<T, Exception?>[] predicates)
	{
		_predicates = predicates;
	}

	/// <inheritdoc/>
	bool IValidator<T, T>.TryGetResult(T value, IReadOnlyList<Exception> errors, [MaybeNullWhen(false), NotNullWhen(true)] out T? result)
	{
		return Validator.DefaultCallback(value, errors, out result);
	}

	IEnumerator<Exception> IValidator<T, T>.Validate(T value)
	{
		for (int i = 0; i < _predicates.Length; i++)
		{
			var ex = _predicates[i](value);
			if (ex is not null)
				yield return ex;
		}
	}
}

/// <summary>
/// Default <see cref="IValidator{T, TResult}"/> implementation for value types.
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class DefaultValueValidator<T> : IValueValidator<T, T> where T : struct
{
	private readonly Func<T, Exception?>[] _predicates;

	/// <inheritdoc cref="DefaultValidator{T}"/>
	public DefaultValueValidator(params Func<T, Exception?>[] predicates)
	{
		_predicates = predicates;
	}

	bool IValidator<T, T?>.TryGetResult(T value, IReadOnlyList<Exception> errors, [MaybeNullWhen(false), NotNullWhen(true)] out T? result)
	{
		return Validator.DefaultValueCallback(value, errors, out result);
	}

	IEnumerator<Exception> IValidator<T, T?>.Validate(T value)
	{
		for (int i = 0; i < _predicates.Length; i++)
		{
			var ex = _predicates[i](value);
			if (ex is not null)
				yield return ex;
		}
	}
}
