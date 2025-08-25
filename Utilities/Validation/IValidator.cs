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
	/// <param name="obj"></param>
	/// <returns></returns>
	public IEnumerator<Exception> Validate(T? obj);
	/// <summary>
	/// Final check for successful validation.
	/// </summary>
	/// <param name="obj"></param>
	/// <param name="issues"></param>
	/// <param name="result"></param>
	/// <returns></returns>
	public bool Callback(T? obj,
		IReadOnlyCollection<Exception> issues,
		[NotNullWhen(true)] out TResult? result);
}

/// <summary>
/// Success delegate for validation of a <typeparamref name="T"/> object.
/// </summary>
/// <param name="obj">The object being validated.</param>
/// <param name="issues"></param>
/// <param name="result"></param>
/// <returns></returns>
public delegate bool ValidationCallback<in T, TResult>(T? obj,
	IReadOnlyCollection<Exception> issues,
	[NotNullWhen(true)] out TResult? result);

/// <summary>
/// <inheritdoc cref="ValidationCallback{T, TResult}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="obj"></param>
/// <param name="issues"></param>
/// <returns></returns>
public delegate bool ValidationCallback<in T>(T? obj, IReadOnlyCollection<Exception> issues);

/// <summary>
/// <inheritdoc cref="IValidator{T, TResult}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Validator<T> : IValidator<T, T>
{
	/// <inheritdoc/>
	public abstract IEnumerator<Exception> Validate(T? obj);
	
	/// <inheritdoc cref="IValidator{T, TResult}.Callback(T, IReadOnlyCollection{Exception}, out TResult)"/>
	public abstract bool Callback(T? obj, IReadOnlyCollection<Exception> issues);

	bool IValidator<T, T>.Callback(T? obj,
		IReadOnlyCollection<Exception> issues,
		[NotNullWhen(true)] out T? result)
	{
		result = obj;
		return Callback(obj, issues);
	}
}
