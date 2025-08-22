using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FrootLuips.Subnautica.Extensions;

namespace FrootLuips.Subnautica.Trees;
/// <summary>
/// Helper class for iterating over <see cref="Tree{T}"/> structures.
/// </summary>
public static class TreeHelpers
{
	/// <summary>
	/// The maximum depth allowed for nodes in a tree. Default value is 32.
	/// </summary>
	public static ushort MaxDepth { get; set; } = 32;
	/// <summary>
	/// The delimiter character for node paths.
	/// </summary>
	public const char PATH_SEPARATOR = '/';

	/// <summary>
	/// Creates a new <see cref="Tree{T}"/> with this <paramref name="node"/> at it's root.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="node"></param>
	/// <returns></returns>
	public static Tree<T> CreateTree<T>(this T node) where T : class, ITreeNode<T>
	{
		return new Tree<T>(node, TreeNodeHandler<T>.Main);
	}

	/// <summary>
	/// Gets the root object of a <paramref name="node"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="node"></param>
	/// <param name="handler"></param>
	/// <returns></returns>
	public static T GetRoot<T>(T node, ITreeHandler<T> handler)
	{
		T current = node;
		bool foundRoot = false;
		for (int i = 0; i < TreeHelpers.MaxDepth && !foundRoot; i++)
		{
			if (handler.TryGetParent(current, out var parent))
			{
				current = parent;
			}
			else
			{
				foundRoot = true;
			}
		}
		return current;
	}

	/// <summary>
	/// Gets the number of a <paramref name="node"/>'s ancestors.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="node"></param>
	/// <param name="handler"></param>
	/// <returns></returns>
	public static int GetDepth<T>(T node, ITreeHandler<T> handler)
	{
		T current = node;
		bool foundRoot = false;
		int depth = 0;
		for (int i = 0; i < TreeHelpers.MaxDepth && !foundRoot; i++)
		{
			if (handler.TryGetParent(current, out var parent))
			{
				current = parent;
				depth++;
			}
			else
			{
				foundRoot = true;
			}
		}
		return depth;
	}

	/// <summary>
	/// Gets the absolute path to a <paramref name="node"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="node"></param>
	/// <param name="handler"></param>
	/// <returns></returns>
	public static string GetPath<T>(T node, ITreeHandler<T> handler)
	{
		T current = node;
		bool foundRoot = false;
		var path = new Stack<string>();
		for (int i = 0; i < TreeHelpers.MaxDepth && !foundRoot; i++)
		{
			path.Push(handler.GetName(current));
			if (handler.TryGetParent(current, out var parent))
			{
				current = parent;
			}
			else
			{
				foundRoot = true;
			}
		}
		return string.Join(TreeHelpers.PATH_SEPARATOR.ToString(), path);
	}

	/// <summary>
	/// Gets the first child of a <paramref name="node"/> with the given <paramref name="name"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="handler"></param>
	/// <param name="node"></param>
	/// <param name="name"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentException"></exception>
	/// <exception cref="NodeNotFoundException"></exception>
	public static T GetChildByName<T>(this ITreeHandler<T> handler, T node, string name)
	{
		if (name.ContainsAny(TreeHelpers.PATH_SEPARATOR))
			throw new ArgumentException($"The name of a node cannot contain a '{TreeHelpers.PATH_SEPARATOR}'");

		T child;
		for (int i = 0; i < handler.GetChildCount(node); i++)
		{
			child = handler.GetChild(node, i);
			if (handler.GetName(child) == name)
				return child;
		}
		throw NodeNotFoundException.WithName(name);
	}

	/// <summary>
	/// Tries to get the first child of a <paramref name="node"/> with the name <paramref name="name"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="handler"></param>
	/// <param name="node"></param>
	/// <param name="name"></param>
	/// <param name="child"></param>
	/// <returns></returns>
	public static bool TryGetChildByName<T>(this ITreeHandler<T> handler, T node, string name, [NotNullWhen(true)] out T? child)
	{
		return Validation.Validator.Try(() => handler.GetChildByName(node, name), out child);
	}

	/// <summary>
	/// Enumerates over all nodes relative to the given <paramref name="node"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="handler"></param>
	/// <param name="node"></param>
	/// <param name="options">Defines how the search is conducted.</param>
	/// <returns></returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	/// <exception cref="NodeNotFoundException"></exception>
	public static IEnumerable<T> Enumerate<T>(this ITreeHandler<T> handler, T node, SearchOptions<T> options)
	{
		return options.Search switch {
			SearchMode.BreadthFirst => Enumerate_BreadthFirst(handler, node, options),
			SearchMode.DepthFirst => Enumerate_DepthFirst(handler, node, options),
			SearchMode.Ancestors => Enumerate_Ancestors(handler, node, options),
			_ => throw new ArgumentOutOfRangeException(nameof(options)),
		};
	}

	private static IEnumerable<T> Enumerate_Ancestors<T>(ITreeHandler<T> handler, T node, SearchOptions<T> options)
	{
		T? current = node;
		if (options.Inclusive)
			yield return current;

		while (handler.TryGetParent(current, out current))
		{
			if (options.ShouldSearch(current, handler))
				yield return current;
		}
	}

	private static IEnumerable<T> Enumerate_DepthFirst<T>(ITreeHandler<T> handler, T node, SearchOptions<T> options)
	{
		var stack = new Stack<T>();
		stack.Push(node);
		T current;
		bool isFirst = true;
		do
		{
			current = stack.Pop();

			if (isFirst && options.Inclusive)
				yield return current;
			else if (!isFirst)
				yield return current;
			else
				isFirst = false;

			for (int i = handler.GetChildCount(current) - 1; i >= 0; i--)
			{
				var child = handler.GetChild(current, i);
				if (options.ShouldSearch(child, handler))
					stack.Push(child);
			}
		}
		while (stack.Count > 0);
	}

	private static IEnumerable<T> Enumerate_BreadthFirst<T>(ITreeHandler<T> handler, T node, SearchOptions<T> options)
	{
		Queue<T> queue = new(capacity: 1);
		queue.Enqueue(node);
		T current;
		bool isFirst = true;
		do
		{
			current = queue.Dequeue();

			if (isFirst && options.Inclusive)
				yield return current;
			else if (!isFirst)
				yield return current;
			else
				isFirst = false;

			for (int i = 0; i < handler.GetChildCount(current); i++)
			{
				var child = handler.GetChild(current, i);
				if (options.ShouldSearch(child, handler))
					queue.Enqueue(child);
			}
		}
		while (queue.Count > 0);
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
	[Obsolete]
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
