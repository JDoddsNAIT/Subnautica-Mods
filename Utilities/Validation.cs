using System;
using System.Collections.Generic;
using FrootLuips.Subnautica.Logging;

namespace FrootLuips.Subnautica;
/// <summary>
/// Helper class used for validation.
/// </summary>
public static partial class Validation
{
	/// <summary>
	/// Asserts that <paramref name="condition"/> is <see langword="true"/>, throwing an <see cref="AssertionFailedException"/> if not.
	/// </summary>
	/// <param name="condition"></param>
	/// <param name="message"></param>
	/// <returns>The value of <paramref name="condition"/>.</returns>
	/// <exception cref="AssertionFailedException"></exception>
	public static bool Assert(bool condition, string? message = null)
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
	public static bool AssertLog(bool condition, ILogger logger, string? message = null)
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

/// <summary>
/// The exception thrown when the condition of an Assertion fails.
/// </summary>
[Serializable]
public class AssertionFailedException : Exception
{
	private const string _MESSAGE = "Assertion failed";

	private static string GetMessage(string? msg)
	{
		return _MESSAGE + (string.IsNullOrWhiteSpace(msg) ? "" : ": " + msg);
	}

	/// <summary>
	/// Creates a new <see cref="AssertionFailedException"/>.
	/// </summary>
	public AssertionFailedException()
		: base(_MESSAGE)
	{ }
	/// <summary>
	/// Creates a new <see cref="AssertionFailedException"/> with a <paramref name="message"/> detailing what went wrong.
	/// </summary>
	/// <param name="message"></param>
	public AssertionFailedException(string? message)
		: base(GetMessage(message))
	{ }
	/// <summary>
	/// <inheritdoc cref="AssertionFailedException(string?)"/>
	/// </summary>
	/// <param name="message"></param>
	/// <param name="inner"></param>
	public AssertionFailedException(string? message, Exception inner)
		: base(GetMessage(message), inner)
	{ }
	/// <param name="info"></param>
	/// <param name="context"></param>
	protected AssertionFailedException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
