using System;
using System.Collections.Generic;
using FrootLuips.Subnautica.Extensions;
using static FrootLuips.Subnautica.Validation.Validator;

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
	/// The separator string for node paths.
	/// </summary>
	public const string PATH_SEPARATOR = "/";
	/// <summary>
	/// The delimiter character for node paths.
	/// </summary>
	public const char PATH_DELIMITER = '/';

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
	/// <remarks>
	/// This method does not use the <paramref name="handler"/>'s <see cref="ITreeHandler{T}.GetRoot(T)"/> method, rather provides a default implementation for use in classes that implement the interface.
	/// </remarks>
	/// <typeparam name="T"></typeparam>
	/// <param name="node"></param>
	/// <param name="handler"></param>
	/// <returns></returns>
	public static T GetRoot<T>(T node, ITreeHandler<T> handler)
	{
		T current = node;
		bool foundRoot = false;
		for (int i = 0; i < MaxDepth && !foundRoot; i++)
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
	/// Counts the ancestors of a <paramref name="node"/>.
	/// </summary>
	/// <remarks>
	/// This method does not use the <paramref name="handler"/>'s <see cref="ITreeHandler{T}.GetDepth(T)"/> method, rather provides a default implementation for use in classes that implement the interface.
	/// </remarks>
	/// <typeparam name="T"></typeparam>
	/// <param name="node"></param>
	/// <param name="handler"></param>
	/// <returns></returns>
	public static int GetDepth<T>(T node, ITreeHandler<T> handler)
	{
		T current = node;
		bool foundRoot = false;
		int depth = 0;
		for (int i = 0; i < MaxDepth && !foundRoot; i++)
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
	/// <remarks>
	/// See also: <seealso cref="PATH_DELIMITER"/>
	/// </remarks>
	/// <typeparam name="T"></typeparam>
	/// <param name="node"></param>
	/// <param name="handler"></param>
	/// <returns>The names of this <paramref name="node"/> and it's ancestors delimited by the <seealso cref="PATH_DELIMITER"/>.</returns>
	public static string GetPath<T>(T node, ITreeHandler<T> handler)
	{
		T current = node;
		bool foundRoot = false;
		var path = new Stack<string>();
		for (int i = 0; i < MaxDepth && !foundRoot; i++)
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
		return path.Join(PATH_SEPARATOR);
	}

	#region Handler Extensions
	/// <summary>
	/// Gets the first node at the given <paramref name="path"/>, relative to <paramref name="node"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="handler"></param>
	/// <param name="node"></param>
	/// <param name="path"></param>
	/// <returns></returns>
	/// <exception cref="NodeNotFoundException"></exception>
	public static T GetNode<T>(this ITreeHandler<T> handler, T node, string path)
	{
		return path switch {
			null or "" => node,
			_ when path.Contains(PATH_SEPARATOR) => handler.GetNode(node, path.Split(PATH_DELIMITER)),
			_ => handler.GetChild(node, path),
		};
	}

	/// <inheritdoc cref="GetNode{T}(ITreeHandler{T}, T, string)"/>
	public static T GetNode<T>(this ITreeHandler<T> handler, T node, params string[] path)
	{
		if (path.Length == 0)
			return node;
		T result = node;
		for (int i = 0; i < path.Length; i++)
		{
			result = path[i] switch {
				".." when handler.TryGetParent(result, out T? parent) => parent,
				not ".." when Try(() => handler.GetChild(result, path[i]), out T? child) => child,
				_ => throw NodeNotFoundException.AtPath(handler.GetName(node), path[..(i + 1)]),
			};
		}
		return result;
	}

	/// <summary>
	/// Gets the first child of a <paramref name="node"/> with the given <paramref name="name"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="handler"></param>
	/// <param name="node"></param>
	/// <param name="name"></param>
	/// <returns></returns>
	/// <exception cref="NodeNotFoundException"></exception>
	public static T GetChild<T>(this ITreeHandler<T> handler, T node, string name)
	{
		T result;
		for (int i = 0; i < handler.GetChildCount(node); i++)
		{
			result = handler.GetChildByIndex(node, i);
			if (handler.GetName(result) == name)
			{
				return result;
			}
		}
		throw NodeNotFoundException.WithName(name);
	}
	#endregion

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
				var child = handler.GetChildByIndex(current, i);
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
				var child = handler.GetChildByIndex(current, i);
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
		_PATH = "Could not find node at '{1}' (relative to {0})",
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
	/// <param name="nodeName"></param>
	/// <param name="path"></param>
	/// <param name="inner"></param>
	/// <returns></returns>
	public static NodeNotFoundException AtPath(string nodeName, string[] path, Exception? inner = null)
		=> Create(string.Format(_PATH, nodeName, string.Join(TreeHelpers.PATH_SEPARATOR, path)), inner);
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
