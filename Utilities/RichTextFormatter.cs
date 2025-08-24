using System;
using System.Globalization;

namespace FrootLuips.Subnautica;
/// <summary>
/// Custom format provider used for unity's rich text.
/// </summary>
public sealed class RichTextFormatter : Singleton<RichTextFormatter>, IFormatProvider, ICustomFormatter
{
	/// <inheritdoc/>
	public string Format(string format, object text, IFormatProvider formatProvider)
	{
		string[] fmt = format.ToLowerInvariant().Split('=');
		try
		{
			return fmt[0] switch {
				_ when fmt.Length is < 1 or > 2 => HandleOtherFormats(format, text),
				"b" or "i" => Tag(fmt[0], text),
				"c" or "color" => Tag("color", text, fmt[1]),
				"s" or "size" when int.TryParse(fmt[1], out int size) => Tag("size", text, size),
				"m" or "material" when int.TryParse(fmt[1], out int material) => Tag("material", text, material),
				_ => HandleOtherFormats(format, text)
			};
		}
		catch (Exception ex)
		{
			throw new FormatException($"The format of '{format}' is invalid", ex);
		}
	}

	/// <inheritdoc/>
	public object? GetFormat(Type formatType)
	{
		if (formatType == typeof(ICustomFormatter))
			return this;
		else
			return null;
	}

	/// <summary>
	/// Generates a simple rich text tag.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="text"></param>
	/// <param name="arg"></param>
	/// <returns></returns>
	public static string Tag(string name, object text, object? arg = null)
	{
		return arg == null ? $"<{name}>{text}</{name}>" : $"<{name}={arg}>{text}</{name}>";
	}

	private string HandleOtherFormats(string format, object arg)
	{
		return arg switch {
			IFormattable formattable => formattable.ToString(format, CultureInfo.CurrentCulture),
			not null => arg.ToString(),
			_ => string.Empty
		};
	}
}
