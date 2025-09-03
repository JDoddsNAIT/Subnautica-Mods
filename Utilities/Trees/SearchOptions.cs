using System;
using System.Diagnostics.CodeAnalysis;

namespace FrootLuips.Subnautica.Trees;
/// <summary>
/// Defines how a search is conducted.
/// </summary>
public record class SearchOptions<T> where T : class
{
	/// <summary>
	/// How and what nodes are searched.
	/// </summary>
	public required SearchMode Search { get; set; }
	/// <summary>
	/// Should the first node be included in the search?
	/// </summary>
	public bool Inclusive { get; set; } = true;
	/// <summary>
	/// The maximum depth of the search. If not initialized, <see cref="TreeHelpers.MaxDepth"/> will be used.
	/// </summary>
	public ushort? MaxDepth { get; set; } = null;
	/// <summary>
	/// Limits the search to nodes that meet this condition.
	/// </summary>
	public Predicate<T>? Predicate { get; set; } = null;

	/// <inheritdoc cref="SearchOptions{T}"/>
	public SearchOptions() { }

	/// <summary>
	/// <inheritdoc cref="SearchOptions{T}"/>
	/// </summary>
	/// <param name="search"> How and what nodes are searched.</param>
	/// <param name="inclusive">Should the first node be included in the search?</param>
	/// <param name="maxDepth">The maximum depth of the search. If not initialized, <see cref="TreeHelpers.MaxDepth"/> will be used.</param>
	/// <param name="predicate">Limits the search to nodes that meet this condition.</param>
	[SetsRequiredMembers]
	public SearchOptions(SearchMode search, bool inclusive = true, ushort? maxDepth = null, Predicate<T>? predicate = null)
	{
		this.Search = search;
		this.Inclusive = inclusive;
		this.MaxDepth = maxDepth;
		this.Predicate = predicate;
	}

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
