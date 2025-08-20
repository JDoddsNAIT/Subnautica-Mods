using UnityEngine;

namespace FrootLuips.Subnautica.Trees.Handlers;
/// <summary>
/// Standard <see cref="ITreeHandler{T}"/> for <see cref="Transform"/>s.
/// </summary>
public sealed class TransformHandler : ITreeHandler<Transform>
{
	/// <summary>
	/// A static instance of this class.
	/// </summary>
	public static TransformHandler Main { get; } = new();

	private readonly ITreeHandler<Transform> _handler = new TreeHandler<Transform>() {
		GetParent = static (Transform v, out Transform? p) => (p = v.parent) != null,
		SetParent = static (v, p) => v.SetParent(p),
		GetName = static (v) => v.gameObject.name,
		GetChildCount = static (v) => v.childCount,
		GetChildByIndex = static (v, i) => v.GetChild(i),
	};

	/// <inheritdoc/>
	public Tree<Transform>.Node? GetParent(Transform value) => _handler.GetParent(value);

	/// <inheritdoc/>
	public void SetParent(Transform value, Tree<Transform>.Node? parent) => _handler.SetParent(value, parent);

	/// <inheritdoc/>
	public string GetName(Transform value) => _handler.GetName(value);

	/// <inheritdoc/>
	public int GetChildCount(Transform value) => _handler.GetChildCount(value);

	/// <inheritdoc/>
	public Tree<Transform>.Node GetChild(Transform value, int index) => _handler.GetChild(value, index);
}
