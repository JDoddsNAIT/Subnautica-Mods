using System;
using System.Collections.Generic;
using FrootLuips.Subnautica.Logging;

namespace FrootLuips.Subnautica;
public static class Validation
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

	public static ValidationResult Validate(IEnumerator<Exception> validator, Func<bool>? callback = null)
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
		return new ValidationResult(callback(), errors);
	}

	public static AggregateException ToAggregate(this IEnumerable<Exception> exceptions, string? message = null)
	{
		return string.IsNullOrWhiteSpace(message)
			? new AggregateException(exceptions)
			: new AggregateException(message, exceptions);
	}
}


[Serializable]
public class AssertionFailedException : Exception
{
	private const string _MESSAGE = "Assertion failed";

	static string GetMessage(string? msg)
	{
		return _MESSAGE + (string.IsNullOrWhiteSpace(msg) ? "" : ": " + msg);
	}

	public AssertionFailedException()
		: base(_MESSAGE)
	{ }
	public AssertionFailedException(string? message)
		: base(GetMessage(message))
	{ }
	public AssertionFailedException(string? message, Exception inner)
		: base(GetMessage(message), inner)
	{ }
	protected AssertionFailedException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}

public readonly record struct ValidationResult(bool Success, List<Exception> Errors)
{
	public static implicit operator (bool result, List<Exception> errors)(ValidationResult value)
	{
		return (value.Success, value.Errors);
	}

	public static implicit operator ValidationResult((bool result, List<Exception> errors) value)
	{
		return new ValidationResult(value.result, value.errors);
	}

	public static implicit operator bool(ValidationResult obj) { return obj.Success; }
}