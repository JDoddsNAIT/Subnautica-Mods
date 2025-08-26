using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FrootLuips.Subnautica.Extensions;

namespace FrootLuips.Subnautica.Validation;
/// <summary>
/// Helper class with methods for asserting conditions.
/// </summary>
public static class Assert
{
	private const string _EQUALS_MESSAGE = "'{0}' is not equal to '{1}'";

	/// <summary>
	/// Asserts that <paramref name="condition"/> is <see langword="true"/>, throwing an <see cref="AssertionFailedException"/> if not.
	/// </summary>
	/// <param name="condition"></param>
	/// <param name="message"></param>
	/// <returns>The value of <paramref name="condition"/>.</returns>
	/// <exception cref="AssertionFailedException"></exception>
	public static bool IsTrue(bool condition, string? message = null)
	{
		if (!condition)
			throw new AssertionFailedException(message);
		return condition;
	}

	/// <summary>
	/// Asserts that <paramref name="condition"/> is <see langword="false"/>, throwing an <see cref="AssertionFailedException"/> if not.
	/// </summary>
	/// <param name="condition"></param>
	/// <param name="message"></param>
	/// <returns></returns>
	/// <exception cref="AssertionFailedException"></exception>
	public static bool IsFalse(bool condition, string? message = null)
	{
		if (condition)
			throw new AssertionFailedException(message);
		return !condition;
	}

	/// <summary>
	/// Asserts that the given <paramref name="value"/> is not <see langword="null"/>, throwing an <see cref="AssertionFailedException"/> if not.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="message"></param>
	/// <returns></returns>
	/// <exception cref="AssertionFailedException"></exception>
	public static bool NotNull([NotNull] object? value, string? message = null)
	{
		return value switch {
			null => throw new AssertionFailedException(message),
			UnityEngine.Object obj when obj == null => throw new AssertionFailedException(message),
			_ => true
		};
	}

	/// <summary>
	/// Asserts that two <typeparamref name="T"/> objects are equal, throwing an <see cref="AssertionFailedException"/> if not.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="expected"></param>
	/// <param name="actual"></param>
	/// <returns></returns>
	/// <exception cref="AssertionFailedException"></exception>
	public static bool Equals<T>(T? expected, T? actual)
	{
		string message = GetEqualsMessage(expected, actual);

		return Assert.IsTrue(object.Equals(expected, actual), message);
	}

	/// <summary>
	/// <inheritdoc cref="Equals{T}(T, T)"/>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="expected"></param>
	/// <param name="actual"></param>
	/// <param name="comparer"></param>
	/// <returns></returns>
	/// <exception cref="AssertionFailedException"></exception>
	public static bool Equals<T>(T? expected, T? actual, [DisallowNull] IEqualityComparer<T> comparer)
	{
		string message = GetEqualsMessage(expected, actual);

		return (expected, actual) switch {
			(null, null) => true,
			(not null, not null) when comparer.Equals(expected, actual) => true,
			_ => throw new AssertionFailedException(message),
		};
	}

	private static string GetEqualsMessage<T>(T? expected, T? actual)
	{
		string exp = (expected as IEnumerable)?.ToSeparatedString() ?? expected?.ToString() ?? "Null";
		string act = (actual as IEnumerable)?.ToSeparatedString() ?? actual?.ToString() ?? "Null";
		return string.Format(_EQUALS_MESSAGE, exp, act);
	}

	/// <summary>
	/// Asserts that the given <paramref name="action"/> throws a <typeparamref name="T"/> exception, throwing an <see cref="AssertionFailedException"/> if not.
	/// </summary>
	/// <typeparam name="T">The exception type.</typeparam>
	/// <param name="action"></param>
	/// <param name="message"></param>
	/// <returns></returns>
	/// <exception cref="AssertionFailedException"></exception>
	public static bool Throws<T>(Action action, string? message = null)
		where T : Exception
	{
		try
		{
			action();
			throw new AssertionFailedException(message);
		}
		catch (AssertionFailedException)
		{
			throw;
		}
		catch (Exception ex)
		{
			return ex is T ? true : throw new AssertionFailedException(message);
		}
	}
}
