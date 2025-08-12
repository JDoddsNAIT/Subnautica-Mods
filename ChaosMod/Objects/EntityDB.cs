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

	public static bool Deregister(T entity)
	{
		return _entities.Remove(entity);
	}

	public static void GetEntities(ref T[] values)
	{
		if (values.Length != Count)
			Array.Resize(ref values, Count);

		for (int i = 0; i < Count; i++)
		{
			values[i] = _entities[i];
		}
	}
}
