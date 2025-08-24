using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FrootLuips.Subnautica.Logging;

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
		catch (Exception)
		{
			return false;
		}
	}

	/// <summary>
	/// Validates an <paramref name="object"/> using the given <paramref name="validator"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="object"></param>
	/// <param name="validator"></param>
	/// <returns><inheritdoc cref="Validate{T}(T, IEnumerator{Exception}, IValidator{T}.ValidationCallback?)"/></returns>
	public static ValidationResult<T> Validate<T>(this T @object, IValidator<T> validator)
	{
		return Validate(@object, validator.Validate(@object), callback: validator.GetSuccess);
	}

	/// <summary>
	/// Validates the given <paramref name="object"/>, iterating over the <paramref name="validator"/> then returning the result.
	/// </summary>
	/// <remarks>
	/// Successful validation is determined by whether any issues were found, or the result of <paramref name="callback"/> if one is provided.
	/// </remarks>
	/// <typeparam name="T"></typeparam>
	/// <param name="object"></param>
	/// <param name="validator"></param>
	/// <param name="callback"></param>
	/// <returns>Whether validation succeeded or not, along with any issues that may have occurred.</returns>
	public static ValidationResult<T> Validate<T>(this T @object,
		IEnumerator<Exception> validator,
		IValidator<T>.ValidationCallback? callback = null)
	{
		var errors = new List<Exception>();
		callback ??= (static (o, e) => o != null && e.Count == 0)!;
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
		return new ValidationResult<T>(@object, callback(@object, errors), errors);
	}

	/// <summary>
	/// Aggregates a collection of <see cref="Exception"/>s.
	/// </summary>
	/// <param name="exceptions"></param>
	/// <param name="message"></param>
	/// <returns></returns>
	public static AggregateException ToAggregate(this IEnumerable<Exception> exceptions, string? message = null)
	{
		return string.IsNullOrWhiteSpace(message)
			? new AggregateException(exceptions)
			: new AggregateException(message, exceptions);
	}
}
