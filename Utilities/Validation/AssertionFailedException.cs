using System;

namespace FrootLuips.Subnautica.Validation;

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
