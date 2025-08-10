using System.Collections.Generic;
using FrootLuips.Subnautica.Logging;
using static FrootLuips.Subnautica.Tests.TestResult;

namespace FrootLuips.Subnautica.Tests;
internal class LogMessage_Tests : ITestContainer
{
	public IEnumerator<TestResult> GetResults()
	{
		var group = nameof(LogMessage_Tests);
		yield return Assert(nameof(ToString), ToString, group);
		yield return Assert(nameof(NoContext), NoContext, group);
		yield return Assert(nameof(NoNotice), NoNotice, group);
		yield return Assert(nameof(NoMessage), NoMessage, group);
		yield return Assert(nameof(NoRemarks), NoRemarks, group);
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
		return GetResult(out message, actual, expected);
	}

	private bool NoContext(out string message)
	{
		var logMessage = new LogMessage()
			.WithNotice("Notice")
			.WithMessage("Message")
			.WithRemarks("Remarks");
		string actual = logMessage.ToString();
		string expected = "Notice - Message (Remarks)";
		return GetResult(out message, actual, expected);
	}

	private bool NoNotice(out string message)
	{
		var logMessage = new LogMessage()
			.WithContext("Context")
			.WithMessage("Message")
			.WithRemarks("Remarks");
		string expected = "[Context] Message (Remarks)";
		string actual = logMessage.ToString();
		return GetResult(out message, actual, expected);
	}

	private bool NoMessage(out string message)
	{
		var logMessage = new LogMessage()
			.WithContext("Context")
			.WithNotice("Notice")
			.WithRemarks("Remarks");
		string actual = logMessage.ToString();
		string expected = "[Context] Notice - (Remarks)";
		return GetResult(out message, actual, expected);
	}

	private bool NoRemarks(out string message)
	{
		var logMessage = new LogMessage()
			.WithContext("Context")
			.WithNotice("Notice")
			.WithMessage("Message");
		string actual = logMessage.ToString();
		string expected = "[Context] Notice - Message";
		return GetResult(out message, actual, expected);
	}
}
