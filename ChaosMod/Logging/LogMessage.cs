using System;
using FrootLuips.ChaosMod.Utilities;

namespace FrootLuips.ChaosMod.Logging;

/// <summary>
/// Helper class for generating log messages with a standardized format.
/// </summary>
/// <remarks>
/// Format is as follows: "[{Context}] {Notice} - {Message}\n{Remarks}"
/// </remarks>
public class LogMessage
{
	private readonly ArrayBuilder _context, _notice, _message, _remarks;

	public bool HasContext => _context.Count > 0;
	public bool HasNotice => _notice.Count > 0;
	public bool HasMessage => _message.Count > 0;
	public bool HasRemarks => _remarks.Count > 0;

	public LogMessage()
	{
		_context = new ArrayBuilder();
		_notice = new ArrayBuilder();
		_message = new ArrayBuilder();
		_remarks = new ArrayBuilder();
	}

	public LogMessage(string? context = null, string? notice = null, string? message = null, string? remarks = null)
		: this()
	{
		if (!string.IsNullOrEmpty(context))
			this.WithContext(context!);

		if (!string.IsNullOrEmpty(notice))
			this.WithNotice(notice!);

		if (!string.IsNullOrEmpty(message))
			this.WithMessage(message!);

		if (!string.IsNullOrEmpty(remarks))
			this.WithRemarks(remarks!);
	}

	public LogMessage WithContext(params object[] context)
	{
		_context.Clear();
		return this.AddContext(context);
	}
	public LogMessage AddContext(params object[] context)
	{
		_context.Append(context);
		return this;
	}

	public LogMessage WithNotice(params object[] notice)
	{
		_notice.Clear();
		return this.AddNotice(notice);
	}
	public LogMessage AddNotice(params object[] notice)
	{
		_notice.Append(notice);
		return this;
	}

	public LogMessage WithMessage(params object[] message)
	{
		_message.Clear();
		return this.AddMessage(message);
	}
	public LogMessage AddMessage(params object[] message)
	{
		_message.Append(message);
		return this;
	}

	public LogMessage WithRemarks(params object[] remarks)
	{
		_remarks.Clear();
		return this.AddRemarks(remarks);
	}
	public LogMessage AddRemarks(params object[] remarks)
	{
		_remarks.Append(remarks);
		return this;
	}

	public override string ToString()
	{
		var sb = new System.Text.StringBuilder();

		if (this.HasContext)
		{
			sb.Append('[');
			AppendValues(sb, _context.ToArray());
			sb.Append(']');

			if (this.HasNotice || this.HasMessage)
				sb.Append(' ');
		}


		if (this.HasNotice)
		{
			AppendValues(sb, _notice);
			if (this.HasMessage)
				sb.Append(" - ");
			else if (this.HasRemarks)
				sb.Append(':');
		}

		if (this.HasMessage)
		{
			AppendValues(sb, _message);
		}

		if (this.HasRemarks)
		{
			if (this.HasNotice || this.HasMessage)
			{
				sb.Append('\n');
			}

			AppendValues(sb, _remarks);
		}

		return sb.ToString().Trim();
	}

	private static void AppendValues(System.Text.StringBuilder stringBuilder, params object[] values)
	{
		for (int i = 0; i < values.Length; i++)
		{
			if (values[i] is not null)
				stringBuilder.Append(values[i]);
		}
	}

	/// <summary>
	/// Creates a new <see cref="LogMessage"/> using an <see cref="Exception"/>.
	/// </summary>
	/// <param name="exception"></param>
	/// <returns>A <see cref="LogMessage"/> with the notice, message, and remarks set.</returns>
	public static LogMessage FromException(Exception exception)
	{
		return new LogMessage(message: exception.Message + "; See below for details.", remarks: exception.ToString());
	}

	public static implicit operator string(LogMessage logMessage) => logMessage.ToString();
}
