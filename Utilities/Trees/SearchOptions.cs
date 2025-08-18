using System;

namespace FrootLuips.Subnautica.Trees;
/// <summary>
/// Options for enumerating through <see cref="Tree{T}"/>s.
/// </summary>
public record struct SearchOptions<T>
{
	/// <summary>
	/// <inheritdoc cref="SearchMode"/>
	/// </summary>
	public required SearchMode Search { get; set; }
	/// <summary>
	/// The maximum depth of the search.
	/// </summary>
	public ushort? MaxDepth { get; set; }
	/// <summary>
	/// Limits the search to only nodes that meet this condition. The root node is always searched.
	/// </summary>
	/// <remarks>
	/// Children of nodes that don't meet this condition are excluded from the search.
	/// </remarks>
	public Predicate<Tree<T>.Node>? Predicate { get; set; }

	internal readonly bool ShouldSearch(Tree<T>.Node node)
	{
		return node.GetDepth() < (MaxDepth ?? TreeHelpers.MaxDepth) && (Predicate == null || Predicate(node));
	}
}

/// <summary>
/// Determines how the children of a <see cref="Tree{T}.Node"/> are searched.
/// </summary>
public enum SearchMode
{
	/// <summary>
	/// Breadth-First Search through all descendants.
	/// </summary>
	BreadthFirst,
	/// <summary>
	/// Depth-First Search through all descendants.
	/// </summary>
	DepthFirst
}

