using System;
using System.Collections.Generic;
using System.Text;

namespace FrootLuips.CustomCraft3Remake;
internal sealed class LogMessage
{
	[Flags]
	internal enum Format : byte
	{
		Default = 0b00,
		ExcludeContext = 0b01,
		ExcludeNotice = 0b10,
		MessageOnly = 0b11,
	}

	private readonly StringBuilder _builder;

	private IReadOnlyCollection<object> _context;
	private IReadOnlyCollection<object> _notice;
	private IReadOnlyCollection<object> _message;

	public LogMessage() => _builder = new StringBuilder();

	public LogMessage(string context = null, string notice = null, string message = null) : this()
	{
		if (context is not null)
			this.WithContext(context);
		if (notice is not null)
			this.WithNotice(notice);
		if (message is not null)
			this.WithMessage(message);
	}

	public LogMessage(Exception exception) : this(exception.Message, "See below for details.\n", exception.StackTrace)
	{ }

	public LogMessage(params object[] contents) : this((IReadOnlyCollection<object>)contents)
	{ }

	public LogMessage(IReadOnlyCollection<object> contents) : this() => _message = contents;


	public LogMessage WithContext(params object[] context) => this.WithContext((IReadOnlyCollection<object>)context);

	public LogMessage WithContext(IReadOnlyCollection<object> context)
	{
		_context = context;
		return this;
	}

	public LogMessage WithNotice(params object[] notice) => this.WithNotice((IReadOnlyCollection<object>)notice);

	public LogMessage WithNotice(IReadOnlyCollection<object> notice)
	{
		_notice = notice;
		return this;
	}

	public LogMessage WithMessage(params object[] message) => this.WithMessage((IReadOnlyCollection<object>)message);

	public LogMessage WithMessage(IReadOnlyCollection<object> message)
	{
		_message = message;
		return this;
	}

	public override string ToString()
	{
		return ToString(Format.Default);
	}

	public string ToString(Format format)
	{
		_builder.Clear();

		if (!format.HasFlag(Format.ExcludeContext) && !_context.IsNullOrEmpty())
		{
			_builder.Append('[');
			foreach (object o in _context)
			{
				if (o != null)
					_builder.Append(o);
			}
			_builder.Append("] ");
		}

		if (!format.HasFlag(Format.ExcludeNotice) && !_notice.IsNullOrEmpty())
		{
			foreach (object o in _notice)
			{
				if (o != null)
					_builder.Append(o);
			}
			_builder.Append(" - ");
		}

		foreach (object o in _message)
		{
			if (o is not null)
				_builder.Append(o);
		}

		return _builder.ToString();
	}

	public static implicit operator string(LogMessage message) => message.ToString();
}
