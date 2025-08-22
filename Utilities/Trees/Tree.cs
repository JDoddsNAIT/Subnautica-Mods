using System;
using System.Collections.Generic;
using FrootLuips.Subnautica.Validation;

namespace FrootLuips.Subnautica.Trees;
/// <summary>
/// Represents a tree structure of <typeparamref name="T"/> values.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Tree<T>
{
	/// <summary>
	/// The root node of the tree.
	/// </summary>
	public T Root { get; set; }
	/// <summary>
	/// The <see cref="ITreeHandler{T}"/> used to access the parent, name, and children of a node.
	/// </summary>
	public ITreeHandler<T> Handler { get; }

	/// <inheritdoc cref="GetNodeAt(string)"/>
	public T this[string path] => GetNodeAt(path);

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
		=> TreeHelpers.Enumerate(this.Handler, this.Root, search);

	/// <summary>
	/// <inheritdoc cref="Enumerate(SearchMode)"/>
	/// </summary>
	/// <param name="options"></param>
	/// <returns></returns>
	public IEnumerable<T> Enumerate(SearchOptions<T> options)
		=> TreeHelpers.Enumerate(this.Handler, this.Root, options);

	/// <summary>
	/// Finds all nodes in the tree that meet the given <paramref name="predicate"/>.
	/// </summary>
	/// <param name="predicate"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	public IEnumerable<T> FindAll(Predicate<T> predicate, SearchOptions<T> options)
	{
		foreach (var node in this.Enumerate(options))
		{
			if (predicate(node))
			{
				yield return node;
			}
		}
	}

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

	/// <inheritdoc cref="Find(Predicate{T}, SearchOptions{T})"/>
	public bool TryFind(Predicate<T> predicate, SearchOptions<T> options, out T? result)
	{
		return Validator.Try(() => Find(predicate, options), out result);
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

	/// <inheritdoc cref="Find(string, SearchOptions{T})"/>
	public bool TryFind(string name, SearchOptions<T> options, out T? result)
	{
		return Validator.Try(() => Find(name, options), out result);
	}

	/// <summary>
	/// Gets the node at the given <paramref name="path"/> relative to the <see cref="Root"/>.
	/// </summary>
	/// <param name="path"></param>
	/// <returns></returns>
	/// <exception cref="NodeNotFoundException"></exception>
	public T GetNodeAt(string path)
	{
		return GetNodeAtPath(path.Split(TreeHelpers.PATH_SEPARATOR));
	}

	/// <inheritdoc cref="GetNodeAt(string)"/>
	public bool TryGetNodeAt(string path, out T? result)
	{
		return Validator.Try(() => GetNodeAt(path), out result);
	}

	/// <inheritdoc cref="GetNodeAt(string)"/>
	public T GetNodeAtPath(params string[] path)
	{
		T current = this.Root;

		for (int i = 0; i < path.Length; i++)
		{
			try
			{
				if (path[i] == ".." && Handler.TryGetParent(current, out T? parent))
				{
					current = parent;
				}
				else if (path[i] != "..")
				{
					current = Handler.GetChildByName(current, path[i]);
				}
				else
				{
					throw NodeNotFoundException.AtPath(Handler.GetName(this.Root), path[..(i + 1)]);
				}
			}
			catch (Exception ex) when (ex is not NodeNotFoundException)
			{
				throw NodeNotFoundException.AtPath(Handler.GetName(this.Root), path[..(i + 1)], ex);
			}
		}
		return current;
	}
}
