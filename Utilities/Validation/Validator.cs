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

	internal static bool DefaultCallback<T>(T value, IReadOnlyList<Exception> errors, [MaybeNullWhen(false), NotNullWhen(true)] out T? result) where T : class
	{
		bool success = errors.Count == 0;
		result = success ? value : null;
		return success;
	}

	internal static bool DefaultValueCallback<T>(T value, IReadOnlyList<Exception> errors, [MaybeNullWhen(false), NotNullWhen(true)] out Nullable<T> result) where T : struct
	{
		bool success = errors.Count == 0;
		result = success ? value : null;
		return success;
	}

	/// <summary>
	/// <inheritdoc cref="Validate{T, TResult}(T, IValidator{T, TResult})"/>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="value"></param>
	/// <param name="validator"></param>
	/// <returns></returns>
	public static ValidationResult<T?> Validate<T>(T value, IEnumerator<Exception> validator) where T : class
	{
		return Validate<T, T?>(value, validator, DefaultCallback);
	}
	/// <inheritdoc cref=" Validate{T}(T, IEnumerator{Exception})"/>
	public static ValidationResult<T?> ValidateValue<T>(T value, IEnumerator<Exception> validator) where T : struct
	{
		return Validate<T, T?>(value, validator, DefaultValueCallback);
	}

	/// <summary>
	/// Validates the <paramref name="value"/> using the <paramref name="validator"/> and returns the result.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TResult"></typeparam>
	/// <param name="value"></param>
	/// <param name="validator"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException"></exception>
	public static ValidationResult<TResult?> Validate<T, TResult>(T value, IValidator<T, TResult> validator)
	{
		return Validate<T, TResult?>(value, validator.Validate(value), validator.TryGetResult);
	}

	/// <summary>
	/// <inheritdoc cref="Validate{T, TResult}(T, IValidator{T, TResult})"/>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TResult"></typeparam>
	/// <param name="value"></param>
	/// <param name="validator"></param>
	/// <param name="callback"></param>
	/// <returns></returns>
	public static ValidationResult<TResult?> Validate<T, TResult>(T value, IEnumerator<Exception> validator, ValidationCallback<T, TResult?> callback)
	{
		bool success;
		var errors = new List<Exception>();
		TResult? result;
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
		finally
		{
			success = callback(value, errors, out result);
		}
		return new ValidationResult<TResult?>(success, errors, result);
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
