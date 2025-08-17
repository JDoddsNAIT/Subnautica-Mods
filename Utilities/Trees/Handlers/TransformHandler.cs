using UnityEngine;

namespace FrootLuips.Subnautica.Trees.Handlers;
/// <summary>
/// Standard <see cref="ITreeHandler{T}"/> for <see cref="Transform"/>s and <see cref="GameObject"/>s.
/// </summary>
public sealed class TransformHandler : ITreeHandler<Transform>, ITreeHandler<GameObject>
{
	/// <inheritdoc/>
	public Node<Transform> GetChild(Transform value, int index)
		=> new(value.GetChild(index), this);

	/// <inheritdoc/>
	public int GetChildCount(Transform value)
		=> value.childCount;

	/// <inheritdoc/>
	public string GetName(Transform value)
		=> value.gameObject.name;

	/// <inheritdoc/>
	public Node<Transform>? GetParent(Transform value)
		=> new(value.parent, this);

	/// <inheritdoc/>
	public void SetParent(Transform value, Node<Transform>? parent)
		=> value.SetParent(parent?.Value);

	/// <inheritdoc/>
	public Node<GameObject> GetChild(GameObject value, int index)
		=> new(value.transform.GetChild(index).gameObject, this);

	/// <inheritdoc/>
	public int GetChildCount(GameObject value)
		=> value.transform.childCount;

	/// <inheritdoc/>
	public string GetName(GameObject value)
		=> value.name;

	/// <inheritdoc/>
	public Node<GameObject>? GetParent(GameObject value)
		=> new(value.transform.parent.gameObject, this);

	/// <inheritdoc/>
	public void SetParent(GameObject value, Node<GameObject>? parent)
		=> value.transform.SetParent(parent?.Value.transform);
}
