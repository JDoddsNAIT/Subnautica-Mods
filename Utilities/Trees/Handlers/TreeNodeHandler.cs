namespace FrootLuips.Subnautica.Trees.Handlers;
/// <summary>
/// Default <see cref="ITreeHandler{T}"/> for types that implement <see cref="ITreeNode{T}"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class TreeNodeHandler<T> : ITreeHandler<T> where T : ITreeNode<T>
{
	/// <summary>
	/// A static instance of this class.
	/// </summary>
	public static TreeNodeHandler<T> Main { get; } = new();

	/// <inheritdoc/>
	public Tree<T>.Node GetChild(T value, int index) => value.GetChild(index);

	/// <inheritdoc/>
	public int GetChildCount(T value) => value.ChildCount;

	/// <inheritdoc/>
	public string GetName(T value) => value.Name;

	/// <inheritdoc/>
	public Tree<T>.Node? GetParent(T value) => value.Parent;

	/// <inheritdoc/>
	public void SetParent(T value, Tree<T>.Node? parent) => value.Parent = parent;
}
