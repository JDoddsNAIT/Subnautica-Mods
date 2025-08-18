using System.Collections.Generic;
using System.Linq;
using FrootLuips.Subnautica.Extensions;
using FrootLuips.Subnautica.Trees;
using UnityEngine;

namespace FrootLuips.Subnautica.Tests;
internal class Trees_Tests : ITestContainer
{
	private static GameObject[]? _objects;
	private Tree<Transform>? _tree;

	private static Tree<Transform> CreateTestHeirarchy()
	{
		_objects ??= new GameObject[] {
			new(name: "A"),
			new(name: "B"),
			new(name: "C"),
			new(name: "D"),
			new(name: "E"),
			new(name: "F")
		};

		_objects[1].transform.SetParent(_objects[0].transform);
		_objects[2].transform.SetParent(_objects[0].transform);

		_objects[3].transform.SetParent(_objects[1].transform);
		_objects[4].transform.SetParent(_objects[1].transform);

		_objects[5].transform.SetParent(_objects[2].transform);

		//     A
		//    / \
		//   B   C
		//  / \   \
		// D   E   F

		return Trees.Handlers.TransformHandler.CreateTree(_objects[0].transform);
	}

	public IEnumerator<TestResult> GetResults()
	{
		string group = nameof(Trees_Tests);
		_tree = CreateTestHeirarchy();
		yield return TestResult.Assert(nameof(Enumerate_BFS), Enumerate_BFS, group);
		yield return TestResult.Assert(nameof(Enumerate_DFS), Enumerate_DFS, group);
		yield return TestResult.Assert(nameof(FindNodeAtPath), FindNodeAtPath, group);
		yield return TestResult.Assert(nameof(GetDepth), GetDepth, group);
		yield return TestResult.Assert(nameof(GetPath), GetPath, group);
	}

	private bool Enumerate_BFS(out string message)
	{
		var expected = new[] { "A", "B", "C", "D", "E", "F" };

		var actual = _tree!.Enumerate(SearchMode.BreadthFirst)
			.Select(static n => n.Value.gameObject.name)
			.ToArray();

		TestResult.GetResult(out message, string.Join(", ", actual), string.Join(", ", expected));
		return actual.CompareValues(expected);
	}

	private bool Enumerate_DFS(out string message)
	{
		var expected = new[] { "A", "B", "D", "E", "C", "F" };

		var actual = _tree!.Enumerate(SearchMode.DepthFirst)
			.Select(static n => n.Value.gameObject.name)
			.ToArray();

		TestResult.GetResult(out message, string.Join(", ", actual), string.Join(", ", expected));
		return actual.CompareValues(expected);
	}

	private bool FindNodeAtPath(out string message)
	{
		var expected = _objects![4].transform; // E
		string path = "A/B/E";
		var actual = _tree!.GetNodeAtPath(path);

		TestResult.GetResult(out message, actual.ToString(), expected.ToString());
		return actual == expected;
	}

	private bool GetDepth(out string message)
	{
		int expected = 2;
		var node = _tree!.Find(SearchMode.DepthFirst, "E");
		int actual = node.GetDepth();

		TestResult.GetResult(out message, actual.ToString(), expected.ToString());
		return actual == expected;
	}

	private bool GetPath(out string message)
	{
		string expected = "A/B/E";
		var node = _tree!.Find(SearchMode.DepthFirst, "E");
		string actual = node.GetPath();

		return TestResult.GetResult(out message, actual, expected);
	}
}
