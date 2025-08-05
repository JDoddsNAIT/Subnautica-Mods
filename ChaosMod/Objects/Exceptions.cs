using FrootLuips.ChaosMod.Logging;

namespace FrootLuips.ChaosMod.Objects;

[Serializable]
internal class UnexpectedAttributesException : Exception
{
	const string _MESSAGE = "Data does not have the expected number of attributes. ";

	public UnexpectedAttributesException(int expectedAmount)
		: base(_MESSAGE + $"({expectedAmount})")
	{ }
	public UnexpectedAttributesException(int expectedAmount, Exception inner)
		: base(_MESSAGE + $"({expectedAmount})", inner)
	{ }

	protected UnexpectedAttributesException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}


[Serializable]
internal class InvalidAttributeException : Exception
{
	const string _MESSAGE = " is an invalid data tag.";

	public InvalidAttributeException()
	{ }
	public InvalidAttributeException(Effects.Effect.Attribute attr)
		: base(attr.Name + _MESSAGE)
	{ }
	public InvalidAttributeException(Effects.Effect.Attribute attr, Exception inner)
		: base(attr.Name + _MESSAGE, inner)
	{ }
	
	protected InvalidAttributeException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}

[Serializable]
public class CommandFailedException : Exception
{
	const string _MESSAGE1 = "Failed to execute command",
		_MESSAGE2 = "Callback was not executed.";

	public CommandFailedException()
		: base(new LogMessage(notice: _MESSAGE1, message: _MESSAGE2))
	{ }
	public CommandFailedException(string methodName)
		: base(message: new LogMessage(
			notice: _MESSAGE1 + $" '{ConsoleCommands.COMMAND_NAME} {methodName.ToLower()}'",
			message: _MESSAGE2))
	{ }

	protected CommandFailedException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
