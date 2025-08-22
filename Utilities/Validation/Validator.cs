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
	/// Asserts that <paramref name="condition"/> is <see langword="true"/>, throwing an <see cref="AssertionFailedException"/> if not.
	/// </summary>
	/// <param name="condition"></param>
	/// <param name="message"></param>
	/// <returns>The value of <paramref name="condition"/>.</returns>
	/// <exception cref="AssertionFailedException"></exception>
	public static bool Assert(this bool condition, string? message = null)
	{
		if (!condition)
			throw new AssertionFailedException(message);
		return condition;
	}

	/// <summary>
	/// Asserts that <paramref name="condition"/> is <see langword="true"/>, logging a <paramref name="message"/> to the console if not.
	/// </summary>
	/// <param name="condition"></param>
	/// <param name="message"></param>
	/// <param name="logger"></param>
	/// <returns>The value of <paramref name="condition"/>.</returns>
	public static bool AssertLog(this bool condition, ILogger logger, string? message = null)
	{
		try
		{
			return Assert(condition, message);
		}
		catch (AssertionFailedException ex)
		{
			logger.LogError(ex.Message);
			return condition;
		}
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
		IValidator<T>.SuccessCallback? callback = null)
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
	/// <inheritdoc cref="Validate{T}(T, IEnumerator{Exception}, IValidator{T}.SuccessCallback?)"/>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="object"></param>
	/// <param name="validator"></param>
	/// <returns><inheritdoc cref="Validate{T}(T, IEnumerator{Exception}, IValidator{T}.SuccessCallback?)"/></returns>
	public static ValidationResult<T> Validate<T>(this T @object, IValidator<T> validator)
	{
		return Validate(@object, validator.Validate(@object), callback: validator.GetSuccess);
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
