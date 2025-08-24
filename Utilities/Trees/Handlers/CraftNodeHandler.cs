using System.Diagnostics.CodeAnalysis;

namespace FrootLuips.Subnautica.Trees.Handlers;
/// <summary>
/// Standard <see cref="ITreeHandler{T}"/> for <see cref="global::TreeNode"/>s.
/// </summary>
public class CraftNodeHandler : Singleton<CraftNodeHandler>, ITreeHandler<global::TreeNode>
{
	/// <inheritdoc/>
	public TreeNode GetRoot(TreeNode node)
	{
		return node.root;
	}

	/// <inheritdoc/>
	public bool TryGetParent(TreeNode node, [NotNullWhen(true)] out TreeNode? parent)
	{
		return (parent = node.parent) != null;
	}

	/// <inheritdoc/>
	public int GetDepth(TreeNode node)
	{
		return node.depth;
	}

	/// <inheritdoc/>
	public string GetName(TreeNode node)
	{
		return node.id;
	}

	/// <inheritdoc/>
	public int GetChildCount(TreeNode node)
	{
		return node.childCount;
	}

	/// <inheritdoc/>
	public TreeNode GetChildByIndex(TreeNode node, int index)
	{
		return node[index];
	}
}
