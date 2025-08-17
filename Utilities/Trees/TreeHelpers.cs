using System;
using System.Collections.Generic;

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
	/// The maximum depth allowed for tree structures in order to prevent infinite loops. Default value is 256.
	/// </summary>
	public static uint MaxDepth { get; set; } = 256;
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
	public static int GetDepth<T>(this Node<T> node) where T : class
	{
		var current = node;
		int depth;
		for (depth = 0; depth < MaxDepth; depth++)
		{
			if (current.Parent.HasValue)
				current = (Node<T>)current.Parent;
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
	public static string GetPath<T>(this Node<T> node) where T : class
	{
		var current = node;
		Stack<string> path = new(capacity: 1);
		for (int i = 0; i < MaxDepth; i++)
		{
			path.Push(current.Name);

			if (current.Parent.HasValue)
				current = (Node<T>)current.Parent;
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
	public static IEnumerable<Node<T>> EnumerateAncestors<T>(this Node<T> node) where T : class
	{
		var current = node;
		for (int i = 0; i < MaxDepth; i++)
		{
			yield return current;
			if (current.Parent.HasValue)
				current = (Node<T>)current.Parent;
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
	public static IEnumerable<Node<T>> Enumerate<T>(this Node<T> node, SearchMode search) where T : class
	{
		return search switch {
			SearchMode.BreadthFirst => Enumerate_BreadthFirst(node),
			SearchMode.DepthFirst => Enumerate_DepthFirst(node),
			_ => throw new ArgumentOutOfRangeException(nameof(search)),
		};
	}

	private static IEnumerable<Node<T>> Enumerate_BreadthFirst<T>(Node<T> root) where T : class
	{
		Queue<Node<T>> queue = new(capacity: 1);
		queue.Enqueue(root);
		Node<T> current;
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

	private static IEnumerable<Node<T>> Enumerate_DepthFirst<T>(Node<T> root) where T : class
	{
		Stack<Node<T>> stack = new(capacity: 1);
		stack.Push(root);
		Node<T> current;
		do
		{
			current = stack.Pop();
			if (current.GetDepth() < MaxDepth)
				yield return current;
			else
				continue;

			for (int i = 0; i < current.ChildCount; i++)
			{
				var child = current[childIndex: i];
				if (!CheckForChildDesync(current, child))
					stack.Push(child);
			}
		}
		while (stack.Count > 0);
	}

	private static bool CheckForChildDesync<T>(Node<T>? current, Node<T> child) where T : class
	{
		if (child.Parent != current)
		{
			const string message = "Child desync detected. Make sure to remove a node from the parent when a new one is assigned.";
			Plugin.Logger.LogWarning(message);
			return true;
		}

		return false;
	}
}
