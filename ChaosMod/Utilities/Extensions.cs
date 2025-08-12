namespace FrootLuips.ChaosMod.Utilities;
internal static class Extensions
{
	// input: ThisIsThe101stTEST
	// output: This Is The 101st Test

	/// <summary>
	/// Modifies a PascalCase string to add spaces.
	/// </summary>
	/// <param name="text"></param>
	/// <returns></returns>
	public static string GetDisplayString(this string text)
	{
		if (string.IsNullOrEmpty(text))
			return text;
		string result = "";

		for (int i = 0; i < text.Length; i++)
		{
			char p = result.Length > 0 ? result[^1] : '\0';
			char c = text[i];
			string next;

			if (c is ' ' or '_' && p is ' ')
			{
				next = string.Empty;
			}
			else if (c is '_' or ' ')
			{
				next = " ";
			}
			else if ((c.IsUpper() || c.IsDigit()) && !(p.IsUpper() || p.IsDigit()))
			{
				next = " " + c;
			}
			else if (c.IsUpper() && p.IsUpper())
			{
				next = c.ToLower().ToString();
			}
			else
			{
				next = c.ToString();
			}

			result += next;
			continue;
		}

		return result.Trim();
	}

	public static bool IsControl(this char c) => char.IsControl(c);
	public static bool IsDigit(this char c) => char.IsDigit(c);
	public static bool IsLetter(this char c) => char.IsLetter(c);
	public static bool IsLetterOrDigit(this char c) => char.IsLetterOrDigit(c);
	public static bool IsLower(this char c) => char.IsLower(c);
	public static bool IsNumber(this char c) => char.IsNumber(c);
	public static bool IsPunctuation(this char c) => char.IsPunctuation(c);
	public static bool IsSeparator(this char c) => char.IsSeparator(c);
	public static bool IsSymbol(this char c) => char.IsSymbol(c);
	public static bool IsUpper(this char c) => char.IsUpper(c);
	public static bool IsWhiteSpace(this char c) => char.IsWhiteSpace(c);
	public static bool IsBetween(this char c, char low, char high)
	{
		if (high < low) (low, high) = (high, low);
		return c >= low && c <= high;
	}

	public static char ToUpper(this char c) => char.ToUpper(c);
	public static char ToLower(this char c) => char.ToLower(c);

	public static IEnumerator<Exception> GetEnumerator(this AggregateException aggregate)
	{
		for (int i = 0; i < aggregate.InnerExceptions.Count; i++)
		{
			yield return aggregate.InnerExceptions[i];
		}
	}

	public static bool TryAdd<T>(this List<T> list, T? item)
	{
		if (item == null || list.Contains(item))
		{
			return false;
		}
		else
		{
			list.Add(item);
			return true;
		}
	}

	public static bool CopyItems<T>(this IReadOnlyList<T> from, ref T[] to, CopyMode mode = CopyMode.Replace)
	{
		int length = from.Count;
		int offset = 0;
		switch (mode)
		{
			case CopyMode.Replace:
				if (to.Length != length)
					Array.Resize(ref to, length);
				break;
			case CopyMode.Append:
				offset = to.Length;
				Array.Resize(ref to, to.Length + offset);
				break;
			default:
				return false;
		}

		for (int i = 0; i < length; i++)
		{
			to[i + offset] = from[i];
		}
		return true;
	}

	public static bool CopyItems<T>(this IReadOnlyList<T> from, ref List<T> to, CopyMode mode = CopyMode.Replace)
	{
		if (mode is not CopyMode.Replace or CopyMode.Append)
			return false;

		int length = from.Count;

		for (int i = 0; i < length; i++)
		{
			if (mode is CopyMode.Replace && i < to.Count)
				to[i] = from[i];
			else
				to.Add(from[i]);
		}
		return true;
	}
}

/// <summary>
/// Determines how the CopyItems extension methods will function.
/// </summary>
public enum CopyMode
{
	/// <summary>
	/// Replace all elements in destination with elements from source
	/// </summary>
	Replace,
	/// <summary>
	/// Append elements from source to destination
	/// </summary>
	Append,
}
