using System.Collections.Generic;
using FrootLuips.Subnautica.Extensions;

namespace FrootLuips.Subnautica.Tests;
internal class StringExtensions_Tests : ITestContainer
{
	const string _TEST_GROUP = nameof(StringExtensions);

	public IEnumerator<TestResult> GetResults()
	{
		yield return TestResult.Run(_TEST_GROUP, nameof(TrimAll), TrimAll);
		yield return TestResult.Run(_TEST_GROUP, nameof(FromStartTo_Inclusive), FromStartTo_Inclusive);
		yield return TestResult.Run(_TEST_GROUP, nameof(FromStartTo_Exclusive), FromStartTo_Exclusive);
		yield return TestResult.Run(_TEST_GROUP, nameof(ToEndFrom_Inclusive), ToEndFrom_Inclusive);
		yield return TestResult.Run(_TEST_GROUP, nameof(ToEndFrom_Exclusive), ToEndFrom_Exclusive);
	}

	void TrimAll()
	{
		string value = " This  is a    line  of text     ";
		string expected = "This is a line of text";

		value = value.TrimAll();

		Assert.Equals(expected, value);
	}

	void FromStartTo_Inclusive()
	{
		string value = "This is a line of text";
		string expected = "This is a line";

		value = value.FromStartTo("line", inclusive: true);

		Assert.Equals(expected, value);
	}

	void FromStartTo_Exclusive()
	{
		string value = "This is a line of text";
		string expected = "This is a ";

		value = value.FromStartTo("line", inclusive: false);

		Assert.Equals(expected, value);
	}

	void ToEndFrom_Inclusive()
	{
		string value = "This is a line of text";
		string expected = "line of text";

		value = value.ToEndFrom("line", inclusive: true);

		Assert.Equals(expected, value);
	}

	void ToEndFrom_Exclusive()
	{
		string value = "This is a line of text";
		string expected = " of text";
		value = value.ToEndFrom("line", inclusive: false);
		Assert.Equals(expected, value);
	}
}
