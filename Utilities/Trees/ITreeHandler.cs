using System.Diagnostics.CodeAnalysis;

namespace FrootLuips.Subnautica.Trees;
/// <summary>
/// Defines how the parent, name, and children of <typeparamref name="T"/> values are accessed.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ITreeHandler<T>
{
	/// <summary>
	/// Gets the root object of a <paramref name="node"/>.
	/// </summary>
	/// <param name="node"></param>
	/// <returns></returns>
	T GetRoot(T node);
	/// <summary>
	/// Gets the <paramref name="parent"/> object of a <typeparamref name="T"/> <paramref name="node"/>.
	/// </summary>
	/// <param name="node"></param>
	/// <param name="parent">The parent object. This value is not null if this method returns <see langword="true"/>.</param>
	/// <returns><see langword="true"/> if the <paramref name="node"/> has a parent.</returns>
	bool TryGetParent(T node, [NotNullWhen(true)] out T? parent);
	/// <summary>
	/// Gets the ancestors of a <paramref name="node"/>.
	/// </summary>
	/// <param name="node"></param>
	/// <returns></returns>
	int GetDepth(T node);
	/// <summary>
	/// Gets the name of a <paramref name="node"/>.
	/// </summary>
	/// <param name="node"></param>
	/// <returns></returns>
	string GetName(T node);
	/// <summary>
	/// Gets the number of objects with the given <paramref name="node"/> as it's parent.
	/// </summary>
	/// <param name="node"></param>
	/// <returns></returns>
	int GetChildCount(T node);
	/// <summary>
	/// Gets the child of a <paramref name="node"/> at the specified <paramref name="index"/>.
	/// </summary>
	/// <param name="node"></param>
	/// <param name="index"></param>
	/// <returns></returns>
	T GetChild(T node, int index);
}
