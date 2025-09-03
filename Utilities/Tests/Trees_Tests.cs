using System.Collections.Generic;
using System.Linq;
using FrootLuips.Subnautica.Helpers;
using FrootLuips.Subnautica.Trees;
using UnityEngine;

namespace FrootLuips.Subnautica.Tests;
internal class Trees_Tests : ITestContainer
{
	private static readonly UnityObjectComparer<GameObject> _objectComparer = new();

	private Tree<Transform>? _tree;

#pragma warning disable IDE1006 // Naming Styles
	GameObject? A, B, C, D, E, F;
#pragma warning restore IDE1006 // Naming Styles

	private Tree<Transform> CreateTestHeirarchy()
	{
		A = new(name: "A");
		B = new(name: "B");
		C = new(name: "C");
		D = new(name: "D");
		E = new(name: "E");
		F = new(name: "F");
		

		B.transform.SetParent(A.transform);
		C.transform.SetParent(A.transform);

		D.transform.SetParent(B.transform);
		E.transform.SetParent(B.transform);

		F.transform.SetParent(C.transform);

		//     A
		//    / \
		//   B   C
		//  / \   \
		// D   E   F

		return new Tree<Transform>(A.transform, Trees.Handlers.TransformHandler.Main);
	}

	const string _TEST_GROUP = nameof(Tree<Transform>);

	public IEnumerator<TestResult> GetResults()
	{
		_tree = CreateTestHeirarchy();

		yield return TestResult.Run(_TEST_GROUP, nameof(Enumerate_BFS), Enumerate_BFS);
		yield return TestResult.Run(_TEST_GROUP, nameof(Enumerate_DFS), Enumerate_DFS);
		yield return TestResult.Run(_TEST_GROUP, nameof(Enumerate_MaxDepth), Enumerate_MaxDepth);
		yield return TestResult.Run(_TEST_GROUP, nameof(Enumerate_NotInclusive), Enumerate_NotInclusive);
		yield return TestResult.Run(_TEST_GROUP, nameof(Enumerate_Predicate), Enumerate_Predicate);

		yield return TestResult.Run(_TEST_GROUP, nameof(Find_Predicate), Find_Predicate);
		yield return TestResult.Run(_TEST_GROUP, nameof(Find_Name), Find_Name);
		yield return TestResult.Run(_TEST_GROUP, nameof(Find_Path), Find_Path);
		yield return TestResult.Run(_TEST_GROUP, nameof(GetPath), GetPath);
	}

	void Enumerate_BFS()
	{
		var expected = new[] { A!, B!, C!, D!, E!, F! };

		var actual = _tree!.Enumerate(SearchMode.BreadthFirst)
			.Select(static t => t.gameObject).ToArray();

		Assert.Equals(expected, actual, _objectComparer);
	}

	void Enumerate_DFS()
	{
		var expected = new[] { A!, B!, D!, E!, C!, F! };

		var actual = _tree!.Enumerate(SearchMode.DepthFirst)
			.Select(static t => t.gameObject).ToArray();

		Assert.Equals(expected, actual, _objectComparer);
	}

	void Enumerate_MaxDepth()
	{
		var expected = new[] { A!, B!, C! };
		var actual = _tree!.Enumerate(options: new(SearchMode.BreadthFirst, maxDepth: 1))
			.Select(static t => t.gameObject).ToArray();

		Assert.Equals(expected, actual, _objectComparer);
	}

	void Enumerate_NotInclusive()
	{
		var expected = new[] { B!, D!, E!, C!, F! };
		var actual = _tree!.Enumerate(options: new(SearchMode.DepthFirst, inclusive: false))
			.Select(static t => t.gameObject).ToArray();

		Assert.Equals(expected, actual, _objectComparer);
	}

	private static bool Predicate(Transform n) => n.gameObject.name[0] % 2 == 0;

	void Enumerate_Predicate()
	{
		var expected = new[] { A!, B!, D! };
		var actual = _tree!.Enumerate(options: new(SearchMode.BreadthFirst, predicate: Predicate))
			.Select(static t => t.gameObject).ToArray();

		Assert.Equals(expected, actual, _objectComparer);
	}

	void Find_Predicate()
	{
		var expected = B!;
		var actual = _tree!.Find(Predicate, SearchMode.BreadthFirst).gameObject;

		Assert.Equals(expected, actual, _objectComparer);
	}

	void Find_Name()
	{
		var expected = C;
		var actual = _tree!.Find("C", SearchMode.DepthFirst).gameObject;

		Assert.Equals(expected, actual, _objectComparer);
	}

	void Find_Path()
	{
		var expected = E!;
		string path = "B/E";
		var actual = _tree!.GetNode(path).gameObject;

		Assert.Equals(expected, actual, _objectComparer);
	}

	void GetPath()
	{
		string expected = "A/B/E";
		var node = _tree!.Find("E", SearchMode.DepthFirst);
		string actual = TreeHelpers.GetPath(node, _tree.Handler);

		Assert.Equals(expected, actual);
	}
}
