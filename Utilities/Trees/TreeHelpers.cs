using System;
using System.Collections.Generic;
using FrootLuips.Subnautica.Trees.Handlers;

namespace FrootLuips.Subnautica.Trees;

/// <summary>
/// Determines how the children of a <see cref="ITreeNode{T}"/> are searched.
/// </summary>
public enum SearchMode
{
	/// <summary>
	/// Breadth-First Search.
	/// </summary>
	BreadthFirst,
	/// <summary>
	/// Depth-First Search.
	/// </summary>
	DepthFirst
}

/// <summary>
/// Static helper class for <see cref="Tree{T}"/> structures.
/// </summary>
public static class TreeHelpers
{
	/// <summary>
	/// The maximum depth allowed for tree structures in order to prevent infinite loops. Default value is 32.
	/// </summary>
	public static uint MaxDepth { get; set; } = 32;
	/// <summary>
	/// Delimiter for node paths.
	/// </summary>
	public const char PATH_SEPARATOR = '/';

	/// <summary>
	/// Gets the depth of a <paramref name="node"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="node"></param>
	/// <returns></returns>
	public static int GetDepth<T>(this Tree<T>.Node node)
	{
		var current = node;
		int depth;
		for (depth = 0; depth < MaxDepth; depth++)
		{
			if (current.Parent.HasValue)
				current = (Tree<T>.Node)current.Parent;
			else
				break;
		}
		return depth;
	}

	/// <summary>
	/// Gets a <paramref name="node"/>'s path.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="node"></param>
	/// <returns></returns>
	public static string GetPath<T>(this Tree<T>.Node node)
	{
		var current = node;
		Stack<string> path = new(capacity: 1);
		for (int i = 0; i < MaxDepth; i++)
		{
			path.Push(current.Name);

			if (current.Parent.HasValue)
				current = (Tree<T>.Node)current.Parent;
			else
				break;
		}
		return string.Join(PATH_SEPARATOR.ToString(), path);
	}

	/// <summary>
	/// Enumerates over all of a <paramref name="node"/>'s ancestors.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="node"></param>
	/// <returns></returns>
	public static IEnumerable<Tree<T>.Node> EnumerateAncestors<T>(this	Tree<T>.Node node)
	{
		var current = node;
		for (int i = 0; i < MaxDepth; i++)
		{
			yield return current;
			if (current.Parent.HasValue)
				current = (Tree<T>.Node)current.Parent;
			else
				yield break;
		}
	}

	/// <summary>
	/// Enumerates over all children of a <paramref name="node"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="node"></param>
	/// <param name="search"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static IEnumerable<Tree<T>.Node> Enumerate<T>(this Tree<T>.Node node, SearchMode search)
	{
		return search switch {
			SearchMode.BreadthFirst => Enumerate_BreadthFirst(node),
			SearchMode.DepthFirst => Enumerate_DepthFirst(node),
			_ => throw new ArgumentOutOfRangeException(nameof(search)),
		};
	}

	private static IEnumerable<Tree<T>.Node> Enumerate_BreadthFirst<T>(Tree<T>.Node root)
	{
		Queue<Tree<T>.Node> queue = new(capacity: 1);
		queue.Enqueue(root);
		Tree<T>.Node current;
		do
		{
			current = queue.Dequeue();
			if (current.GetDepth() < MaxDepth)
				yield return current;
			else
				continue;

			for (int i = 0; i < current.ChildCount; i++)
			{
				var child = current[childIndex: i];
				if (!CheckForChildDesync(current, child))
					queue.Enqueue(child);
			}
		}
		while (queue.Count > 0);
	}

	private static IEnumerable<Tree<T>.Node> Enumerate_DepthFirst<T>(Tree<T>.Node root)
	{
		Stack<Tree<T>.Node> stack = new(capacity: 1);
		stack.Push(root);
		Tree<T>.Node current;
		do
		{
			current = stack.Pop();
			if (current.GetDepth() < MaxDepth)
				yield return current;
			else
				continue;

			for (int i = current.ChildCount - 1; i >= 0; i--)
			{
				var child = current[childIndex: i];
				if (!CheckForChildDesync(current, child))
					stack.Push(child);
			}
		}
		while (stack.Count > 0);
	}

	private static bool CheckForChildDesync<T>(Tree<T>.Node? current, Tree<T>.Node child)
	{
		if (child.Parent != current)
		{
			const string message = "Child desync detected. Make sure to remove a node from the parent when a new one is assigned.";
			Plugin.Logger.LogWarning(message);
			return true;
		}

		return false;
	}

	/// <summary>
	/// Creates a new <see cref="Tree{T}"/> with this node at it's <paramref name="root"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="root"></param>
	/// <returns></returns>
	public static Tree<T> Create<T>(this T root)
		where T : class, ITreeNode<T>
	{
		return new Tree<T>(root, TreeNodeHandler<T>.Main);
	}
}
