using System;

namespace FrootLuips.Subnautica.Trees;
public partial class Tree<T>
{
	/// <summary>
	/// Options for enumerating through <see cref="Tree{T}"/>s.
	/// </summary>
	public record class SearchOptions
	{
		/// <summary>
		/// <inheritdoc cref="SearchMode"/>
		/// </summary>
		public required SearchMode Search { get; set; }
		/// <summary>
		/// Should the starting node be excluded from the search?
		/// </summary>
		public bool IncludeStart { get; set; } = true;
		/// <summary>
		/// The maximum depth of the search.
		/// </summary>
		public ushort? MaxDepth { get; set; } = null;
		/// <summary>
		/// Limits the search to only nodes that meet this condition. The starting node is always searched.
		/// </summary>
		public Predicate<Node>? Predicate { get; set; } = null;
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
	DepthFirst,
	/// <summary>
	/// Simple search through all ancestors.
	/// </summary>
	Ancestors,
}

