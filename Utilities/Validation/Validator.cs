using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FrootLuips.Subnautica.Validation;
/// <summary>
/// Helper class for validation.
/// </summary>
public static class Validator
{
	/// <summary>
	/// Wraps a <paramref name="method"/> in a <see langword="try"/>/<see langword="catch"/> statement.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="method"></param>
	/// <param name="result">The result of <paramref name="method"/>.</param>
	/// <returns><see langword="true"/> if <paramref name="result"/> is not <see langword="null"/> and no exception was thrown, otherwise <see langword="false"/>.</returns>
	public static bool Try<T>(Func<T> method, [NotNullWhen(true)] out T? result)
	{
		try
		{
			result = method();
			return result != null;
		}
		catch
		{
			result = default;
			return false;
		}
	}

	/// <summary>
	/// <inheritdoc cref="Try{T}(Func{T}, out T)"/>
	/// </summary>
	/// <param name="method"></param>
	/// <returns><see langword="true"/> if no exception was thrown, otherwise <see langword="false"/>.</returns>
	public static bool Try(Action method)
	{
		try
		{
			method();
			return true;
		}
		catch
		{
			return false;
		}
	}

	private static ValidationResult<TResult> Validate<T, TResult>(this T obj,
		IEnumerator<Exception> validator, ValidationCallback<T, TResult> callback)
	{
		List<Exception> errors = new();
		try
		{
			while (validator.MoveNext())
			{
				errors.Add(validator.Current);
			}
		}
		catch (Exception ex)
		{
			errors.Add(ex);
		}

		bool success = callback(obj, errors, out var result);
		return new ValidationResult<TResult>(success, errors, result);
	}

	/// <summary>
	/// Uses the <paramref name="validator"/> to validate <paramref name="obj"/>, and returns the result.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TResult"></typeparam>
	/// <param name="obj"></param>
	/// <param name="validator"></param>
	/// <returns></returns>
	public static ValidationResult<TResult> Validate<T, TResult>(this T obj,
		IValidator<T, TResult> validator)
	{
		return Validate<T, TResult>(obj, validator.Validate(obj), validator.Callback);
	}

	/// <summary>
	/// Validates this object.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TResult"></typeparam>
	/// <param name="obj"></param>
	/// <returns></returns>
	public static ValidationResult<TResult> Validate<T, TResult>(this T obj) where T : IValidator<T, TResult>
	{
		return Validate(obj, obj);
	}

	/// <summary>
	/// <inheritdoc cref="Validate{T, TResult}(T, IValidator{T, TResult})"/>
	/// </summary>
	/// <remarks>
	/// Successful validation is determined by whether the <paramref name="validator"/> returns any errors,
	/// or the result of <paramref name="callback"/> if specified.
	/// </remarks>
	/// <typeparam name="T"></typeparam>
	/// <param name="obj"></param>
	/// <param name="validator"></param>
	/// <param name="callback"></param>
	/// <returns></returns>
	public static ValidationResult<T> Validate<T>(this T obj,
		IEnumerator<Exception> validator, ValidationCallback<T>? callback = null)
	{
		callback ??= (static (o, e) => o != null && e.Count == 0);
		return Validate(obj, validator, DefaultCallback(callback));
	}

	private static ValidationCallback<T, T> DefaultCallback<T>(ValidationCallback<T> callback)
	{
		return (T? obj, IReadOnlyCollection<Exception> errors, [NotNullWhen(true)] out T? result) => {
			result = obj;
			return callback(obj, errors);
		};
	}

	/// <summary>
	/// Aggregates a collection of <see cref="Exception"/>s.
	/// </summary>
	/// <param name="exceptions"></param>
	/// <param name="message"></param>
	/// <returns></returns>
	public static AggregateException ToAggregate(this IEnumerable<Exception> exceptions,
		string? message = null)
	{
		return string.IsNullOrWhiteSpace(message)
			? new AggregateException(exceptions)
			: new AggregateException(message, exceptions);
	}
}
