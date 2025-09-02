using System;
using System.Text;
using FrootLuips.Subnautica.Extensions;

namespace FrootLuips.Subnautica.Logging;
/// <summary>
/// Helper class for generating log messages with a standardized format.
/// </summary>
public partial class LogMessage : IFormattable
{
	/// <summary>
	/// Format string for a <see cref="LogMessage"/>.
	/// </summary>
	public const string FORMAT = "{0:C} {1:M} {2:N} {3:R}";

	private readonly StringBuilder _context, _message, _notice, _remarks;
	private readonly object[] _formatArgs;

	/// <summary>
	/// The context of the message.
	/// </summary>
	/// <remarks>
	/// Use the <see cref="AddContext(object[])"/> and <see cref="WithContext(object[])"/> methods to modify this value.
	/// </remarks>
	public string Context => _context.ToString();
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// Use the <see cref="AddMessage(object[])"/> and <see cref="WithMessage(object[])"/> methods to modify this value.
	/// </remarks>
	public string Message => _message.ToString();
	/// <summary>
	/// The main contents of the message.
	/// </summary>
	/// <remarks>
	/// Use the <see cref="AddNotice(object[])"/> and <see cref="WithNotice(object[])"/> methods to modify this value.
	/// </remarks>
	public string Notice => _notice.ToString();
	/// <summary>
	/// The recommended action to be taken by the user, or additional information regarding the message.
	/// </summary>
	/// <remarks>
	/// Use the <see cref="AddRemarks(object[])"/> and <see cref="WithRemarks(object[])"/> methods to modify this value.
	/// </remarks>
	public string Remarks => _remarks.ToString();

	/// <summary>
	/// Creates a new <see cref="LogMessage"/>.
	/// </summary>
	public LogMessage()
	{
		_context = new StringBuilder();
		_message = new StringBuilder();
		_notice = new StringBuilder();
		_remarks = new StringBuilder();
		_formatArgs = new[] { _context, _message, _notice, _remarks };
	}

	#region Builder Methods
	/// <summary/>
	/// <param name="context"></param>
	/// <returns></returns>
	public LogMessage AddContext(params object[] context)
	{
		AppendValues(_context, context);
		return this;
	}
	/// <summary/>
	/// <param name="context"></param>
	/// <returns></returns>
	public LogMessage WithContext(params object[] context)
	{
		_context.Clear();
		return AddContext(context);
	}

	/// <summary/>
	/// <param name="message"></param>
	/// <returns></returns>
	public LogMessage AddMessage(params object[] message)
	{
		AppendValues(_message, message);
		return this;
	}
	/// <summary/>
	/// <param name="message"></param>
	/// <returns></returns>
	public LogMessage WithMessage(params object[] message)
	{
		_message.Clear();
		return AddMessage(message);
	}

	/// <summary/>
	/// <param name="notice"></param>
	/// <returns></returns>
	public LogMessage AddNotice(params object[] notice)
	{
		AppendValues(_notice, notice);
		return this;
	}
	/// <summary/>
	/// <param name="notice"></param>
	/// <returns></returns>
	public LogMessage WithNotice(params object[] notice)
	{
		_notice.Clear();
		return AddNotice(notice);
	}

	/// <summary/>
	/// <param name="remarks"></param>
	/// <returns></returns>
	public LogMessage AddRemarks(params object[] remarks)
	{
		AppendValues(_remarks, remarks);
		return this;
	}
	/// <summary/>
	/// <param name="remarks"></param>
	/// <returns></returns>
	public LogMessage WithRemarks(params object[] remarks)
	{
		_remarks.Clear();
		return AddRemarks(remarks);
	}
	#endregion

	/// <inheritdoc/>
	public override string ToString()
	{
		return this.ToString(FORMAT, FormatProvider.Default);
	}

	/// <inheritdoc cref="ToString(string, IFormatProvider)"/>
	public string ToString(IFormatProvider provider)
	{
		return string.Format(provider, FORMAT, this.GetFormatArgs()).TrimAll();
	}

	/// <inheritdoc/>
	public string ToString(string format, IFormatProvider provider)
	{
		return string.Format(provider, format ?? FORMAT, this.GetFormatArgs()).TrimAll();
	}

	/// <inheritdoc cref="ToString()"/>
	public static implicit operator string(LogMessage message) => message.ToString();

	private object[] GetFormatArgs() => _formatArgs;

	private static void AppendValues(StringBuilder stringBuilder, object[] values)
	{
		for (int i = 0; i < values.Length; i++)
		{
			if (values[i] is null)
				continue;
			if (values[i] is string str && str.Length == 0)
				continue;

			stringBuilder.Append(values[i]);
		}
	}
}
