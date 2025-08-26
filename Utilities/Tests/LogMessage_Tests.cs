using System.Collections.Generic;
using FrootLuips.Subnautica.Logging;

namespace FrootLuips.Subnautica.Tests;
internal class LogMessage_Tests : ITestContainer
{
	const string _TEST_GROUP = nameof(LogMessage);

	public IEnumerator<TestResult> GetResults()
	{
		yield return TestResult.Run(_TEST_GROUP, nameof(AllFields), AllFields);
		yield return TestResult.Run(_TEST_GROUP, nameof(NoContext), NoContext);
		yield return TestResult.Run(_TEST_GROUP, nameof(NoMessage), NoMessage);
		yield return TestResult.Run(_TEST_GROUP, nameof(NoNotice), NoNotice);
		yield return TestResult.Run(_TEST_GROUP, nameof(NoRemarks), NoRemarks);
	}

	void AllFields()
	{
		var logMessage = new LogMessage()
			.WithContext("Context")
			.WithMessage("Message")
			.WithNotice("Notice")
			.WithRemarks("Remarks");
		string actual = logMessage.ToString();
		string expected = "[Context] Message - Notice (Remarks)";
		Assert.Equals(actual, expected);
	}

	void NoContext()
	{
		var logMessage = new LogMessage()
			.WithMessage("Message")
			.WithNotice("Notice")
			.WithRemarks("Remarks");
		string actual = logMessage.ToString();
		string expected = "Message - Notice (Remarks)";
		Assert.Equals(expected, actual);
	}

	void NoMessage()
	{
		var logMessage = new LogMessage()
			.WithContext("Context")
			.WithNotice("Notice")
			.WithRemarks("Remarks");
		string expected = "[Context] Notice (Remarks)";
		string actual = logMessage.ToString();
		Assert.Equals(expected, actual);
	}

	void NoNotice()
	{
		var logMessage = new LogMessage()
			.WithContext("Context")
			.WithMessage("Message")
			.WithRemarks("Remarks");
		string actual = logMessage.ToString();
		string expected = "[Context] Message - (Remarks)";
		Assert.Equals(expected, actual);
	}

	void NoRemarks()
	{
		var logMessage = new LogMessage()
			.WithContext("Context")
			.WithMessage("Message")
			.WithNotice("Notice");
		string actual = logMessage.ToString();
		string expected = "[Context] Message - Notice";
		Assert.Equals(expected, actual);
	}
}
