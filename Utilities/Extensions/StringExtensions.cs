using System;

namespace FrootLuips.Subnautica.Extensions;
public static class StringExtensions
{
	/// <summary>
	/// Removes all repeating spaces.
	/// </summary>
	/// <param name="text"></param>
	/// <returns></returns>
	public static string TrimAll(this string text)
	{
		string result = "";
		for (int i = 0; i < text.Length; i++)
		{
			if (text[i] == ' ' && (i == 0 || text[i - 1] == ' '))
				continue;
			result += text[i];
		}
		return result.Trim();
	}

	/// <summary>
	/// Evaluates if a given string has any whitespace characters.
	/// </summary>
	/// <param name="str"></param>
	/// <returns></returns>
	public static bool ContainsWhitespace(this string str)
	{
		for (int i = 0; i < str.Length; i++)
		{
			if (char.IsWhiteSpace(str, i))
				return true;
		}
		return false;
	}

	/// <summary>
	/// Evaluate if a given string contains any of the specified <paramref name="characters"/>.
	/// </summary>
	/// <param name="str"></param>
	/// <param name="characters"></param>
	/// <returns></returns>
	public static bool ContainsAny(this string str, params char[] characters)
	{
		if (characters.Length <= 0)
			return false;

		bool contains = false;
		for (int i = 0; i < str.Length && !contains; i++)
		{
			if (Array.IndexOf(characters, str[i]) == -1)
				contains = true;
		}
		return contains;
	}

	/// <summary>
	/// Returns a substring of <paramref name="text"/>, from the start to the first instance of the <paramref name="substring"/>.
	/// </summary>
	/// <param name="text"></param>
	/// <param name="to"></param>
	/// <param name="inclusive">Whether to include the <paramref name="substring"/> in the result.</param>
	/// <returns></returns>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static string FromStartTo(this string text, string substring, bool inclusive = false)
	{
		if (text == substring)
			return inclusive ? text : string.Empty;

		for (int i = text.Length - 1; i >= 0; i--)
		{
			if (text[..i].EndsWith(substring))
			{
				if (!inclusive)
					i -= substring.Length;
				return text[..i];
			}
		}

		var message = string.Format("Substring '{0}' does not exist within '{1}'.", substring, text);
		throw new ArgumentOutOfRangeException(nameof(substring), message);
	}

	/// <summary>
	/// Returns a substring of <paramref name="text"/>, from the last instance of the <paramref name="substring"/> to the end.
	/// </summary>
	/// <param name="text"></param>
	/// <param name="substring"></param>
	/// <param name="inclusive">Whether to include the <paramref name="substring"/> in the result.</param>
	/// <returns></returns>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static string ToEndFrom(this string text, string substring, bool inclusive = true)
	{
		if (text == substring)
			return inclusive ? text : string.Empty;

		for (int i = 0; i < text.Length; i++)
		{
			if (text[i..].StartsWith(substring))
			{
				if (!inclusive)
					i += substring.Length;
				return text[i..];
			}
		}

		var message = string.Format("Substring '{0}' does not exist within '{1}'.", substring, text);
		throw new ArgumentOutOfRangeException(nameof(substring), message);
	}
}
