using System;
using System.Collections.Generic;
using FrootLuips.Subnautica.Logging;

namespace FrootLuips.Subnautica.Tests;

internal interface ITestContainer
{
	IEnumerator<TestResult> GetResults();
}

internal readonly partial record struct TestResult(string Group, string Name, bool Passed, string Message = "")
{
	public static TestResult Run(string group, string name, Action test)
	{
		string? message = null;
		bool passed = false;

		try
		{
			test();
		}
		catch (Context context)
		{
			message = context.Message;
			passed = context.Passed;
		}
		catch (Exception ex)
		{
			message = ex.ToString();
			passed = false;
		}
		Validation.Assert.NotNull(message);
		return new TestResult(group, name, passed, message);
	}

	public override string ToString()
	{
		var provider = new LogMessage.FormatProvider(message: "{0}:");
		var message = new LogMessage()
			.WithContext(Group)
			.WithMessage("Test ", Name);
		if (Passed)
		{
			message.WithNotice("Passed");
		}
		else
		{
			message.WithNotice("Failed");
		}
		message.WithRemarks(Message);
		return message.ToString(provider);
	}

	public static implicit operator bool(TestResult result) => result.Passed;
}
