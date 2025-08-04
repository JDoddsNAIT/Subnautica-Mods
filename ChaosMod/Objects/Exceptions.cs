using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrootLuips.ChaosMod.Objects;

[Serializable]
internal class UnexpectedAttributesException : Exception
{
	const string _MESSAGE = "Data does not have the expected number of attributes. ";

	public UnexpectedAttributesException() { }
	public UnexpectedAttributesException(int expectedAmount)
		: base(_MESSAGE + $"({expectedAmount})") { }
	public UnexpectedAttributesException(int expectedAmount, Exception inner)
		: base(_MESSAGE + $"({expectedAmount})", inner) { }

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
