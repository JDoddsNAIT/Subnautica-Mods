using System.Diagnostics.CodeAnalysis;

namespace FrootLuips.Subnautica.Trees;

/// <summary>
/// Generic implementation of <see cref="ITreeHandler{T}"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public class TreeHandler<T> : ITreeHandler<T>
{
	/// <inheritdoc cref="GetParentDelegate"/>
	public required GetParentDelegate TryGetParent { get; set; }
	/// <inheritdoc cref="GetNameDelegate"/>
	public required GetNameDelegate GetName { get; set; }
	/// <inheritdoc cref="GetChildCountDelegate"/>
	public required GetChildCountDelegate GetChildCount { get; set; }
	/// <inheritdoc cref="GetChildByIndexDelegate"/>
	public required GetChildByIndexDelegate GetChildByIndex { get; set; }

	/// <inheritdoc cref="GetRootDelegate"/>
	public GetRootDelegate? GetRoot { get; set; } = null;
	/// <inheritdoc cref="GetDepthDelegate"/>
	public GetDepthDelegate? GetDepth { get; set; } = null;

	T ITreeHandler<T>.GetChild(T node, int index)
	{
		return GetChildByIndex(node, index);
	}

	int ITreeHandler<T>.GetChildCount(T node)
	{
		return GetChildCount(node);
	}

	string ITreeHandler<T>.GetName(T node)
	{
		return GetName(node);
	}

	int ITreeHandler<T>.GetDepth(T node)
	{
		return GetDepth == null ? TreeHelpers.GetDepth(node, this) : GetDepth(node);
	}

	bool ITreeHandler<T>.TryGetParent(T node, [NotNullWhen(true)] out T? parent)
	{
		return TryGetParent(node, out parent);
	}

	T ITreeHandler<T>.GetRoot(T node)
	{
		return GetRoot == null ? TreeHelpers.GetRoot(node, this) : GetRoot(node);
	}

	/// <inheritdoc cref="ITreeHandler{T}.GetRoot(T)"/>
	public delegate T GetRootDelegate(T node);
	
	/// <inheritdoc cref="ITreeHandler{T}.TryGetParent(T, out T)"/>
	public delegate bool GetParentDelegate(T node, [NotNullWhen(true)] out T? parent);
	
	/// <inheritdoc cref="ITreeHandler{T}.GetName(T)"/>
	public delegate string GetNameDelegate(T node);

	/// <inheritdoc cref="ITreeHandler{T}.GetDepth(T)"/>
	public delegate int GetDepthDelegate(T node);
	
	/// <inheritdoc cref="ITreeHandler{T}.GetChildCount(T)"/>
	public delegate int GetChildCountDelegate(T node);
	
	/// <inheritdoc cref="ITreeHandler{T}.GetChild(T, int)"/>
	public delegate T GetChildByIndexDelegate(T node, int index);
}
