#pragma warning disable CS1591 // Missing XML comment
using System;
using System.Globalization;

namespace FrootLuips.Subnautica.Logging;
public partial class LogMessage
{
	/// <summary>
	/// Standard <see cref="IFormatProvider"/> for <see cref="LogMessage"/>s.
	/// </summary>
	public class FormatProvider : IFormatProvider, ICustomFormatter
	{
		public static FormatProvider Default { get; } = new FormatProvider();

		public readonly string Context, Message, Notice, Remarks;

		public FormatProvider(
			string context = "[{0}]",
			string message = "{0} -",
			string notice = "{0}",
			string remarks = "({0})")
		{
			Context = context.Trim();
			Message = message.Trim();
			Notice = notice.Trim();
			Remarks = remarks.Trim();
		}

#nullable disable
		public object GetFormat(Type formatType)
		{
			if (formatType == typeof(ICustomFormatter))
				return this;
			else
				return null;
		}
#nullable enable

		public string Format(string format, object arg, IFormatProvider formatProvider)
		{
			bool emptyArg = string.IsNullOrWhiteSpace(arg.ToString());
			return format.ToUpperInvariant() switch {
				"C" or "M" or "N" or "R" when emptyArg => string.Empty,
				"C" => string.Format(Context, arg),
				"M" => string.Format(Message, arg),
				"N" => string.Format(Notice, arg),
				"R" => string.Format(Remarks, arg),
				_ => HandleOthers(format, arg),
			};
		}

		private string HandleOthers(string format, object arg)
		{
			try
			{
				return arg switch {
					IFormattable formattable => formattable.ToString(format, CultureInfo.CurrentCulture),
					not null => arg.ToString(),
					_ => string.Empty
				};
			}
			catch (FormatException ex)
			{
				throw new FormatException($"The format of '{format}' is invalid", ex);
			}
		}
	}
}
