using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FrootLuips.Subnautica.Tests;
internal static class Assert
{
	private const string
		_EQUALS_MESSAGE = "\n\tExpected: {0}\n\tActual\t: {1}\n",
		_NULL_MESSAGE = "Expected a null value.",
		_NOT_NULL_MESSAGE = "Expected a not null value.",
		_THROWN_MESSAGE = "\n\tExpected: {0}\n\tThrown\t: {1}\n";

	public static void Equals<T>(T? expected, T? actual)
	{
		string message = string.Format(_EQUALS_MESSAGE, expected, actual);
		throw new TestResult.Context(object.Equals(expected, actual), message);
	}

	public static void Equals<T>(T? expected, T? actual, [DisallowNull] IEqualityComparer<T> comparer)
	{
		string message = string.Format(_EQUALS_MESSAGE, expected, actual);
		bool passed = (expected, actual) switch {
			(null, null) => true,
			(null, not null) or (not null, null) => false,
			(not null, not null) => comparer.Equals(expected, actual),
		};
		throw new TestResult.Context(passed, message);
	}

	public static void IsTrue(bool condition)
	{
		throw new TestResult.Context(condition, "");
	}

	public static void IsFalse(bool condition)
	{
		throw new TestResult.Context(!condition, "");
	}

	public static void NotNull(object? value)
	{
		bool passed = value switch {
			UnityEngine.Object obj => obj != null,
			null => false,
			not null => true,
		};
		throw new TestResult.Context(passed, _NOT_NULL_MESSAGE);
	}

	public static void IsNull(object? value)
	{
		bool passed = value switch {
			UnityEngine.Object obj => obj == null,
			null => true,
			not null => false,
		};
		throw new TestResult.Context(passed, _NULL_MESSAGE);
	}

	public static void Throws<T>(Action action) where T : Exception
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
		throw new TestResult.Context(passed, message);
	}
}
