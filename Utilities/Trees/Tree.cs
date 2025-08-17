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
	/// The root node of the tree. (Read only)
	/// </summary>
	public Node Root { get; }

	/// <summary>
	/// Constructs a new <see cref="Tree{T}"/> structure with a <paramref name="root"/> node and <paramref name="handler"/>.
	/// </summary>
	/// <param name="root"></param>
	/// <param name="handler"></param>
	public Tree(T root, ITreeHandler<T> handler) : this(new Tree<T>.Node(root, handler)) { }

	/// <summary>
	/// Constructs a new <see cref="Tree{T}"/> structure.
	/// </summary>
	/// <param name="root"></param>
	public Tree(Node root) => Root = root;

	/// <summary>
	/// Enumerates over all values in the tree.
	/// </summary>
	/// <param name="search"></param>
	/// <returns></returns>
	public IEnumerable<T> Enumerate(SearchMode search)
	{
		foreach (T value in Root.Enumerate(search))
		{
			yield return value;
		}
	}

	/// <summary>
	/// Finds the first node in the tree with the given <paramref name="name"/>.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="search"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentException"></exception>
	public T FindNodeWithName(string name, SearchMode search)
	{
		foreach (var node in Root.Enumerate(search))
		{
			if (node.Name == name)
				return node;
		}
		throw new ArgumentException($"There is no node in the tree with the name '{name}'");
	}

	/// <summary>
	/// Finds the first node in the tree that meets the <paramref name="predicate"/>.
	/// </summary>
	/// <param name="predicate"></param>
	/// <param name="search"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentException"></exception>
	public T Find(Predicate<T> predicate, SearchMode search)
	{
		foreach (var node in Root.Enumerate(search))
		{
			if (predicate(node))
				return node;
		}
		throw new ArgumentException($"No node exists that matches the predicate.");
	}

	/// <summary>
	/// Tries to find the first <paramref name="node"/> in the tree that meets the <paramref name="predicate"/>.
	/// </summary>
	/// <param name="predicate"></param>
	/// <param name="search"></param>
	/// <param name="node"></param>
	/// <returns></returns>
	public bool TryFind(Predicate<T> predicate, SearchMode search, out T? node)
	{
		try
		{
			node = Find(predicate, search);
			return true;
		}
		catch (Exception)
		{
			node = default;
			return false;
		}
	}

	/// <summary>
	/// Finds all nodes in the tree that meet the <paramref name="predicate"/>.
	/// </summary>
	/// <param name="predicate"></param>
	/// <param name="search"></param>
	/// <returns></returns>
	public IEnumerable<T> FindAll(Predicate<T> predicate, SearchMode search)
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
	/// <exception cref="ArgumentException"></exception>
	public T GetNodeAtPath(string path)
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
				string subPath = string.Join(TreeHelpers.PATH_SEPARATOR.ToString(), parts[..(i + 1)]);
				throw new ArgumentException($"No node exists at path '{subPath}'");
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
	public bool TryGetNodeAtPath(string path, out T? node)
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
