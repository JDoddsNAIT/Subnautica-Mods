using System.Collections.Generic;
using System.Linq;
using FrootLuips.Subnautica.Extensions;
using FrootLuips.Subnautica.Trees;
using UnityEngine;

namespace FrootLuips.Subnautica.Tests;
internal class Trees_Tests : ITestContainer
{
	private static IReadOnlyDictionary<char, GameObject>? _objects;
	private Tree<Transform>? _tree;
#pragma warning disable IDE1006 // Naming Styles
	const char A = 'A', B = 'B', C = 'C', D = 'D', E = 'E', F = 'F';
#pragma warning restore IDE1006 // Naming Styles

	private static Tree<Transform> CreateTestHeirarchy()
	{
		_objects ??= new Dictionary<char, GameObject>() {
			[A] = new(name: "A"),
			[B] = new(name: "B"),
			[C] = new(name: "C"),
			[D] = new(name: "D"),
			[E] = new(name: "E"),
			[F] = new(name: "F")
		};

		_objects[B].transform.SetParent(_objects[A].transform);
		_objects[C].transform.SetParent(_objects[A].transform);

		_objects[D].transform.SetParent(_objects[B].transform);
		_objects[E].transform.SetParent(_objects[B].transform);

		_objects[F].transform.SetParent(_objects[C].transform);

		//     A
		//    / \
		//   B   C
		//  / \   \
		// D   E   F

		return new Tree<Transform>(_objects[A].transform, Trees.Handlers.TransformHandler.Main);
	}

	public IEnumerator<TestResult> GetResults()
	{
		string group = nameof(Trees_Tests);
		_tree = CreateTestHeirarchy();

		yield return TestResult.Assert(nameof(Enumerate_BFS), Enumerate_BFS, group);
		yield return TestResult.Assert(nameof(Enumerate_DFS), Enumerate_DFS, group);
		yield return TestResult.Assert(nameof(Enumerate_MaxDepth), Enumerate_MaxDepth, group);
		yield return TestResult.Assert(nameof(Enumerate_NotInclusive), Enumerate_NotInclusive, group);
		yield return TestResult.Assert(nameof(Enumerate_Predicate), Enumerate_Predicate, group);

		yield return TestResult.Assert(nameof(Find_Predicate), Enumerate_Predicate, group);
		yield return TestResult.Assert(nameof(Find_Name), Find_Name, group);
		yield return TestResult.Assert(nameof(Find_Path), Find_Path, group);
		yield return TestResult.Assert(nameof(GetPath), GetPath, group);
	}

	private bool Enumerate_BFS(out string message)
	{
		var expected = new[] { "A", "B", "C", "D", "E", "F" };

		var actual = _tree!.Enumerate(SearchMode.BreadthFirst)
			.Select(_tree.Handler.GetName)
			.ToArray();

		TestResult.GetResult(out message, string.Join(", ", actual), string.Join(", ", expected));
		return actual.CompareValues(expected);
	}

	private bool Enumerate_DFS(out string message)
	{
		var expected = new[] { "A", "B", "D", "E", "C", "F" };

		var actual = _tree!.Enumerate(SearchMode.DepthFirst)
			.Select(_tree.Handler.GetName)
			.ToArray();

		TestResult.GetResult(out message, string.Join(", ", actual), string.Join(", ", expected));
		return actual.CompareValues(expected);
	}

	private bool Enumerate_MaxDepth(out string message)
	{
		var expected = new[] { "A", "B", "C" };
		var actual = _tree!.Enumerate(options: new(SearchMode.BreadthFirst, MaxDepth: 1))
			.Select(_tree.Handler.GetName)
			.ToArray();

		TestResult.GetResult(out message, string.Join(", ", actual), string.Join(", ", expected));
		return actual.CompareValues(expected);
	}

	private bool Enumerate_NotInclusive(out string message)
	{
		var expected = new[] { "B", "D", "E", "C", "F" };
		var actual = _tree!.Enumerate(options: new(SearchMode.DepthFirst, Inclusive: false))
			.Select(_tree.Handler.GetName)
			.ToArray();

		TestResult.GetResult(out message, string.Join(", ", actual), string.Join(", ", expected));
		return actual.CompareValues(expected);
	}

	private static bool Predicate(Transform n) => n.gameObject.name[0] % 2 == 0;

	private bool Enumerate_Predicate(out string message)
	{
		var expected = new[] { "A", "B", "D" };
		var actual = _tree!.Enumerate(options: new(SearchMode.BreadthFirst, Predicate: Predicate))
			.Select(_tree.Handler.GetName)
			.ToArray();

		TestResult.GetResult(out message, string.Join(", ", actual), string.Join(", ", expected));
		return actual.CompareValues(expected);
	}

	private bool Find_Predicate(out string message)
	{
		var expected = _objects![B].transform;
		var actual = _tree!.Find(Predicate, SearchMode.BreadthFirst);

		return TestResult.GetResult(out message, actual.ToString(), expected.ToString());
	}

	private bool Find_Name(out string message)
	{
		var expected = _objects![C].transform;
		var actual = _tree!.Find(C.ToString(), SearchMode.DepthFirst);

		return TestResult.GetResult(out message, actual.ToString(), expected.ToString());
	}

	private bool Find_Path(out string message)
	{
		var expected = _objects![E].transform; // E
		string path = "B/E";
		var actual = _tree!.GetNode(path);

		TestResult.GetResult(out message, actual.ToString(), expected.ToString());
		return actual == expected;
	}

	private bool GetPath(out string message)
	{
		string expected = "A/B/E";
		var node = _tree!.Find("E", SearchMode.DepthFirst);
		string actual = TreeHelpers.GetPath(node, _tree.Handler);

		return TestResult.GetResult(out message, actual, expected);
	}
}
