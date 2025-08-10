using System.Collections.Generic;

namespace FrootLuips.Subnautica.Tests;

internal class Queries_Tests : ITestContainer
{
	public IEnumerator<TestResult> GetResults()
	{
		var group = nameof(Queries_Tests);
		yield return TestResult.Assert(nameof(FilterArray), FilterArray, group);
		yield return TestResult.Assert(nameof(FilterList), FilterList, group);
		yield return TestResult.Assert(nameof(ConvertList), ConvertList, group);
	}

	private bool FilterArray(out string message)
	{
		int[] ints = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
		int[] expected = new[] { 6, 7, 8, 9, 10 };

		static bool greaterThan5(int value) => value > 5;

		Queries.Filter(ref ints, greaterThan5);
		message = string.Join(", ", ints);

		return ints.CompareValues(expected);
	}

	private bool FilterList(out string message)
	{
		List<int> ints = new() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
		List<int> expected = new() { 6, 7, 8, 9, 10 };

		static bool greaterThan5(int value) => value > 5;

		Queries.Filter(ints, greaterThan5);
		message = string.Join(", ", ints);

		return ints.CompareValues(expected);
	}

	private bool ConvertList(out string message)
	{
		int[] ints = new[] { -2, -1, 0, 1, 2 };
		List<bool> destination = new(capacity: 5);
		List<bool> expected = new() { false, false, false, true, true };

		static bool toBool(int value) => value > 0;

		Queries.Convert(ints, toBool, destination);

		TestResult.GetResult(out message, string.Join(", ", destination), string.Join(", ", expected));
		return destination.CompareValues(expected);
	}
}
