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

		public readonly string Context, Notice, Message, Remarks;

		public FormatProvider(
			string context = "[{0}]",
			string notice = "{0} -",
			string message = "{0}",
			string remarks = "({0})")
		{
			Context = context.Trim();
			Notice = notice.Trim();
			Message = message.Trim();
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
			string ufmt = format.ToUpperInvariant();
			bool emptyArg = string.IsNullOrWhiteSpace(arg.ToString());
			return ufmt switch {
				"C" or "N" or "M" or "R" when emptyArg => string.Empty,
				"C" => string.Format(Context, arg),
				"N" => string.Format(Notice, arg),
				"M" => string.Format(Message, arg),
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
