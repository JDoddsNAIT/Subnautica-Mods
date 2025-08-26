using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FrootLuips.Subnautica.Helpers;

namespace FrootLuips.Subnautica.Validation;
/// <summary>
/// Helper class with methods for asserting conditions.
/// </summary>
public static class Assert
{
	private const string _EQUALS_MESSAGE = "\n\tExpected: {0}\n\n\tBut  was: {1}";

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
	/// Asserts that the given <paramref name="value"/> is not <see langword="null"/>.
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
	/// Asserts that two <typeparamref name="T"/> objects are equal.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="expected"></param>
	/// <param name="actual"></param>
	/// <returns></returns>
	/// <exception cref="AssertionFailedException"></exception>
	public static bool Equals<T>(T? expected, T? actual)
	{
		string message = string.Format(_EQUALS_MESSAGE, expected, actual);
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
		string message = string.Format(_EQUALS_MESSAGE, expected, actual);
		return (expected, actual) switch {
			(null, null) => true,
			(not null, not null) when comparer.Equals(expected, actual) => true,
			_ => throw new AssertionFailedException(message),
		};
	}

	/// <summary>
	/// Asserts that two collections have identical values.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="expected"></param>
	/// <param name="actual"></param>
	/// <returns></returns>
	/// <exception cref="AssertionFailedException"></exception>
	public static bool Equals<T>(IEnumerable<T> expected, IEnumerable<T> actual)
	{
		return Assert.Equals(expected, actual, new ListComparer<T>());
	}

	/// <summary>
	/// <inheritdoc cref="Equals{T}(IEnumerable{T}, IEnumerable{T})"/>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="expected"></param>
	/// <param name="actual"></param>
	/// <param name="valueComparer"></param>
	/// <returns></returns>
	/// <exception cref="AssertionFailedException"></exception>
	public static bool Equals<T>(IEnumerable<T> expected, IEnumerable<T> actual, IEqualityComparer<T> valueComparer)
	{
		return Assert.Equals(expected, actual, new ListComparer<T>(valueComparer));
	}

	/// <summary>
	/// Asserts that the given <paramref name="action"/> throws a <typeparamref name="T"/> exception.
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
