using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FrootLuips.Subnautica.Validation;
/// <summary>
/// Defines how <typeparamref name="T"/> values are validated.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TResult"></typeparam>
public interface IValidator<in T, TResult>
{
	/// <summary>
	/// Validates a <typeparamref name="T"/> object.
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	IEnumerator<Exception> Validate(T value);
	/// <summary>
	/// Gets the final result.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="errors"></param>
	/// <param name="result"></param>
	/// <returns></returns>
	bool TryGetResult(T value, IReadOnlyList<Exception> errors, [NotNullWhen(true), MaybeNullWhen(false)] out TResult result);
}

/// <summary>
/// A version of <see cref="IValidator{T, TResult}"/> that ensures proper nullability of value types.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TResult"></typeparam>
public interface IValueValidator<in T, TResult> : IValidator<T, Nullable<TResult>> where TResult : struct { }

/// <summary>
/// Retrieves the result of the <see cref="IValidator{T, TResult}.Validate(T)"/> function.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TResult"></typeparam>
/// <param name="value"></param>
/// <param name="errors"></param>
/// <param name="result"></param>
/// <returns></returns>
public delegate bool ValidationCallback<in T, TResult>(T value, IReadOnlyList<Exception> errors, [NotNullWhen(true), MaybeNullWhen(false)] out TResult result);
