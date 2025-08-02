using System.Text;
using BepInEx.Logging;

namespace FrootLuips.RPGElements.Utilities;
internal class LogMessage
{
	private readonly ArrayBuilder<object> _context, _notice, _message;

	public LogMessage()
	{
		_context = new ArrayBuilder<object>();
		_notice = new ArrayBuilder<object>();
		_message = new ArrayBuilder<object>();
	}

	public LogMessage(string? context = null, string? notice = null, string? message = null) : this()
	{
		if (!string.IsNullOrEmpty(context))
			_context.Append(context!);

		if (!string.IsNullOrEmpty(notice))
			_notice.Append(notice!);

		if (!string.IsNullOrEmpty(message))
			_message.Append(message!);
	}

	public LogMessage AppendContext(params object[] context)
	{
		_context.Append(context);
		return this;
	}
	public LogMessage WithContext(params object[] context)
	{
		_context.Clear();
		return this.AppendContext(context);
	}

	public LogMessage AppendNotice(params object[] notice)
	{
		_notice.Append(notice);
		return this;
	}
	public LogMessage WithNotice(params object[] notice)
	{
		_notice.Clear();
		return this.AppendNotice(notice);
	}

	public LogMessage AppendMessage(params object[] message)
	{
		_message.Append(message);
		return this;
	}
	public LogMessage WithMessage(params object[] message)
	{
		_message.Clear();
		return this.AppendMessage(message);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new();
		if (_context.Count > 0)
		{
			stringBuilder.Append('[').AppendValues(_context.ToArray()).Append(']').Append(' ');
		}

		if (_notice.Count > 0)
		{
			stringBuilder.AppendValues(_notice.ToArray());

			if (_message.Count > 0)
				stringBuilder.Append(" - ");
		}

		if (_message.Count > 0)
		{
			stringBuilder.AppendValues(_message.ToArray());
		}

		return stringBuilder.ToString().Trim();
	}

	public void Log(ManualLogSource logger, LogLevel severity = LogLevel.Info) => logger.Log(severity, this);

	public static LogMessage FromException(System.Exception exception)
	{
		return new LogMessage(notice: exception.Message)
			.AppendMessage("See below for details.\n", exception.ToString());
	}
}

public static class LogMessageHelpers
{
	public static StringBuilder AppendValues(this StringBuilder sb, params object[] values)
	{
		for (int i = 0; i < values.Length; i++)
		{
			sb.Append(values[i]);
		}
		return sb;
	}
}
