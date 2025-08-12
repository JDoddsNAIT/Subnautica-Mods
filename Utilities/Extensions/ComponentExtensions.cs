#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FrootLuips.Subnautica.Extensions;

public enum HeirarchyPreference { Children, Parents }

public static class ComponentExtensions
{
	internal static List<ComponentQuery> QueryPool { get; } = new(capacity: 2);
	internal static ComponentQuery GetQuery(GameObject go)
	{
		for (int i = 0; i < QueryPool.Count; i++)
		{
			if (QueryPool[i].gameObject == null)
			{
				QueryPool[i].gameObject = go;
				return QueryPool[i];
			}
		}

		var q = new ComponentQuery() {
			gameObject = go
		};
		QueryPool.Add(q);
		return q;
	}

	/// <summary>
	/// Begins a component query for a <paramref name="gameObject"/>.
	/// </summary>
	/// <param name="gameObject"></param>
	/// <returns></returns>
	public static ComponentQuery Get(this GameObject gameObject) => GetQuery(gameObject);
	/// <summary>
	/// Begins a component query for a <paramref name="gameObject"/>.
	/// </summary>
	/// <param name="component"></param>
	/// <returns></returns>
	public static ComponentQuery Get(this Component component) => GetQuery(component.gameObject);

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

	public static IEnumerable<T> EnumerateComponentsInChildren<T>(this GameObject obj,
		bool includeInactive = false)
	{
		Transform current = obj.transform;
		List<T> components = new();
		var indices = new List<ushort?>() { null };
		int depth = 0;

		do
		{
			bool currentActive = current.gameObject == obj || includeInactive || current.gameObject.activeSelf;
			if (indices[depth] == null)
			{
				indices[depth] = 0;
				if (currentActive)
				{
					components.Clear();
					current.GetComponents(components);
					for (int i = 0; i < components.Count; i++)
					{
						yield return components[i];
					}
				}
			}
			else
			{
				indices[depth]++;
			}

			if (currentActive && indices[depth] < current.childCount)
			{
				// Descend
				current = current.GetChild((int)indices[depth]!);
				depth++;

				if (depth >= indices.Count)
					indices.Add(null);
				else
					indices[depth] = null;
			}
			else
			{
				// Ascend
				depth--;
				current = current.parent;
			}
		} while (depth >= 0);
	}
}

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
		/// <param name="component"></param>
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
		/// <inheritdoc cref="InHeirarchy(bool)"/>
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
