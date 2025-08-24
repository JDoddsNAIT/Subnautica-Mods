using System.Diagnostics.CodeAnalysis;

namespace FrootLuips.Subnautica.Trees;
/// <summary>
/// Template interface for nodes of a <see cref="Tree{T}"/> structure. Comes with a pre-built <see cref="ITreeHandler{T}"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ITreeNode<out T> where T : class, ITreeNode<T>
{
	/// <summary>
	/// The root object.
	/// </summary>
	T Root { get; }
	/// <summary>
	/// The parent object.
	/// </summary>
	T? Parent { get; }
	/// <summary>
	/// The number of ancestors on this object.
	/// </summary>
	int Depth { get; }

	/// <summary>
	/// The name of this object.
	/// </summary>
	string Name { get; }
	
	/// <summary>
	/// The number of objects with <see langword="this"/> as it's <see cref="Parent"/>.
	/// </summary>
	int ChildCount { get; }
	/// <summary>
	/// Gets the child at the specified <paramref name="index"/>.
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	T this[int index] { get; }
}

/// <summary>
/// Standard <see cref="ITreeHandler{T}"/> for reference types that implement <see cref="ITreeNode{T}"/>
/// </summary>
/// <remarks>
/// This class cannot be inherited.
/// </remarks>
/// <typeparam name="T"></typeparam>
public sealed class TreeNodeHandler<T> : ITreeHandler<T>
	where T : class, ITreeNode<T>
{
	/// <summary>
	/// Static instance of this class.
	/// </summary>
	public static TreeNodeHandler<T> Main => Singleton<TreeNodeHandler<T>>.Main;

	/// <inheritdoc/>
	public T GetRoot(T node)
	{
		return node.Root;
	}

	/// <inheritdoc/>
	public bool TryGetParent(T node, [NotNullWhen(true)] out T? parent)
	{
		return (parent = node.Parent) != null;
	}

	/// <inheritdoc/>
	public int GetDepth(T node)
	{
		return node.Depth;
	}

	/// <inheritdoc/>
	public string GetName(T node)
	{
		return node.Name;
	}

	/// <inheritdoc/>
	public int GetChildCount(T node)
	{
		return node.ChildCount;
	}

	/// <inheritdoc/>
	public T GetChildByIndex(T node, int index)
	{
		return node[index];
	}
}
