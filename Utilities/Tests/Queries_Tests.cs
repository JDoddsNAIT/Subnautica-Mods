using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace FrootLuips.Subnautica.Tests;

internal class Queries_Tests : ITestContainer
{
	public IEnumerator<TestResult> GetResults()
	{
		yield return TestResult.Assert(nameof(FilterArray), FilterArray);
	}

	private bool FilterArray(out string message)
	{
		int[] ints = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
		int[] expected = new[] { 6, 7, 8, 9, 10 };

		static bool greaterThan5(int value) => value > 5;

		Queries.Filter(ints, greaterThan5);
		message = string.Join(", ", ints);

		return ints.CompareValues(expected);
	}
}
