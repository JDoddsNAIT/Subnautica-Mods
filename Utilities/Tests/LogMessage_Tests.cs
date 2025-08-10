using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrootLuips.Subnautica.Logging;

namespace FrootLuips.Subnautica.Tests;
internal class LogMessage_Tests : ITestContainer
{
	public IEnumerator<TestResult> GetResults()
	{
		yield return TestResult.Assert(nameof(ToString), ToString);
		yield return TestResult.Assert(nameof(NoContext), NoContext);
		yield return TestResult.Assert(nameof(NoNotice), NoNotice);
		yield return TestResult.Assert(nameof(NoMessage), NoMessage);
		yield return TestResult.Assert(nameof(NoRemarks), NoRemarks);
	}

	private bool ToString(out string message)
	{
		var logMessage = new LogMessage()
			.WithContext("Context")
			.WithNotice("Notice")
			.WithMessage("Message")
			.WithRemarks("Remarks");
		string actual = logMessage.ToString();
		string expected = "[Context] Notice - Message (Remarks)";
		return GetResult(out message, expected, actual);
	}

	private bool NoContext(out string message)
	{
		var logMessage = new LogMessage()
			.WithNotice("Notice")
			.WithMessage("Message")
			.WithRemarks("Remarks");
		string actual = logMessage.ToString();
		string expected = "Notice - Message (Remarks)";
		return GetResult(out message, expected, actual);
	}

	private bool NoNotice(out string message)
	{
		var logMessage = new LogMessage()
			.WithContext("Context")
			.WithMessage("Message")
			.WithRemarks("Remarks");
		string expected = "[Context] Message (Remarks)";
		string actual = logMessage.ToString();
		return GetResult(out message, expected, actual);
	}

	private bool NoMessage(out string message)
	{
		var logMessage = new LogMessage()
			.WithContext("Context")
			.WithNotice("Notice")
			.WithRemarks("Remarks");
		string actual = logMessage.ToString();
		string expected = "[Context] Notice - (Remarks)";
		return GetResult(out message, expected, actual);
	}

	private bool NoRemarks(out string message)
	{
		var logMessage = new LogMessage()
			.WithContext("Context")
			.WithNotice("Notice")
			.WithMessage("Message");
		string actual = logMessage.ToString();
		string expected = "[Context] Notice - Message";
		return GetResult(out message, expected, actual);
	}

	private static bool GetResult(out string message, string expected, string actual)
	{
		message = $"Result is '{actual}', expected '{expected}'.";
		return actual == expected;
	}
}
