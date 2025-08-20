namespace FrootLuips.Subnautica.Trees;

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
