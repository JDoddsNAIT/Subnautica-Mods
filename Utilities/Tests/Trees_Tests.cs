using System.Collections.Generic;
using FrootLuips.Subnautica.Trees;

namespace FrootLuips.Subnautica.Tests;
internal class Trees_Tests : ITestContainer
{
	public IEnumerator<TestResult> GetResults()
	{
		string group = nameof(Trees_Tests);
		yield return TestResult.Assert(nameof(Enumerate_BFS), Enumerate_BFS, group);
	}

	private bool Enumerate_BFS(out string message)
	{
		message = "";
		return false;
	}
}
