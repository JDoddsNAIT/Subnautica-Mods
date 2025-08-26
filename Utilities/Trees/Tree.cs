using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FrootLuips.Subnautica.Validation;

namespace FrootLuips.Subnautica.Trees;
/// <summary>
/// Represents a tree structure of <typeparamref name="T"/> objects.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Tree<T> : ITreeHandler<T>
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
	/// Enumerates over all nodes, relative to the <see cref="Root"/>.
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
		return this.Enumerate(options).Where(predicate.Invoke);
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
	public bool TryFind(Predicate<T> predicate, SearchOptions<T> options, [NotNullWhen(true)] out T? result)
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
	public bool TryFind(string name, SearchOptions<T> options, [NotNullWhen(true)] out T? result)
	{
		return Validator.Try(() => Find(name, options), out result);
	}

	/// <summary>
	/// Gets the first node at the given <paramref name="path"/>, relative to the <see cref="Tree{T}.Root"/>
	/// </summary>
	/// <param name="path"></param>
	/// <returns></returns>
	/// <exception cref="NodeNotFoundException"></exception>
	public T GetNode(string path) => Handler.GetNode(Root, path);

	/// <inheritdoc cref="GetNode(string)"/>
	public T GetNode(params string[] path) => Handler.GetNode(Root, path);

	/// <inheritdoc cref="GetNode(string)"/>
	public bool TryGetNode(string path, [NotNullWhen(true)] out T? result)
	{
		return Validator.Try(() => GetNode(path), out result);
	}

	/// <inheritdoc cref="GetNode(string[])"/>
	public bool TryGetNode(string[] path, [NotNullWhen(true)] out T? node)
	{
		return Validator.Try(() => GetNode(path), out node);
	}

	T ITreeHandler<T>.GetRoot(T node)
		=> this.Handler.GetRoot(node);

	bool ITreeHandler<T>.TryGetParent(T node, [NotNullWhen(true)] out T? parent)
		=> this.Handler.TryGetParent(node, out parent);

	int ITreeHandler<T>.GetDepth(T node)
		=> this.Handler.GetDepth(node);

	string ITreeHandler<T>.GetName(T node)
		=> this.Handler.GetName(node);

	int ITreeHandler<T>.GetChildCount(T node)
		=> this.Handler.GetChildCount(node);

	T ITreeHandler<T>.GetChildByIndex(T node, int index)
		=> this.Handler.GetChildByIndex(node, index);
}
