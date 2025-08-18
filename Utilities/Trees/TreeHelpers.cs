using System;
using System.Collections.Generic;
using FrootLuips.Subnautica.Trees.Handlers;

namespace FrootLuips.Subnautica.Trees;

/// <summary>
/// Static helper class for <see cref="Tree{T}"/> structures.
/// </summary>
public static class TreeHelpers
{
	/// <summary>
	/// The maximum depth allowed for tree structures in order to prevent infinite loops. Default value is 32.
	/// </summary>
	public static ushort MaxDepth { get; set; } = 32;
	/// <summary>
	/// Delimiter for node paths.
	/// </summary>
	public const char PATH_SEPARATOR = '/';

	private const string _MESSAGE = "Child desync detected. Make sure to remove a node from the parent when a new one is assigned.";

	/// <summary>
	/// Gets the root node.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="node"></param>
	/// <returns></returns>
	public static Tree<T>.Node GetRoot<T>(this Tree<T>.Node node)
	{
		var root = node;
		bool found = false;
		for (int i = 0; i < MaxDepth && !found; i++)
		{
			if (root.IsRoot)
				found = true;
			else
				root = (Tree<T>.Node)root.Parent!;
		}
		return root;
	}

	/// <summary>
	/// Gets the depth of a <paramref name="node"/>.
	/// </summary>
	/// <remarks>
	/// A node without a parent has a depth of 0.
	/// </remarks>
	/// <typeparam name="T"></typeparam>
	/// <param name="node"></param>
	/// <returns></returns>
	public static int GetDepth<T>(this Tree<T>.Node node)
	{
		var current = node;
		int depth;
		for (depth = 0; depth < MaxDepth; depth++)
		{
			if (current.IsRoot)
				break;
			else
				current = (Tree<T>.Node)current.Parent!;
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

			if (current.IsRoot)
				break;
			else
				current = (Tree<T>.Node)current.Parent!;
		}
		return string.Join(PATH_SEPARATOR.ToString(), path);
	}

	/// <summary>
	/// Gets the first immediate child with the name <paramref name="childName"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="node"></param>
	/// <param name="childName"></param>
	/// <returns></returns>
	/// <exception cref="NodeNotFoundException"></exception>
	public static Tree<T>.Node GetChild<T>(this Tree<T>.Node node, string childName)
	{
		for (int i = 0; i < node.ChildCount; i++)
		{
			var child = node.GetChild(i);
			if (child.Name == childName)
				return child;
		}
		throw NodeNotFoundException.WithName(childName);
	}
	/// <summary>
	/// Enumerates over all of a <paramref name="node"/>'s ancestors.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="node"></param>
	/// <returns></returns>
	public static IEnumerable<Tree<T>.Node> EnumerateAncestors<T>(this Tree<T>.Node node)
	{
		var current = node;
		for (int i = 0; i < MaxDepth; i++)
		{
			yield return current;
			if (current.IsRoot)
				yield break;
			else
				current = (Tree<T>.Node)current.Parent!;
		}
	}

	/// <summary>
	/// Enumerates over all descendants of a <paramref name="node"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="node"></param>
	/// <param name="search"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	/// <exception cref="InvalidOperationException"></exception>
	public static IEnumerable<Tree<T>.Node> Enumerate<T>(this Tree<T>.Node node, SearchMode search)
	{
		return Enumerate(node, new SearchOptions<T>() { Search = search });
	}

	/// <summary>
	/// Enumerates over the values of all descendants of a <paramref name="node"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="node"></param>
	/// <param name="search"></param>
	/// <returns></returns>
	public static IEnumerable<T> EnumerateValues<T>(this Tree<T>.Node node, SearchMode search)
	{
		return EnumerateValues(node, new SearchOptions<T>() { Search = search });
	}

	/// <summary>
	/// <inheritdoc cref="Enumerate{T}(Tree{T}.Node, SearchMode)"/>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="node"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	/// <exception cref="InvalidOperationException"></exception>
	public static IEnumerable<Tree<T>.Node> Enumerate<T>(this Tree<T>.Node node, SearchOptions<T> options)
	{
		static Tree<T>.Node converter(Tree<T>.Node n) => n;
		try
		{
			return options.Search switch {
				SearchMode.BreadthFirst => Enumerate_BreadthFirst(node, options, converter),
				SearchMode.DepthFirst => Enumerate_DepthFirst(node, options, converter),
				_ => throw new ArgumentOutOfRangeException(nameof(SearchMode)),
			};
		}
		catch
		{
			throw;
		}
	}

