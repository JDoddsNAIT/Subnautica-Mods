using System;

namespace FrootLuips.Subnautica.Trees;
/// <summary>
/// Represents a single node in a <see cref="Tree{T}"/> structure.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ITreeNode<T>
{
	/// <summary>
	/// Gets or sets the parent of this node.
	/// </summary>
	Tree<T>.Node? Parent { get; set; }

	/// <summary>
	/// The name of this node. (Read only)
	/// </summary>
	/// <remarks>
	/// Ideally a node's name should be unique across siblings.
	/// </remarks>
	string Name { get; }
	/// <summary>
	/// The node's underlying <typeparamref name="T"/> value. (Read only)
	/// </summary>
	T Value { get; }

	/// <summary>
	/// The number of children this node has. (Read only)
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
