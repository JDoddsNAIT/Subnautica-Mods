using System.Text;
using BepInEx.Logging;

namespace FrootLuips.Subnautica.Logging;
/// <summary>
/// Helper class for generating log messages with a standardized format.
/// </summary>
/// <remarks>
/// Use the <see cref="ToString(FormatProvider)"/> method to customize the message format.
/// </remarks>
public partial class LogMessage
{
	public const string FORMAT = "{0:C} {1:N} {2:M} {3:R}";

	private readonly StringBuilder _context, _notice, _message, _remarks;
	private readonly object[] _formatArgs;

	public string Context => _context.ToString();
	public string Notice => _notice.ToString();
	public string Message => _message.ToString();
	public string Remarks => _remarks.ToString();

	public LogMessage()
	{
		_context = new StringBuilder();
		_notice = new StringBuilder();
		_message = new StringBuilder();
		_remarks = new StringBuilder();
		_formatArgs = new[] { _context, _notice, _message, _remarks };
	}

	#region Builder Methods
	public LogMessage AddContext(params object[] context)
	{
		AppendValues(_context, context);
		return this;
	}
	public LogMessage WithContext(params object[] context)
	{
		_context.Clear();
		return AddContext(context);
	}

	public LogMessage AddNotice(params object[] notice)
	{
		AppendValues(_notice, notice);
		return this;
	}
	public LogMessage WithNotice(params object[] notice)
	{
		_notice.Clear();
		return AddNotice(notice);
	}

	public LogMessage AddMessage(params object[] message)
	{
		AppendValues(_message, message);
		return this;
	}
	public LogMessage WithMessage(params object[] message)
	{
		_message.Clear();
		return AddMessage(message);
	}

	public LogMessage AddRemarks(params object[] remarks)
	{
		AppendValues(_remarks, remarks);
		return this;
	}
	public LogMessage WithRemarks(params object[] remarks)
	{
		_remarks.Clear();
		return AddRemarks(remarks);
	}
	#endregion

	public void Log(ILogger logger, LogLevel level, bool showInGame = false) => logger.Log(this, level, showInGame);

	public override string ToString()
	{
		return this.ToString(FormatProvider.Default);
	}

	public string ToString(FormatProvider provider)
	{
		return string.Format(provider, FORMAT, this.GetFormatArgs()).TrimAll();
	}

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
