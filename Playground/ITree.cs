using System.Collections;
using System.Collections.Generic;

namespace FrootLuips.Playground;
#nullable enable
public sealed class Tree<T> : ITree<T>, IEnumerable<T>
	where T : class, ITreeNode<T>
{
	public T RootNode { get; }

	public string Name => this.RootNode.Name;

	public int ChildCount => this.RootNode.ChildCount;

	public Tree(T root)
	{
		RootNode = root;
	}

	public IEnumerator<T> GetEnumerator()
	{
		var parent = RootNode;

		int depth = 0;
		int[] childIndices = new int[TreeHelpers.MAX_DEPTH];
		bool noChildren;

		while (parent != null)
		{
			if (childIndices[depth] == 0)
			{
				yield return parent;
			}

			noChildren = depth == childIndices.Length - 1 || childIndices[depth] >= parent.ChildCount;
			if (noChildren)
			{
				childIndices[depth] = 0;
				depth--;
				parent = parent.Parent;
			}
			else
			{
				parent = parent.GetChild(childIndices[depth++]);
			}
			continue;
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

	public T GetChild(int index) => this.RootNode.GetChild(index);
	public T GetChild(string name) => this.RootNode.GetChild(name);
	public void AddChild(T child) => this.RootNode.AddChild(child);
	public bool RemoveChild(T child) => this.RootNode.RemoveChild(child);
}

public interface ITree<T> where T : class, ITree<T>
{
	string Name { get; }
	int ChildCount { get; }

	T GetChild(int index);
	T GetChild(string name);

	void AddChild(T child);
	bool RemoveChild(T child);
}

public interface ITreeNode<T> : ITree<T> where T : class, ITreeNode<T>
{
	T? Parent { get; }
}

public static class TreeHelpers
{
	public const int MAX_DEPTH = 1024;
	public const string PATH_SEPERATOR = "/";

	public static T GetRoot<T>(this T node, out int depth) where T : class, ITreeNode<T>
	{
		ref T root = ref node;

		for (depth = 0; depth < MAX_DEPTH; depth++)
		{
			if (root.Parent == null)
			{
				return root;
			}
			root = root.Parent;
		}
		return root;
	}
	public static T GetRoot<T>(this T node) where T : class, ITreeNode<T> => GetRoot(node, out _);

	public static string GetPath<T>(this T node) where T : class, ITreeNode<T>
	{
		List<string> pathParts = new();

		ref T current = ref node;
		for (int depth = 0; depth < MAX_DEPTH; depth++)
		{
			pathParts.Add(current.Name);
			if (current.Parent == null)
			{
				break;
			}
			else
			{
				current = current.Parent;
			}
		}
		pathParts.Reverse();
		return string.Join(PATH_SEPERATOR, pathParts);
	}

	public static bool ContainsChild<T>(this T node) where T : class, ITreeNode<T>
	{
		bool found = false;
		for (int i = 0; i < node.ChildCount && !found; i++)
		{
			found = node.GetChild(i) == node;
		}
		return found;
	}

	public static IEnumerator<T> GetEnumerator<T>(this T node) where T : class, ITreeNode<T>
	{
		for (int i = 0; i < node.ChildCount; i++)
		{
			yield return node.GetChild(i);
		}
	}
}
