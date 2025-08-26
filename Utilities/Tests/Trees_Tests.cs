using System.Collections.Generic;
using System.Linq;
using FrootLuips.Subnautica.Helpers;
using FrootLuips.Subnautica.Trees;
using UnityEngine;

namespace FrootLuips.Subnautica.Tests;
internal class Trees_Tests : ITestContainer
{
	private static readonly UnityObjectComparer<GameObject> _objectComparer = new();
	private static readonly ListComparer<GameObject> _listComparer = new(_objectComparer);

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

		yield return Enumerate_BFS();
		yield return Enumerate_DFS();
		yield return Enumerate_MaxDepth();
		yield return Enumerate_NotInclusive();
		yield return Enumerate_Predicate();

		yield return Find_Predicate();
		yield return Find_Name();
		yield return Find_Path();
		yield return GetPath();
	}

	private TestResult Enumerate_BFS()
	{
		var expected = new[] { A!, B!, C!, D!, E!, F! };

		var actual = _tree!.Enumerate(SearchMode.BreadthFirst)
			.Select(static t => t.gameObject).ToArray();

		return new TestResult.Context(_TEST_GROUP, nameof(Enumerate_BFS))
			.AssertEquals(expected, actual, _listComparer);
	}

	private TestResult Enumerate_DFS()
	{
		var expected = new[] { A!, B!, D!, E!, C!, F! };

		var actual = _tree!.Enumerate(SearchMode.DepthFirst)
			.Select(static t => t.gameObject).ToArray();

		return new TestResult.Context(_TEST_GROUP, nameof(Enumerate_DFS))
			.AssertEquals(expected, actual, _listComparer);
	}

	private TestResult Enumerate_MaxDepth()
	{
		var expected = new[] { A!, B!, C! };
		var actual = _tree!.Enumerate(options: new(SearchMode.BreadthFirst, MaxDepth: 1))
			.Select(static t => t.gameObject).ToArray();

		return new TestResult.Context(_TEST_GROUP, nameof(Enumerate_MaxDepth))
			.AssertEquals(expected, actual, _listComparer);
	}

	private TestResult Enumerate_NotInclusive()
	{
		var expected = new[] { B!, D!, E!, C!, F! };
		var actual = _tree!.Enumerate(options: new(SearchMode.DepthFirst, Inclusive: false))
			.Select(static t => t.gameObject).ToArray();

		return new TestResult.Context(_TEST_GROUP, nameof(Enumerate_NotInclusive))
			.AssertEquals(expected, actual, _listComparer);
	}

	private static bool Predicate(Transform n) => n.gameObject.name[0] % 2 == 0;

	private TestResult Enumerate_Predicate()
	{
		var expected = new[] { A!, B!, D! };
		var actual = _tree!.Enumerate(options: new(SearchMode.BreadthFirst, Predicate: Predicate))
			.Select(static t => t.gameObject).ToArray();

		return new TestResult.Context(_TEST_GROUP, nameof(Enumerate_Predicate))
			.AssertEquals(expected, actual, _listComparer);
	}

	private TestResult Find_Predicate()
	{
		var expected = B!;
		var actual = _tree!.Find(Predicate, SearchMode.BreadthFirst).gameObject;

		return new TestResult.Context(_TEST_GROUP, nameof(Enumerate_BFS))
			.AssertEquals(expected, actual, _objectComparer);
	}

	private TestResult Find_Name()
	{
		var expected = C;
		var actual = _tree!.Find(C!.ToString(), SearchMode.DepthFirst).gameObject;

		return new TestResult.Context(_TEST_GROUP, nameof(Find_Name))
			.AssertEquals(expected, actual, _objectComparer);
	}

	private TestResult Find_Path()
	{
		var expected = E!;
		string path = "B/E";
		var actual = _tree!.GetNode(path).gameObject;

		return new TestResult.Context(_TEST_GROUP, nameof(Find_Path))
			.AssertEquals(expected, actual, _objectComparer);
	}

	private TestResult GetPath()
	{
		string expected = "A/B/E";
		var node = _tree!.Find("E", SearchMode.DepthFirst);
		string actual = TreeHelpers.GetPath(node, _tree.Handler);

		return new TestResult.Context(_TEST_GROUP, nameof(GetPath))
			.AssertEquals(expected, actual);
	}
}
