using System.Diagnostics.CodeAnalysis;
using FrootLuips.Subnautica.Helpers;
using UnityEngine;

namespace FrootLuips.Subnautica.Trees.Handlers;
/// <summary>
/// Standard <see cref="ITreeHandler{T}"/> for <see cref="Transform"/>s.
/// </summary>
public sealed class TransformHandler : Singleton<TransformHandler>, ITreeHandler<Transform>
{
	private static readonly ITreeHandler<Transform> _handler = new TreeHandler<Transform>() {
		GetRoot = static (n) => n.root,
		TryGetParent = static (Transform n, [NotNullWhen(true)] out Transform? p) => (p = n.parent) != null,
		GetName = static (n) => n.gameObject.name,
		GetChildCount = static (n) => n.childCount,
		GetChildByIndex = static (n, i) => n.GetChild(i),
	};

	/// <inheritdoc/>
	public Transform GetRoot(Transform node)
	{
		return _handler.GetRoot(node);
	}

	/// <inheritdoc/>
	public bool TryGetParent(Transform node, [NotNullWhen(true)] out Transform? parent)
	{
		return _handler.TryGetParent(node, out parent);
	}

	/// <inheritdoc/>
	public int GetDepth(Transform node)
	{
		return _handler.GetDepth(node);
	}

	/// <inheritdoc/>
	public string GetName(Transform node)
	{
		return _handler.GetName(node);
	}

	/// <inheritdoc/>
	public int GetChildCount(Transform node)
	{
		return _handler.GetChildCount(node);
	}

	/// <inheritdoc/>
	public Transform GetChildByIndex(Transform node, int index)
	{
		return _handler.GetChildByIndex(node, index);
	}
}
