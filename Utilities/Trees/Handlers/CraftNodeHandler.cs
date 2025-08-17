namespace FrootLuips.Subnautica.Trees.Handlers;
/// <summary>
/// Standard <see cref="ITreeHandler{T}"/> for <see cref="global::TreeNode"/>s.
/// </summary>
public class CraftNodeHandler : ITreeHandler<global::TreeNode>
{
	/// <summary>
	/// A static instance of this class.
	/// </summary>
	public static CraftNodeHandler Main { get; } = new();

	/// <inheritdoc/>
	public Tree<TreeNode>.Node GetChild(TreeNode value, int index) => new(value[index], this);

	/// <inheritdoc/>
	public int GetChildCount(TreeNode value) => value.childCount;

	/// <inheritdoc/>
	public string GetName(TreeNode value) => value.id;

	/// <inheritdoc/>
	public Tree<TreeNode>.Node? GetParent(TreeNode value) => value.parent == null ? null : new(value.parent, this);

	/// <inheritdoc/>
	public void SetParent(TreeNode value, Tree<TreeNode>.Node? parent) => value.parent = parent;
}
