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
		return string.Join(" ", text.Split(' '));
	}
}