	/// <summary>
	/// <inheritdoc cref="EnumerateValues{T}(Tree{T}.Node, SearchMode)"/>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="node"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	/// <exception cref="InvalidOperationException"></exception>
	public static IEnumerable<T> EnumerateValues<T>(this Tree<T>.Node node, SearchOptions<T> options)
	{
		static T converter(Tree<T>.Node n) => n.Value;
		try
		{
			return options.Search switch {
				SearchMode.BreadthFirst => Enumerate_BreadthFirst(node, options, converter),
				SearchMode.DepthFirst => Enumerate_DepthFirst(node, options, converter),
				_ => throw new ArgumentOutOfRangeException(nameof(SearchMode)),
			};
		}
		catch
		{
			throw;
		}
	}

	private static IEnumerable<TResult> Enumerate_BreadthFirst<T, TResult>(
		Tree<T>.Node root,
		SearchOptions<T> options,
		Converter<Tree<T>.Node, TResult> converter)
	{
		Queue<Tree<T>.Node> queue = new(capacity: 1);
		queue.Enqueue(root);
		Tree<T>.Node current;
		do
		{
			current = queue.Dequeue();
			yield return converter(current);

			for (int i = 0; i < current.ChildCount; i++)
			{
				var child = current[childIndex: i];
				CheckForChildDesync(parent: current, child);

				if (options.ShouldSearch(child))
					queue.Enqueue(child);
			}
		}
		while (queue.Count > 0);
	}

	private static IEnumerable<TResult> Enumerate_DepthFirst<T, TResult>(
		Tree<T>.Node root,
		SearchOptions<T> options,
		Converter<Tree<T>.Node, TResult> converter)
	{
		Stack<Tree<T>.Node> stack = new(capacity: 1);
		stack.Push(root);
		Tree<T>.Node current;
		do
		{
			current = stack.Pop();
			yield return converter(current);

			for (int i = current.ChildCount - 1; i >= 0; i--)
			{
				var child = current.GetChild(i);
				CheckForChildDesync(parent: current, child);

				if (options.ShouldSearch(child))
					stack.Push(child);
			}
		}
		while (stack.Count > 0);
	}

	private static void CheckForChildDesync<T>(Tree<T>.Node parent, Tree<T>.Node child)
	{
		if (child.Parent != parent)
		{
			throw new InvalidOperationException(_MESSAGE);
		}
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

/// <summary>
/// The exception thrown when a node is not found within a <see cref="Tree{T}"/>.
/// </summary>
[Serializable]
public class NodeNotFoundException : Exception
{
	private const string
		_NAME = "There is no node in the tree with the name '{0}'.",
		_VALUE = "There is no node in the tree with the given value '{0}'.",
		_PATH = "No node exists at the path '{0}'",
		_PREDICATE = "There is no node in the tree that matches the predicate.";

	/// <summary/>
	public NodeNotFoundException() { }
	/// <summary/>
	public NodeNotFoundException(string message) : base(message) { }
	/// <summary/>
	public NodeNotFoundException(string message, Exception inner) : base(message, inner) { }
	/// <summary/>
	protected NodeNotFoundException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

	/// <summary>
	/// No node was found with the given <paramref name="name"/>.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="inner"></param>
	public static NodeNotFoundException WithName(string name, Exception? inner = null)
		=> Create(string.Format(_NAME, name), inner);

	/// <summary>
	/// No node was found with the given <paramref name="value"/>.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="inner"></param>
	/// <returns></returns>
	public static NodeNotFoundException WithValue(object? value, Exception? inner = null)
		=> Create(string.Format(_VALUE, value), inner);
	/// <summary>
	/// No node was found at the given <paramref name="path"/>.
	/// </summary>
	/// <param name="path"></param>
	/// <param name="inner"></param>
	/// <returns></returns>
	public static NodeNotFoundException AtPath(string[] path, Exception? inner = null)
		=> Create(string.Format(_PATH, string.Join(TreeHelpers.PATH_SEPARATOR.ToString(), path)), inner);
	/// <summary>
	/// No node was found that meets a <see cref="Predicate{T}"/>
	/// </summary>
	/// <param name="inner"></param>
	/// <returns></returns>
	public static NodeNotFoundException MeetsPredicate(Exception? inner = null)
		=> Create(_PREDICATE, inner);

	private static NodeNotFoundException Create(string msg, Exception? inner)
	{
		return inner == null ? new NodeNotFoundException(msg) : new NodeNotFoundException(msg, inner);
	}
}
