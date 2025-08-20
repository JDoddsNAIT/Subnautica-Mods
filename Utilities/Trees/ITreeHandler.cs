namespace FrootLuips.Subnautica.Trees;
/// <summary>
/// Generic implementation of <see cref="ITreeHandler{T}"/> for reference types.
/// </summary>
/// <typeparam name="T"></typeparam>
public class TreeHandler<T> : ITreeHandler<T> where T : class
{
	/// <inheritdoc cref="GetParentDelegate"/>
	public required GetParentDelegate GetParent { get; set; }
	/// <inheritdoc cref="SetParentDelegate"/>
	public required SetParentDelegate SetParent { get; set; }
	/// <inheritdoc cref="GetNameDelegate"/>
	public required GetNameDelegate GetName { get; set; }
	/// <inheritdoc cref="GetChildCountDelegate"/>
	public required GetChildCountDelegate GetChildCount { get; set; }
	/// <inheritdoc cref="GetChildByIndexDelegate"/>
	public required GetChildByIndexDelegate GetChildByIndex { get; set; }

	Tree<T>.Node ITreeHandler<T>.GetChild(T value, int index)
	{
		return new Tree<T>.Node(this.GetChildByIndex(value, index), this);
	}

	int ITreeHandler<T>.GetChildCount(T value)
	{
		return this.GetChildCount(value);
	}

	string ITreeHandler<T>.GetName(T value)
	{
		return this.GetName(value);
	}

	Tree<T>.Node? ITreeHandler<T>.GetParent(T value)
	{
		return this.GetParent(value, out var parent) ? new Tree<T>.Node(parent!, this) : null;
	}

	void ITreeHandler<T>.SetParent(T value, Tree<T>.Node? parent)
	{
		this.SetParent(value, parent?.Value);
	}

	/// <summary>
	/// Gets the <paramref name="parent"/> object of a <typeparamref name="T"/> <paramref name="value"/>.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="parent"></param>
	/// <returns></returns>
	public delegate bool GetParentDelegate(T value, out T? parent);
	/// <summary>
	/// Sets the <paramref name="parent"/> object for a <typeparamref name="T"/> <paramref name="value"/>.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="parent"></param>
	public delegate void SetParentDelegate(T value, T? parent);
	/// <summary>
	/// Get the name of a <typeparamref name="T"/> <paramref name="value"/>.
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	public delegate string GetNameDelegate(T value);
	/// <summary>
	/// Gets number of <typeparamref name="T"/> objects with <paramref name="value"/> as their parent.
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	public delegate int GetChildCountDelegate(T value);
	/// <summary>
	/// Gets the child of a <typeparamref name="T"/> <paramref name="value"/> at the specified <paramref name="index"/>.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="index"></param>
	/// <returns></returns>
	public delegate T GetChildByIndexDelegate(T value, int index);
}

/// <summary>
/// Defines how the parent, name, and children of <typeparamref name="T"/> values are accessed within a <see cref="Tree{T}"/> structure.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ITreeHandler<T>
{
	/// <summary>
	/// Returns the parent object of a <typeparamref name="T"/> <paramref name="value"/>.
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	Tree<T>.Node? GetParent(T value);

	/// <summary>
	/// Set a <typeparamref name="T"/> <paramref name="value"/>'s <paramref name="parent"/> object.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="parent"></param>
	void SetParent(T value, Tree<T>.Node? parent);

	/// <summary>
	/// Gets the name of a <typeparamref name="T"/> <paramref name="value"/>.
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	string GetName(T value);

	/// <summary>
	/// Gets number of <typeparamref name="T"/> objects with <paramref name="value"/> as their parent.
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	int GetChildCount(T value);
	/// <summary>
	/// Gets the child of a <typeparamref name="T"/> <paramref name="value"/> at the specified <paramref name="index"/>.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="index"></param>
	/// <returns></returns>
	Tree<T>.Node GetChild(T value, int index);
}
