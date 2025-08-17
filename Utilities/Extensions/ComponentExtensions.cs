#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FrootLuips.Subnautica.Extensions;

/// <summary>
/// Determines which components should be processed first during a query.
/// </summary>
public enum HeirarchyPreference
{
	/// <summary>
	/// Components in parent objects are processed first.
	/// </summary>
	Parents,
	/// <summary>
	/// Components in child objects are processed first.
	/// </summary>
	Children,
}

/// <summary>
/// Extension method for <see cref="GameObject"/>s and <see cref="Component"/>s relating to getting other components.
/// </summary>
public static class ComponentExtensions
{
	internal static List<ComponentQuery> QueryPool { get; } = new(capacity: 2);
	internal static ComponentQuery GetQuery(GameObject go)
	{
		ComponentQuery query = null;

		for (int i = 0; i < QueryPool.Count && query == null; i++)
		{
			if (QueryPool[i].gameObject == null)
			{
				QueryPool[i].gameObject = go;
				query = QueryPool[i];
			}
		}

		if (query == null)
		{
			query = new ComponentQuery() {
				gameObject = go
			};
			QueryPool.Add(query);
		}
		return query;
	}

	/// <summary>
	/// Begins a component query for a <paramref name="gameObject"/>.
	/// </summary>
	/// <param name="gameObject"></param>
	/// <returns></returns>
	public static ComponentQuery Get(this GameObject gameObject) => GetQuery(gameObject);
	/// <summary>
	/// Begins a component query for a <paramref name="component"/>.
	/// </summary>
	/// <param name="component"></param>
	/// <returns></returns>
	public static ComponentQuery Get(this Component component) => GetQuery(component.gameObject);

	/// <summary>
	/// Enumerates over all components of type <typeparamref name="T"/> in a <see cref="GameObject"/> and it's parents.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="obj"></param>
	/// <param name="includeInactive">Should inactive objects be included in the enumeration? (this <see cref="GameObject"/> is always included, regardless of this value.)</param>
	/// <returns></returns>
	public static IEnumerable<T> EnumerateComponentsInParent<T>(this GameObject obj,
		bool includeInactive = false)
	{
		Transform current = obj.transform;
		List<T> components = new();
		while (current != null)
		{
			if (current.gameObject == obj || includeInactive || current.gameObject.activeSelf)
			{
				components.Clear();
				current.GetComponents(components);
				for (int i = 0; i < components.Count; i++)
				{
					yield return components[i];
				}
			}

			current = current.parent;
		}
	}

	/// <summary>
	/// Enumerates over all components of type <typeparamref name="T"/> in the children of a <see cref="GameObject"/> using depth-first search.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="obj"></param>
	/// <param name="includeInactive">Should inactive objects be included in the enumeration? (this <see cref="GameObject"/> is always included, regardless of this value.)</param>
	/// <returns></returns>
	public static IEnumerable<T> EnumerateComponentsInChildren<T>(this GameObject obj,
		bool includeInactive = false)
	{
		Stack<Transform> stack = new(capacity: 1);
		stack.Push(obj.transform);
		Transform current;
		List<T> components = new();

		do
		{
			current = stack.Pop();
			if (current != obj.transform && !includeInactive && !current.gameObject.activeSelf)
			{
				continue;
			}

			components.Clear();
			current.GetComponents(components);
			for (int i = 0; i < components.Count; i++)
			{
				yield return components[i];
			}

			for (int i = current.childCount - 1; i >= 0; i--)
			{
				stack.Push(current.GetChild(i));
			}
		}
		while (stack.Count > 0);
	}
}

/// <summary>
/// Represents a query for components relative to a <see cref="GameObject"/>.
/// </summary>
/// <remarks>
/// This class cannot be inherited, nor instantiated externally.
/// </remarks>
public sealed class ComponentQuery
{
	internal GameObject gameObject;
	internal ComponentQuery() { }
	internal void Release() => gameObject = null;

	/// <summary>
	/// <inheritdoc cref="Single{T}"/>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="where"></param>
	/// <returns></returns>
	public Single<T> Component<T>(Func<T, bool> where = null) => new(this) { predicate = where };
	/// <summary>
	/// Any component of type <typeparamref name="T"/>...
	/// </summary>
	/// <remarks>
	/// Represents a query for a single component.
	/// </remarks>
	/// <typeparam name="T"></typeparam>
	public struct Single<T>
	{
		internal ComponentQuery query;
		internal Func<T, bool> predicate = null;
		internal Single(ComponentQuery query) => this.query = query;

