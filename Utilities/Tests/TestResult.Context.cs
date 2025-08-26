using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FrootLuips.Subnautica.Tests;

internal readonly partial record struct TestResult
{
	[Serializable]
	public class Context : Exception
	{
		public bool Passed { get; }

		public Context(bool passed, string message) : base(message)
		{
			Passed = passed;
		}

		protected Context(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
