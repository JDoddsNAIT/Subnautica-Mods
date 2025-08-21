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
	/// The root node of the tree.
	/// </summary>
	public T Root { get; set; }
	/// <summary>
	/// The <see cref="ITreeHandler{T}"/> used to access the parent, name, and children of a node.
	/// </summary>
	public ITreeHandler<T> Handler { get; }

	/// <summary>
	/// Constructs a <see cref="Tree{T}"/> structure with the given <paramref name="root"/> node and <paramref name="handler"/>.
	/// </summary>
	/// <param name="root"></param>
	/// <param name="handler"></param>
	public Tree(T root, ITreeHandler<T> handler)
	{
		this.Root = root;
		this.Handler = handler;
	}

	/// <summary>
	/// Enumerates over every node in the tree.
	/// </summary>
	/// <param name="search"></param>
	/// <returns></returns>
	public IEnumerable<T> Enumerate(SearchMode search)
		=> TreeHelpers.Enumerate(this.Root, this.Handler, search);

	/// <summary>
	/// <inheritdoc cref="Enumerate(SearchMode)"/>
	/// </summary>
	/// <param name="options"></param>
	/// <returns></returns>
	public IEnumerable<T> Enumerate(SearchOptions<T> options)
		=> TreeHelpers.Enumerate(this.Root, this.Handler, options);

	/// <summary>
	/// Finds the first node that meets the given <paramref name="predicate"/>.
	/// </summary>
	/// <param name="predicate"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	/// <exception cref="NodeNotFoundException"></exception>
	public T Find(Predicate<T> predicate, SearchOptions<T> options)
	{
		foreach (var node in this.Enumerate(options))
		{
			if (predicate(node))
				return node;
		}
		throw NodeNotFoundException.MeetsPredicate();
	}

	/// <summary>
	/// Finds all nodes in the tree that meet the given <paramref name="predicate"/>.
	/// </summary>
	/// <param name="predicate"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	/// <exception cref="NodeNotFoundException"></exception>
	public IEnumerable<T> FindAll(Predicate<T> predicate, SearchOptions<T> options)
	{
		int count = 0;
		foreach (var node in this.Enumerate(options))
		{
			if (predicate(node))
			{
				count++;
				yield return node;
			}
		}

		if (count == 0)
			throw NodeNotFoundException.MeetsPredicate();
	}

	/// <summary>
	/// Finds the first node with the given <paramref name="name"/>.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	/// <exception cref="NodeNotFoundException"></exception>
	public T Find(string name, SearchOptions<T> options)
	{
		foreach (var node in this.Enumerate(options))
		{
			if (Handler.GetName(node) == name)
				return node;
		}
		throw NodeNotFoundException.WithName(name);
	}

	/// <summary>
	/// Gets the node at the given <paramref name="path"/> relative to the <see cref="Root"/>.
	/// </summary>
	/// <param name="path"></param>
	/// <returns></returns>
	/// <exception cref="NodeNotFoundException"></exception>
	public T GetNodeAt(string path)
	{
		T current = this.Root;
		var parts = path.Split(TreeHelpers.PATH_SEPARATOR);
		if (parts[0] == Handler.GetName(current))
			parts = parts[1..];

		for (int i = 0; i < parts.Length; i++)
		{
			current = parts[i] switch {
				".." when Handler.TryGetParent(current, out T? parent) => parent,
				_ when Handler.TryGetChildByName(node: current, name: parts[i], out T? child) => child,
				_ => throw NodeNotFoundException.AtPath(parts[..(i + 1)]),
			};
		}
		return current;
	}
}