		/// <summary>
		/// ...on this <see cref="GameObject"/>.
		/// </summary>
		/// <returns></returns>
		public readonly T OnSelf()
		{
			T component = predicate switch {
				null => query.gameObject.GetComponent<T>(),
				_ => query.gameObject.GetComponents<T>().FirstOrDefault(predicate)
			};
			query.Release();
			return component;
		}
		/// <summary>
		/// <inheritdoc cref="OnSelf"/>
		/// </summary>
		/// <param name="component"></param>
		/// <returns><see langword="true"/> if <paramref name="component"/> is not <see langword="null"/>.</returns>
		public readonly bool TryOnSelf(out T component)
		{
			component = OnSelf();
			return component != null;
		}

		/// <summary>
		/// ...on this <see cref="GameObject"/> or one of it's ancestors.
		/// </summary>
		/// <param name="includeInactive">Should inactive objects be included in the query? This object will always be included.</param>
		/// <returns></returns>
		public readonly T InParent(bool includeInactive = false)
		{
			T component = predicate switch {
				null => query.gameObject.EnumerateComponentsInParent<T>(includeInactive).FirstOrDefault(),
				_ => query.gameObject.EnumerateComponentsInParent<T>(includeInactive).FirstOrDefault(predicate),
			};
			query.Release();
			return component;
		}
		/// <summary>
		/// <inheritdoc cref="InParent(bool)"/>
		/// </summary>
		/// <param name="component"></param>
		/// <param name="includeInactive">Should inactive objects be included in the query? This object will always be included.</param>
		/// <returns><inheritdoc cref="TryOnSelf(out T)"/></returns>
		public readonly bool TryInParent(out T component,
			bool includeInactive = false)
		{
			component = InParent(includeInactive);
			return component != null;
		}

		/// <summary>
		/// ...on this <see cref="GameObject"/> or any of it's children.
		/// </summary>
		/// <param name="includeInactive"></param>
		/// <returns></returns>
		public readonly T InChildren(bool includeInactive = false)
		{
			T component = predicate switch {
				null => query.gameObject.EnumerateComponentsInChildren<T>(includeInactive).FirstOrDefault(),
				_ => query.gameObject.EnumerateComponentsInChildren<T>(includeInactive).FirstOrDefault(predicate)
			};
			query.Release();
			return component;
		}
		/// <summary>
		/// <inheritdoc cref="InChildren(bool)"/>
		/// </summary>
		/// <param name="component"></param>
		/// <param name="includeInactive">Should inactive objects be included in the query? This object will always be included.</param>
		/// <returns><inheritdoc cref="TryOnSelf(out T)"/></returns>
		public readonly bool TryInChildren(out T component,
			bool includeInactive = false)
		{
			component = InChildren(includeInactive);
			return component != null;
		}

		private delegate bool TryGet(out T component, bool includeInactive);
		/// <summary>
		/// ...on this <see cref="GameObject"/>, one of it's ancestors, or any of it's children.
		/// </summary>
		/// <param name="includeInactive">Should inactive objects be included in the query? This object will always be included.</param>
		/// <param name="preference">Where to look for a component first.</param>
		/// <returns></returns>
		public readonly T InHeirarchy(bool includeInactive = false,
			HeirarchyPreference preference = HeirarchyPreference.Parents)
		{
			TryGet a = TryInParent, b = TryInChildren;
			if (preference != HeirarchyPreference.Parents) (a, b) = (b, a);
			if (!a(out T c, includeInactive))
				b(out c, includeInactive);
			query.Release();
			return c;
		}
		/// <summary>
		/// <inheritdoc cref="InHeirarchy(bool, HeirarchyPreference)"/>
		/// </summary>
		/// <param name="component"></param>
		/// <param name="includeInactive">Should inactive objects be included in the query? This object will always be included.</param>
		/// <param name="preference">Where to look for a component first.</param>
		/// <returns><inheritdoc cref="TryOnSelf(out T)"/></returns>
		public readonly bool TryInHeirarchy(out T component,
			bool includeInactive = false,
			HeirarchyPreference preference = HeirarchyPreference.Parents)
		{
			component = InHeirarchy(includeInactive, preference);
			return component != null;
		}
	}

