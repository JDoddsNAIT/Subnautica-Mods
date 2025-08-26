using System;

namespace FrootLuips.Subnautica.Trees;
/// <summary>
/// Defines how a search is conducted.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="Search">How and what nodes are searched.</param>
/// <param name="Inclusive">Should the first node be included in the search?</param>
/// <param name="MaxDepth">The maximum depth of the search. If not initialized, <see cref="TreeHelpers.MaxDepth"/> will be used.</param>
/// <param name="Predicate">Limits the search to nodes that meet this condition.</param>
public record class SearchOptions<T>(
	SearchMode Search,
	bool Inclusive = true,
	ushort? MaxDepth = null,
	Predicate<T>? Predicate = null)
{
	/// <summary>
	/// Should this <paramref name="node"/> be searched?
	/// </summary>
	/// <param name="node"></param>
	/// <param name="handler"></param>
	/// <returns></returns>
	public bool ShouldSearch(T node, ITreeHandler<T> handler)
	{
		return handler.GetDepth(node) <= (MaxDepth ?? TreeHelpers.MaxDepth)
			&& (Predicate == null || Predicate(node));
	}

	/// <summary>
	/// Creates a set of options from a <see cref="SearchMode"/>.
	/// </summary>
	/// <param name="search"></param>
	public static implicit operator SearchOptions<T>(SearchMode search) => new(search);
}

/// <summary>
/// Determines how and what nodes are searched.
/// </summary>
public enum SearchMode
{
	/// <summary>
	/// Breadth-First search through all descendants.
	/// </summary>
	BreadthFirst,
	/// <summary>
	/// Depth-First search through all descendants.
	/// </summary>
	DepthFirst,
	/// <summary>
	/// Simple search through all ancestors.
	/// </summary>
	Ancestors,
}
