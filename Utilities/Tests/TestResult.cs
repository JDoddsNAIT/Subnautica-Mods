using System;
using System.Collections.Generic;
using FrootLuips.Subnautica.Logging;

namespace FrootLuips.Subnautica.Tests;

internal delegate bool Test(out string message);

internal interface ITestContainer
{
	IEnumerator<TestResult> GetResults();
}

internal readonly record struct TestResult(string Name, bool Passed, string Message = "")
{
	public static TestResult Assert(string name, Test test)
	{
		string message;
		bool passed;

		try
		{
			passed = test(out message);
		}
		catch (Exception ex)
		{
			message = ex.Message;
			passed = false;
		}

		return new TestResult(name, passed, message);
	}

	public override string ToString()
	{
		var provider = new LogMessage.FormatProvider(notice: "{0}:");
		var message = new LogMessage().WithNotice("Test ", Name);
		if (Passed)
		{
			message.WithMessage("Passed");
		}
		else
		{
			message.WithMessage("Failed");
		}
		message.WithRemarks(Message);
		return message.ToString(provider);
	}

	public static implicit operator bool(TestResult result) => result.Passed;
}
