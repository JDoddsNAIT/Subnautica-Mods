using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FrootLuips.Subnautica.Tests;

internal readonly partial record struct TestResult
{
	internal record class Context(string Group, string Name)
	{
		private const string
			_EQUALS_MESSAGE = "\n\tExpected: {0}\n\n\tActual  : {1}",
			_NULL_MESSAGE = "Expected a null value.",
			_NOT_NULL_MESSAGE = "Expected a not null value.",
			_THROWN_MESSAGE = "\n\tExpected: {0}\n\n\tThrown: {1}";

		public TestResult AssertEquals<T>(T? expected, T? actual)
		{
			string message = string.Format(_EQUALS_MESSAGE, expected, actual);
			return new TestResult(Group, Name, object.Equals(expected, actual), message);
		}

		public TestResult AssertEquals<T>(T? expected, T? actual, [DisallowNull] IEqualityComparer<T> comparer)
		{
			string message = string.Format(_EQUALS_MESSAGE, expected, actual);
			bool passed = (expected, actual) switch {
				(null, null) => true,
				(null, not null) or (not null, null) => false,
				(not null, not null) => comparer.Equals(expected, actual),
			};
			return new TestResult(Group, Name, passed, message);
		}

		public TestResult AssertIsTrue(bool condition)
		{
			return new TestResult(Group, Name, condition);
		}

		public TestResult AssertIsFalse(bool condition)
		{
			return new TestResult(Group, Name, !condition);
		}

		public TestResult AssertNotNull(object? value)
		{
			bool passed = value switch {
				UnityEngine.Object obj => obj != null,
				null => false,
				not null => true,
			};
			return new TestResult(Group, Name, passed, _NOT_NULL_MESSAGE);
		}

		public TestResult AssertIsNull(object? value)
		{
			bool passed = value switch {
				UnityEngine.Object obj => obj == null,
				null => true,
				not null => false,
			};
			return new TestResult(Group, Name, passed, _NULL_MESSAGE);
		}

		public TestResult AssertThrows<T>(Action action) where T : Exception
		{
			Type? exception;
			bool passed;
			try
			{
				action();
				exception = null;
				passed = false;
			}
			catch (Exception ex)
			{
				exception = ex.GetType();
				passed = ex is T;
			}

			string message = string.Format(_THROWN_MESSAGE,
				arg0: typeof(T),
				arg1: exception == null ? "No exception thrown" : exception.ToString());
			return new TestResult(Group, Name, passed, message);
		}
	}
}
