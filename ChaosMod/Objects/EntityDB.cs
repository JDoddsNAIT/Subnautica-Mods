using FrootLuips.ChaosMod.Utilities;
using UnityEngine;

namespace FrootLuips.ChaosMod.Objects;
internal static class EntityDB<T> where T : Component
{
	private static readonly List<T> _entities = new();
	public static IReadOnlyList<T> Entities => _entities;
	public static int Count => _entities.Count;

	public static bool Register(T entity)
	{
		return _entities.TryAdd(entity);
	}

	public static void Update()
	{
		SimpleQueries.FilterNulls(_entities);
	}

	public static bool Deregister(T entity)
	{
		return _entities.Remove(entity);
	}
}

internal static class EntityDB
{
	public static bool Register<T>(T entity) where T : Component
	{
		return EntityDB<T>.Register(entity);
	}

	public static bool Deregister<T>(T entity) where T : Component
	{
		return EntityDB<T>.Deregister(entity);
	}
}
