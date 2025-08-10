using System.Collections.Generic;

namespace FrootLuips.Subnautica.Tests;
internal class StringExtensions_Tests : ITestContainer
{
	public IEnumerator<TestResult> GetResults()
	{
		string group = nameof(StringExtensions_Tests);
		yield return TestResult.Assert(nameof(TrimAll), TrimAll, group);
		yield return TestResult.Assert(nameof(FromStartTo_Inclusive), FromStartTo_Inclusive, group);
		yield return TestResult.Assert(nameof(FromStartTo_Exclusive), FromStartTo_Exclusive, group);
		yield return TestResult.Assert(nameof(ToEndFrom_Inclusive), ToEndFrom_Inclusive, group);
		yield return TestResult.Assert(nameof(ToEndFrom_Exclusive), ToEndFrom_Exclusive, group);
	}

	private bool TrimAll(out string message)
	{
		string value = " This  is a    line  of text     ";
		string expected = "This is a line of text";

		value = value.TrimAll();

		return TestResult.GetResult(out message, value, expected);
	}

	private bool FromStartTo_Inclusive(out string message)
	{
		string value = "This is a line of text";
		string expected = "This is a line";
		value = value.FromStartTo("line", inclusive: true);
		return TestResult.GetResult(out message, value, expected);
	}

	private bool FromStartTo_Exclusive(out string message)
	{
		string value = "This is a line of text";
		string expected = "This is a ";
		value = value.FromStartTo("line", inclusive: false);
		return TestResult.GetResult(out message, value, expected);
	}

	private bool ToEndFrom_Inclusive(out string message)
	{
		string value = "This is a line of text";
		string expected = "line of text";
		value = value.ToEndFrom("line", inclusive: true);
		return TestResult.GetResult(out message, value, expected);
	}

	private bool ToEndFrom_Exclusive(out string message)
	{
		string value = "This is a line of text";
		string expected = " of text";
		value = value.ToEndFrom("line", inclusive: false);
		return TestResult.GetResult(out message, value, expected);
	}
}
