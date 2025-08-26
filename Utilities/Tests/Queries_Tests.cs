using System.Collections.Generic;
using FrootLuips.Subnautica.Helpers;

namespace FrootLuips.Subnautica.Tests;

internal class Queries_Tests : ITestContainer
{
	const string _TEST_GROUP = nameof(Queries);

	public IEnumerator<TestResult> GetResults()
	{
		yield return TestResult.Run(_TEST_GROUP, nameof(FilterArray), FilterArray);
		yield return TestResult.Run(_TEST_GROUP, nameof(FilterList), FilterList);
		yield return TestResult.Run(_TEST_GROUP, nameof(ConvertList), ConvertList);
	}

	void FilterArray()
	{
		int[] actual = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
		int[] expected = new[] { 6, 7, 8, 9, 10 };

		static bool greaterThan5(int value) => value > 5;

		Queries.Filter(ref actual, greaterThan5);
		Assert.Equals<int>(expected, actual);
	}

	void FilterList()
	{
		List<int> actual = new() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
		List<int> expected = new() { 6, 7, 8, 9, 10 };

		static bool greaterThan5(int value) => value > 5;

		Queries.Filter(actual, greaterThan5);
		Assert.Equals<int>(expected, actual);
	}

	void ConvertList()
	{
		int[] actual = new[] { -2, -1, 0, 1, 2 };
		List<bool> destination = new(capacity: 5);
		List<bool> expected = new() { false, false, false, true, true };

		static bool toBool(int value) => value > 0;

		Queries.Convert(actual, destination, toBool);
		Assert.Equals<bool>(expected, destination);
	}
}
