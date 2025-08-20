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

	private readonly ITreeHandler<global::TreeNode> _handler = new TreeHandler<global::TreeNode>() {
		GetParent = static (global::TreeNode v, out global::TreeNode? p) => (p = v.parent) != null,
		GetName = static (v) => v.id,
		GetChildCount = static (v) => v.childCount,
		GetChildByIndex = static (v, i) => v[i],
	};

	/// <inheritdoc/>
	public Tree<TreeNode>.Node? GetParent(TreeNode value)
	{
		return _handler.GetParent(value);
	}

	/// <inheritdoc/>
	public string GetName(TreeNode value)
	{
		return _handler.GetName(value);
	}

	/// <inheritdoc/>
	public int GetChildCount(TreeNode value)
	{
		return _handler.GetChildCount(value);
	}

	/// <inheritdoc/>
	public Tree<TreeNode>.Node GetChild(TreeNode value, int index)
	{
		return _handler.GetChild(value, index);
	}
}
