using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace FrootLuips.Subnautica.Helpers;
/// <summary>
/// Uses the overloaded '==' operator to compare two unity objects.
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class UnityObjectComparer<T> : Singleton<UnityObjectComparer<T>>, IEqualityComparer<T>
	where T : Object
{
	/// <inheritdoc/>
	public bool Equals(T? x, T? y)
	{
		return (x, y) switch {
			(null, null) => true,
			(not null, null) => x == null,
			(null, not null) => y == null,
			(not null, not null) => x == y,
		};
	}

	/// <inheritdoc/>
	public int GetHashCode(T obj)
	{
		return obj.GetHashCode();
	}
}
