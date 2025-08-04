using System;
using System.Collections.Generic;
using FrootLuips.ChaosMod.Logging;

namespace FrootLuips.ChaosMod.Utilities;

public static class Assertions
{
	/// <summary>
	/// Throws an <see cref="AssertionFailedException"/> if <paramref name="condition"/> evaluates to <see langword="false"/>.
	/// </summary>
	/// <param name="condition">The condition to evaluate.</param>
	/// <param name="message">A message describing what went wrong.</param>
	/// <returns><see langword="true"/> unless an exception is thrown.</returns>
	/// <exception cref="AssertionFailedException"></exception>
	public static bool Assert(bool condition, string message = "")
	{
		if (!condition)
			throw new AssertionFailedException(message);
		return true;
	}

	/// <summary>
	/// Logs an error message if <paramref name="condition"/> evaluates to <see langword="false"/>.
	/// </summary>
	/// <param name="condition">The condition to evaluate.</param>
	/// <param name="logger">The <see cref="ILogger"/> used to log the message.</param>
	/// <param name="message">A message describing what went wrong.</param>
	/// <returns>The value of <paramref name="condition"/>.</returns>
	public static bool AssertLog(bool condition, ILogger logger, string message = "")
	{
		if (!condition)
			logger.LogError(LogMessage.FromException(new AssertionFailedException(message)));
		return condition;
	}

	/// <summary>
	/// Validates the values of an object using the give <paramref name="enumerator"/>, and throws an <see cref="AggregateException"/> if there are any issues.
	/// </summary>
	/// <param name="enumerator"></param>
	/// <returns><see langword="true"/> unless an exception is thrown.</returns>
	/// <exception cref="AggregateException"></exception>
	public static bool Validate(IEnumerator<Exception> enumerator)
	{
		var errors = new List<Exception>();

		try
		{
			enumerator.Reset();
			while (enumerator.MoveNext())
			{
				errors.Add(enumerator.Current);
			}
		}
		catch (Exception ex)
		{
			errors.Add(ex);
		}

		if (errors.Count > 0)
			throw new AggregateException(errors.ToArray());
		else
			return true;
	}
}

[Serializable]
public class AssertionFailedException : Exception
{
	internal const string ASSERTION_FAILED = "Assertion Failed";

	private static string GetMessage(string message)
	{
		var sb = new System.Text.StringBuilder();
		sb.Append(ASSERTION_FAILED);
		if (!string.IsNullOrWhiteSpace(message))
			sb.Append(": ").Append(message);
		return sb.ToString();
	}

	public AssertionFailedException()
		: base(GetMessage(string.Empty))
	{ }
	public AssertionFailedException(string message)
		: base(GetMessage(message))
	{ }
	public AssertionFailedException(string message, Exception inner)
		: base(GetMessage(message), inner)
	{ }
	protected AssertionFailedException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}