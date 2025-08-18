using System;
using System.Collections.Generic;

namespace FrootLuips.Subnautica.Trees;
/// <summary>
/// Represents a tree structure of <typeparamref name="T"/> values.
/// </summary>
/// <typeparam name="T"></typeparam>
public partial class Tree<T>
{
	/// <summary>
	/// The root node of the <see cref="Tree{T}"/>.
	/// </summary>
	public Node Root { get; }

	/// <summary>
	/// Constructs a new <see cref="Tree{T}"/> structure with a <paramref name="root"/> node and <paramref name="handler"/>.
	/// </summary>
	/// <param name="root"></param>
	/// <param name="handler"></param>
	public Tree(T root, ITreeHandler<T> handler) => Root = new Node(root, handler, isRoot: true);

	/// <inheritdoc cref="TreeHelpers.Enumerate{T}(Tree{T}.Node, SearchMode)"/>
	public IEnumerable<Node> Enumerate(SearchMode search) => Root.Enumerate(search);

	/// <inheritdoc cref="TreeHelpers.Enumerate{T}(Tree{T}.Node, SearchOptions{T})"/>
	public IEnumerable<Node> Enumerate(SearchOptions<T> options) => Root.Enumerate(options);

	/// <inheritdoc cref="TreeHelpers.EnumerateValues{T}(Tree{T}.Node, SearchMode)"/>
	public IEnumerable<T> EnumerateValues(SearchMode search) => Root.EnumerateValues(search);

	/// <inheritdoc cref="TreeHelpers.EnumerateValues{T}(Tree{T}.Node, SearchOptions{T})"/>
	public IEnumerable<T> EnumerateValues(SearchOptions<T> options) => Root.EnumerateValues(options);

	/// <summary>
	/// Finds the first node in the tree with the given <paramref name="name"/>.
	/// </summary>
	/// <param name="search"></param>
	/// <param name="name"></param>
	/// <returns></returns>
	/// <exception cref="NodeNotFoundException"></exception>
	public Node Find(SearchMode search, string name)
	{
		bool withName(Node node) => node.Name == name;
		return TryFind(search, withName, out var node)
			? node!.Value
			: throw NodeNotFoundException.WithName(name);
	}

	/// <summary>
	/// Finds the first node in the tree with the given <paramref name="value"/>.
	/// </summary>
	/// <param name="search"></param>
	/// <param name="value"></param>
	/// <returns></returns>
	/// <exception cref="NodeNotFoundException"></exception>
	public Node Find(SearchMode search, T value)
	{
		Predicate<Node> withValue = value switch {
			IEquatable<T> eq => (node) => eq.Equals(node.Value),
			_ => (node) => value?.Equals(node.Value) ?? false,
		};

		return TryFind(search, withValue, out var node)
			? node!.Value
			: throw NodeNotFoundException.WithValue(value);
	}

	/// <summary>
	/// <inheritdoc cref="Find(SearchMode, T)"/>
	/// </summary>
	/// <param name="search"></param>
	/// <param name="value"></param>
	/// <param name="comparer"></param>
	/// <returns></returns>
	/// <exception cref="NodeNotFoundException"></exception>
	public Node Find(SearchMode search, T value, IEqualityComparer<T> comparer)
	{
		bool withValue(Node node) => comparer.Equals(node.Value, value);
		return TryFind(search, withValue, out var node)
			? node!.Value
			: throw NodeNotFoundException.WithValue(value);
	}

	/// <summary>
	/// Finds the first node in the tree that meets the <paramref name="predicate"/>.
	/// </summary>
	/// <param name="search"></param>
	/// <param name="predicate"></param>
	/// <returns></returns>
	/// <exception cref="NodeNotFoundException"></exception>
	public Node Find(SearchMode search, Predicate<Node> predicate)
	{
		foreach (var node in this.Enumerate(search))
		{
			if (predicate(node))
				return node;
		}
		throw NodeNotFoundException.MeetsPredicate();
	}

	/// <summary>
	/// Tries to find the first <paramref name="node"/> in the tree that meets the <paramref name="predicate"/>.
	/// </summary>
	/// <param name="search"></param>
	/// <param name="predicate"></param>
	/// <param name="node"></param>
	/// <returns><see langword="true"/> if a node was found.</returns>
	public bool TryFind(SearchMode search, Predicate<Node> predicate, out Node? node)
	{
		try
		{
			node = Find(search, predicate);
			return true;
		}
		catch (Exception)
		{
			node = null;
			return false;
		}
	}

	/// <summary>
	/// Finds all nodes in the tree that meet the <paramref name="predicate"/>.
	/// </summary>
	/// <param name="search"></param>
	/// <param name="predicate"></param>
	/// <returns></returns>
	public IEnumerable<Node> FindAll(SearchMode search, Predicate<Node> predicate)
	{
		foreach (var node in Root.Enumerate(search))
		{
			if (predicate(node))
				yield return node;
		}
	}

	/// <summary>
	/// Gets a <see cref="Node"/> at the specified <paramref name="path"/>.
	/// </summary>
	/// <param name="path"></param>
	/// <returns></returns>
	/// <exception cref="NodeNotFoundException"></exception>
	public Node GetNodeAtPath(string path)
	{
		var parts = path.Split(TreeHelpers.PATH_SEPARATOR);

		if (parts[0] == Root.Name)
			parts = parts[1..];
		Node current = Root;

		for (int i = 0; i < parts.Length; i++)
		{
			bool found = false;
			for (int j = 0; j < current.ChildCount && !found; j++)
			{
				if (current[j].Name == parts[i])
				{
					found = true;
					current = current[j];
				}
			}

			if (!found)
			{
				throw NodeNotFoundException.AtPath(parts[..(i + 1)]);
			}
		}

		return current;
	}

	/// <summary>
	/// Tries to get a <see cref="Node"/> at the specified <paramref name="path"/>.
	/// </summary>
	/// <param name="path"></param>
	/// <param name="node"></param>
	/// <returns></returns>
	public bool TryGetNodeAtPath(string path, out Node? node)
	{
		try
		{
			node = GetNodeAtPath(path);
			return true;
		}
		catch (Exception)
		{
			node = default;
			return false;
		}
	}
}
