using System.Collections.Generic;
using FrootLuips.Subnautica.Logging;
using static FrootLuips.Subnautica.Tests.TestResult;

namespace FrootLuips.Subnautica.Tests;
internal class LogMessage_Tests : ITestContainer
{
	const string _TEST_GROUP = nameof(LogMessage);

	public IEnumerator<TestResult> GetResults()
	{
		yield return AllFields();
		yield return NoContext();
		yield return NoMessage();
		yield return NoNotice();
		yield return NoRemarks();
	}

	private TestResult AllFields()
	{
		var logMessage = new LogMessage()
			.WithContext("Context")
			.WithMessage("Message")
			.WithNotice("Notice")
			.WithRemarks("Remarks");
		string actual = logMessage.ToString();
		string expected = "[Context] Message - Notice (Remarks)";
		return new Context(_TEST_GROUP, nameof(AllFields)).AssertEquals(expected, actual);
	}

	private TestResult NoContext()
	{
		var logMessage = new LogMessage()
			.WithMessage("Message")
			.WithNotice("Notice")
			.WithRemarks("Remarks");
		string actual = logMessage.ToString();
		string expected = "Message - Notice (Remarks)";
		return new Context(_TEST_GROUP, nameof(NoContext)).AssertEquals(expected, actual);
	}

	private TestResult NoMessage()
	{
		var logMessage = new LogMessage()
			.WithContext("Context")
			.WithNotice("Notice")
			.WithRemarks("Remarks");
		string expected = "[Context] Notice (Remarks)";
		string actual = logMessage.ToString();
		return new Context(_TEST_GROUP, nameof(NoMessage)).AssertEquals(expected, actual);
	}

	private TestResult NoNotice()
	{
		var logMessage = new LogMessage()
			.WithContext("Context")
			.WithMessage("Message")
			.WithRemarks("Remarks");
		string actual = logMessage.ToString();
		string expected = "[Context] Message - (Remarks)";
		return new Context(_TEST_GROUP, nameof(NoNotice)).AssertEquals(expected, actual);
	}

	private TestResult NoRemarks()
	{
		var logMessage = new LogMessage()
			.WithContext("Context")
			.WithMessage("Message")
			.WithNotice("Notice");
		string actual = logMessage.ToString();
		string expected = "[Context] Message - Notice";
		return new Context(_TEST_GROUP, nameof(NoRemarks)).AssertEquals(expected, actual);
	}
}
