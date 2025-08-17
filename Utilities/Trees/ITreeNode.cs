using System;

namespace FrootLuips.Subnautica.Trees;
/// <summary>
/// Represents a single node in a <see cref="Tree{T}"/> structure.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ITreeNode<T> where T : class
{
	/// <summary>
	/// The parent of this node.
	/// </summary>
	Node<T>? Parent { get; set; }

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
	Node<T> GetChild(int childIndex);
}

/// <summary>
/// Default <see cref="ITreeHandler{T}"/> for types that implement <see cref="ITreeNode{T}"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class TreeNodeHandler<T> : ITreeHandler<T> where T : class, ITreeNode<T>
{
	/// <summary>
	/// A static instance of this class.
	/// </summary>
	public static readonly TreeNodeHandler<T> Default = new();

	/// <inheritdoc/>
	public Node<T> GetChild(T value, int index) => value.GetChild(index);

	/// <inheritdoc/>
	public int GetChildCount(T value) => value.ChildCount;

	/// <inheritdoc/>
	public string GetName(T value) => value.Name;

	/// <inheritdoc/>
	public Node<T>? GetParent(T value) => value.Parent;

	/// <inheritdoc/>
	public void SetParent(T value, Node<T>? parent) => value.Parent = parent;
}
