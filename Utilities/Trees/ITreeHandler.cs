using System;

namespace FrootLuips.Subnautica.Trees;
/// <summary>
/// Default implementation of <see cref="ITreeHandler{T}"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public class TreeHandler<T> : ITreeHandler<T> where T : class
{
	/// <summary>
	/// <inheritdoc cref="ITreeHandler{T}.GetParent(T)"/>
	/// </summary>
	public required Func<T, T?> GetParent { get; set; }
	/// <summary>
	/// <inheritdoc cref="ITreeHandler{T}.SetParent(T, Node{T}?)"/>
	/// </summary>
	public required Action<T, T?> SetParent { get; set; }

	/// <summary>
	/// <inheritdoc cref="ITreeHandler{T}.GetName(T)"/>
	/// </summary>
	public required Func<T, string> GetName { get; set; }

	/// <summary>
	/// <inheritdoc cref="ITreeHandler{T}.GetChildCount(T)"/>
	/// </summary>
	public required Func<T, int> GetChildCount { get; set; }
	/// <summary>
	/// <inheritdoc cref="ITreeHandler{T}.GetChild(T, int)"/>
	/// </summary>
	public required Func<T, int, T> GetChild { get; set; }

	Node<T>? ITreeHandler<T>.GetParent(T value)
	{
		var parent = GetParent(value);
		return parent == null ? null : new(GetParent(value)!, this);
	}

	void ITreeHandler<T>.SetParent(T value, Node<T>? parent) => SetParent(value, parent?.Value);

	string ITreeHandler<T>.GetName(T value) => GetName(value);

	Node<T> ITreeHandler<T>.GetChild(T value, int index) => new(GetChild(value, index), this);
	int ITreeHandler<T>.GetChildCount(T value) => GetChildCount(value);
}

/// <summary>
/// Defines how the parent, name, and children of <typeparamref name="T"/> values are accessed within a <see cref="Tree{T}"/> structure.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ITreeHandler<T> where T : class
{
	/// <summary>
	/// Returns the parent object of a <typeparamref name="T"/> <paramref name="value"/>.
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	Node<T>? GetParent(T value);

	/// <summary>
	/// Set a <typeparamref name="T"/> <paramref name="value"/>'s <paramref name="parent"/> object.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="parent"></param>
	void SetParent(T value, Node<T>? parent);

	/// <summary>
	/// Gets the name of a <typeparamref name="T"/> <paramref name="value"/>.
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	string GetName(T value);

	/// <summary>
	/// Returns the amount of children for a <typeparamref name="T"/> <paramref name="value"/>.
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	int GetChildCount(T value);
	/// <summary>
	/// Returns the child at the given <paramref name="index"/> for a <typeparamref name="T"/> <paramref name="value"/>.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="index"></param>
	/// <returns></returns>
	Node<T> GetChild(T value, int index);
}
