using System;
using System.Collections.Generic;
using FrootLuips.Subnautica.Logging;

namespace FrootLuips.Subnautica.Tests;

internal delegate bool Test(out string message);

internal interface ITestContainer
{
	IEnumerator<TestResult> GetResults();
}

internal readonly record struct TestResult(string Group, string Name, bool Passed, string Message = "")
{
	public static TestResult Assert(string name, Test test, string group = "")
	{
		string message;
		bool passed;

		try
		{
			passed = test(out message);
		}
		catch (Exception ex)
		{
			message = ex.ToString();
			passed = false;
		}

		return new TestResult(group, name, passed, message);
	}

	public static bool GetResult(out string message, string actual, string expected)
	{
		message = $"Result is '{actual}', expected '{expected}'.";
		return actual == expected;
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
