using UnityEngine;

namespace FrootLuips.Subnautica.Trees.Handlers;
/// <summary>
/// Standard <see cref="ITreeHandler{T}"/> for <see cref="Transform"/>s and <see cref="GameObject"/>s.
/// </summary>
public sealed class TransformHandler : ITreeHandler<Transform>, ITreeHandler<GameObject>
{
	/// <summary>
	/// Static instance of this class.
	/// </summary>
	public static TransformHandler Main { get; } = new();

	/// <inheritdoc/>
	public Tree<Transform>.Node GetChild(Transform value, int index)
		=> new(value.GetChild(index), this);

	/// <inheritdoc/>
	public int GetChildCount(Transform value)
		=> value.childCount;

	/// <inheritdoc/>
	public string GetName(Transform value)
		=> value.gameObject.name;

	/// <inheritdoc/>
	public Tree<Transform>.Node? GetParent(Transform value)
		=> new(value.parent, this);

	/// <inheritdoc/>
	public void SetParent(Transform value, Tree<Transform>.Node? parent)
		=> value.SetParent(parent?.Value);

	/// <inheritdoc/>
	public Tree<GameObject>.Node GetChild(GameObject value, int index)
		=> new(value.transform.GetChild(index).gameObject, this);

	/// <inheritdoc/>
	public int GetChildCount(GameObject value)
		=> value.transform.childCount;

	/// <inheritdoc/>
	public string GetName(GameObject value)
		=> value.name;

	/// <inheritdoc/>
	public Tree<GameObject>.Node? GetParent(GameObject value)
		=> new(value.transform.parent.gameObject, this);

	/// <inheritdoc/>
	public void SetParent(GameObject value, Tree<GameObject>.Node? parent)
		=> value.transform.SetParent(parent?.Value.transform);

	/// <summary>
	/// Creates a new <see cref="Tree{T}"/> with the default handler.
	/// </summary>
	/// <param name="root"></param>
	/// <returns></returns>
	public static Tree<Transform> CreateTree(Transform root)
	{
		return new Tree<Transform>(root, Main);
	}
}
