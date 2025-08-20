using System;
using System.Collections.Generic;

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

	private const string _CHILD_DESYNC_MESSAGE = "Child desync detected. Make sure to remove a node from the parent when a new one is assigned.";

	/// <summary>
	/// Enumerates over all descendants of a <paramref name="node"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="node"></param>
	/// <param name="search"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	/// <exception cref="InvalidOperationException"></exception>
	public static IEnumerable<Tree<T>.Node> EnumerateNodes<T>(this Tree<T>.Node node, SearchMode search)
	{
		return EnumerateNodes(node, new Tree<T>.SearchOptions() { Search = search });
	}

	/// <summary>
	/// Enumerates over the values of all descendants of a <paramref name="node"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="node"></param>
	/// <param name="search"></param>
	/// <returns></returns>
	public static IEnumerable<T> Enumerate<T>(this Tree<T>.Node node, SearchMode search)
	{
		return Enumerate(node, new Tree<T>.SearchOptions() { Search = search });
	}

	/// <summary>
	/// <inheritdoc cref="EnumerateNodes{T}(Tree{T}.Node, SearchMode)"/>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="node"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	/// <exception cref="InvalidOperationException"></exception>
	public static IEnumerable<Tree<T>.Node> EnumerateNodes<T>(this Tree<T>.Node node, Tree<T>.SearchOptions options)
	{
		static Tree<T>.Node converter(Tree<T>.Node n) => n;
		try
		{
			return options.Search switch {
				SearchMode.Ancestors => Enumerate_Ancestors(node, options, converter),
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
	/// <inheritdoc cref="Enumerate{T}(Tree{T}.Node, SearchMode)"/>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="node"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	/// <exception cref="InvalidOperationException"></exception>
	public static IEnumerable<T> Enumerate<T>(this Tree<T>.Node node, Tree<T>.SearchOptions options)
	{
		static T converter(Tree<T>.Node n) => n.Value;
		try
		{
			return options.Search switch {
				SearchMode.BreadthFirst => Enumerate_BreadthFirst(node, options, converter),
				SearchMode.DepthFirst => Enumerate_DepthFirst(node, options, converter),
				SearchMode.Ancestors => Enumerate_Ancestors(node, options, converter),
				_ => throw new ArgumentOutOfRangeException(nameof(SearchMode)),
			};
		}
		catch
		{
			throw;
		}
	}

	internal static IEnumerable<TResult> Enumerate_Ancestors<T, TResult>(
		Tree<T>.Node start, Tree<T>.SearchOptions options, Converter<Tree<T>.Node, TResult> converter)
	{
		var current = start;
		bool foundRoot;
		do
		{
			if (current == start && options.IncludeStart)
				yield return converter(current);
			else if (current.ShouldSearch(options))
				yield return converter(current);

			foundRoot = current.IsRoot;
			if (!foundRoot)
			{
				current = (Tree<T>.Node)current.Parent!;
			}
		}
		while (!foundRoot);
	}

	internal static IEnumerable<TResult> Enumerate_BreadthFirst<T, TResult>(
		Tree<T>.Node start,
		Tree<T>.SearchOptions options,
		Converter<Tree<T>.Node, TResult> converter)
	{
		Queue<Tree<T>.Node> queue = new(capacity: 1);
		queue.Enqueue(start);
		Tree<T>.Node current;
		do
		{
			current = queue.Dequeue();
			if (options.IncludeStart || current != start)
				yield return converter(current);

			for (int i = 0; i < current.ChildCount; i++)
			{
				var child = current[childIndex: i];
				CheckForChildDesync(parent: current, child);

				if (child.ShouldSearch(options))
					queue.Enqueue(child);
			}
		}
		while (queue.Count > 0);
	}

	internal static IEnumerable<TResult> Enumerate_DepthFirst<T, TResult>(
		Tree<T>.Node start,
		Tree<T>.SearchOptions options,
		Converter<Tree<T>.Node, TResult> converter)
	{
		Stack<Tree<T>.Node> stack = new(capacity: 1);
		stack.Push(start);
		Tree<T>.Node current;
		do
		{
			current = stack.Pop();
			if (options.IncludeStart || current != start)
				yield return converter(current);

			for (int i = current.ChildCount - 1; i >= 0; i--)
			{
				var child = current.GetChild(i);
				CheckForChildDesync(parent: current, child);

				if (child.ShouldSearch(options))
					stack.Push(child);
			}
		}
		while (stack.Count > 0);
	}

	private static void CheckForChildDesync<T>(Tree<T>.Node parent, Tree<T>.Node child)
	{
		if (child.Parent != parent)
		{
			throw new InvalidOperationException(_CHILD_DESYNC_MESSAGE);
		}
	}

	internal static bool ShouldSearch<T>(this Tree<T>.Node node, Tree<T>.SearchOptions options)
	{
		return node.GetDepth() <= (options.MaxDepth ?? TreeHelpers.MaxDepth)
				&& (options.Predicate == null || options.Predicate(node));
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
