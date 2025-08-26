using System.Collections.Generic;
using FrootLuips.Subnautica.Extensions;

namespace FrootLuips.Subnautica.Tests;
internal class StringExtensions_Tests : ITestContainer
{
	const string _TEST_GROUP = nameof(StringExtensions);

	public IEnumerator<TestResult> GetResults()
	{
		yield return TrimAll();
		yield return FromStartTo_Inclusive();
		yield return FromStartTo_Exclusive();
		yield return ToEndFrom_Inclusive();
		yield return ToEndFrom_Exclusive();
	}

	private TestResult TrimAll()
	{
		string value = " This  is a    line  of text     ";
		string expected = "This is a line of text";

		value = value.TrimAll();

		return new TestResult.Context(_TEST_GROUP, nameof(TrimAll)).AssertEquals(expected, value);
	}

	private TestResult FromStartTo_Inclusive()
	{
		string value = "This is a line of text";
		string expected = "This is a line";

		value = value.FromStartTo("line", inclusive: true);

		return new TestResult.Context(_TEST_GROUP, nameof(FromStartTo_Inclusive)).AssertEquals(expected, value);
	}

	private TestResult FromStartTo_Exclusive()
	{
		string value = "This is a line of text";
		string expected = "This is a ";

		value = value.FromStartTo("line", inclusive: false);

		return new TestResult.Context(_TEST_GROUP, nameof(FromStartTo_Exclusive)).AssertEquals(expected, value);
	}

	private TestResult ToEndFrom_Inclusive()
	{
		string value = "This is a line of text";
		string expected = "line of text";

		value = value.ToEndFrom("line", inclusive: true);

		return new TestResult.Context(_TEST_GROUP, nameof(ToEndFrom_Inclusive)).AssertEquals(expected, value);
	}

	private TestResult ToEndFrom_Exclusive()
	{
		string value = "This is a line of text";
		string expected = " of text";
		value = value.ToEndFrom("line", inclusive: false);
		return new TestResult.Context(_TEST_GROUP, nameof(ToEndFrom_Exclusive)).AssertEquals(expected, value);
	}
}