	/// <summary>
	/// <inheritdoc cref="Multiple{T}"/>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="where"></param>
	/// <returns></returns>
	public Multiple<T> Components<T>(Func<T, bool> where = null) => new(this) { predicate = where };
	/// <summary>
	/// All components of type <typeparamref name="T"/>...
	/// </summary>
	/// <remarks>
	/// Represents a query for multiple components.
	/// </remarks>
	/// <typeparam name="T"></typeparam>
	public struct Multiple<T>
	{
		internal ComponentQuery query;
		internal Func<T, bool> predicate = null;
		internal Multiple(ComponentQuery query) => this.query = query;

		/// <summary>
		/// <inheritdoc cref="Single{T}.OnSelf"/>
		/// </summary>
		/// <returns></returns>
		public readonly T[] OnSelf()
		{
			IEnumerable<T> components = predicate switch {
				null => query.gameObject.GetComponents<T>(),
				_ => query.gameObject.GetComponents<T>().Where(predicate),
			};
			return components as T[] ?? components.ToArray();
		}
		/// <summary>
		/// <inheritdoc cref="OnSelf"/>
		/// </summary>
		/// <param name="components"></param>
		/// <returns><see langword="true"/> if <paramref name="components"/> has at least one value.</returns>
		public readonly bool TryOnSelf(out T[] components)
		{
			components = OnSelf();
			return components.Length > 0;
		}

		/// <summary>
		/// ...on this <see cref="GameObject"/> and it's ancestors.
		/// </summary>
		/// <param name="includeInactive">Should inactive objects be included in the query? This object will always be included.</param>
		/// <returns></returns>
		public readonly T[] InParent(bool includeInactive = false)
		{
			IEnumerable<T> components = predicate switch {
				null => query.gameObject.EnumerateComponentsInParent<T>(includeInactive),
				_ => query.gameObject.EnumerateComponentsInParent<T>(includeInactive).Where(predicate),
			};
			return components.ToArray();
		}
		/// <summary>
		/// <inheritdoc cref="InParent(bool)"/>
		/// </summary>
		/// <param name="components"></param>
		/// <param name="includeInactive">Should inactive objects be included in the query? This object will always be included.</param>
		/// <returns><inheritdoc cref="TryOnSelf(out T[])"/></returns>
		public readonly bool TryInParent(out T[] components,
			bool includeInactive = false)
		{
			components = InParent(includeInactive);
			return components.Length > 0;
		}

		/// <summary>
		/// ...on this <see cref="GameObject"/> and all it's children.
		/// </summary>
		/// <param name="includeInactive">Should inactive objects be included in the query? This object will always be included.</param>
		/// <returns></returns>
		public readonly T[] InChildren(bool includeInactive = false)
		{
			IEnumerable<T> components = predicate switch {
				null => query.gameObject.EnumerateComponentsInChildren<T>(includeInactive),
				_ => query.gameObject.EnumerateComponentsInChildren<T>(includeInactive).Where(predicate),
			};
			return components.ToArray();
		}
		/// <summary>
		/// <inheritdoc cref="InChildren(bool)"/>
		/// </summary>
		/// <param name="components"></param>
		/// <param name="includeInactive">Should inactive objects be included in the query? This object will always be included.</param>
		/// <returns><inheritdoc cref="TryOnSelf(out T[])"/></returns>
		public readonly bool TryInChildren(out T[] components,
			bool includeInactive = false)
		{
			components = InChildren(includeInactive);
			return components.Length > 0;
		}

		/// <summary>
		/// ...on this <see cref="GameObject"/>, it's ancestors, and all it's children.
		/// </summary>
		/// <param name="includeInactive">Should inactive objects be included in the query? This object will always be included.</param>
		/// <param name="preference">Which set of elements should be listed first.</param>
		/// <returns></returns>
		public readonly T[] InHeirarchy(bool includeInactive = false,
			HeirarchyPreference preference = HeirarchyPreference.Parents)
		{
			var builder = new ArrayBuilder<T>(capacity: 2);
			var a = InParent(includeInactive);
			var b = InChildren(includeInactive);
			if (preference != HeirarchyPreference.Parents) (a, b) = (b, a);
			var array = builder.Append(a).Append(b).ToArray();
			Queries.FilterDuplicates(ref array);
			return array;
		}
		/// <summary>
		/// <inheritdoc cref="InHeirarchy(bool, HeirarchyPreference)"/>
		/// </summary>
		/// <param name="components"></param>
		/// <param name="includeInactive">Should inactive objects be included in the query? This object will always be included.</param>
		/// <returns><inheritdoc cref="TryOnSelf(out T[])"/></returns>
		public readonly bool TryInHeirarchy(out T[] components,
			bool includeInactive = false)
		{
			components = InHeirarchy(includeInactive);
			return components.Length > 0;
		}
	}
}
