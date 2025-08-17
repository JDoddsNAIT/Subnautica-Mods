using System;
using System.Collections.Generic;
using FrootLuips.Subnautica.Trees.Handlers;

namespace FrootLuips.Subnautica.Trees;

/// <summary>
/// Provides a default implementation of <see cref="ITreeNode{T}"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class TreeNode<T> : ITreeNode<T> where T : TreeNode<T>
{
	private Tree<T>.Node? _parent = null;
	private readonly List<Tree<T>.Node> _children = new();

	/// <inheritdoc/>
	public Tree<T>.Node? Parent {
		get => _parent;
		set {
			if (_parent == value)
				return;

			if (_parent.HasValue)
			{
				_parent?.Value.RemoveChild(Value);
			}

			_parent = value;
			
			if (value.HasValue)
			{
				value?.Value.AddChild(Value);
			}
		}
	}

	/// <inheritdoc/>
	public string Name { get; }
	/// <inheritdoc/>
	public T Value { get; }

	/// <inheritdoc/>
	public int ChildCount => _children.Count;

	/// <inheritdoc cref="ITreeNode{T}.GetChild(int)"/>
	public Tree<T>.Node this[int childIndex] => GetChild(childIndex);

	/// <summary>
	/// Constructs a new <see cref="TreeNode{T}"/> with the given <paramref name="name"/> and <paramref name="value"/>, along with a <paramref name="parent"/> if specified.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value"></param>
	/// <param name="parent"></param>
	protected TreeNode(string name, T value, Tree<T>.Node? parent = null)
	{
		this.Name = name;
		this.Value = value;
		this.Parent = parent;
	}

	/// <inheritdoc/>
	public Tree<T>.Node GetChild(int childIndex) => _children[childIndex];

	private void AddChild(T child)
	{
		_children.Add(new(child, TreeNodeHandler<T>.Main));
	}

	private void RemoveChild(T child)
	{
		bool removed = false;
		for (int i = 0; i < _children.Count && !removed; i++)
		{
			if (_children[i].Value == child)
			{
				_children.RemoveAt(i);
				removed = true;
			}
		}
	}
}

/// <summary>
/// Represents a single node in a <see cref="Tree{T}"/> structure.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ITreeNode<T> where T : class
{
	/// <summary>
	/// The parent of this node.
	/// </summary>
	Tree<T>.Node? Parent { get; set; }

	/// <summary>
	/// The nodes name. Ideally this should be unique across this node's siblings.
	/// </summary>
	string Name { get; }
	/// <summary>
	/// The node's underlying <typeparamref name="T"/> value.
	/// </summary>
	T Value { get; }

	/// <summary>
	/// The number of children this node has.
	/// </summary>
	int ChildCount { get; }
	/// <summary>
	/// Gets the child at the specified <paramref name="childIndex"/>.
	/// </summary>
	/// <param name="childIndex"></param>
	/// <returns></returns>
	/// <exception cref="IndexOutOfRangeException"></exception>
	Tree<T>.Node GetChild(int childIndex);
}
