using System;
using System.Collections.Generic;
using FrootLuips.Subnautica.Logging;

namespace FrootLuips.Subnautica;
/// <summary>
/// Helper class used for validation.
/// </summary>
[Obsolete]
public static partial class _Validation
{
	/// <summary>
	/// Iterates over <paramref name="validator"/>, and adds all errors to a list.
	/// </summary>
	/// <remarks>
	/// Successful validation is determined by whether any issues were found, or the result of <paramref name="callback"/> if one is provided.
	///  </remarks>
	/// <param name="validator"></param>
	/// <param name="callback"></param>
	/// <returns>Whether validation succeeded or not, along with any issues that may have occurred.</returns>
	public static Result Validate(IEnumerator<Exception> validator, Func<bool>? callback = null)
	{
		var errors = new List<Exception>();
		callback ??= () => errors.Count == 0;
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
		return new Result(callback(), errors);
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
	public static Result<T> ValidateObject<T>(T @object,
		IEnumerator<Exception> validator,
		Func<T, bool>? callback = null)
	{
		var errors = new List<Exception>();
		callback ??= o => o != null && errors.Count == 0;
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
		return new Result<T>(@object, callback(@object), errors);
	}

	/// <summary>
	/// Creates an <see cref="AggregateException"/> from a collection of <paramref name="exceptions"/>.
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
