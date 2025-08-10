namespace FrootLuips.Subnautica;
public static partial class Extensions
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
}
