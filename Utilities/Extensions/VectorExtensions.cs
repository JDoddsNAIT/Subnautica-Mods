using UnityEngine;

namespace FrootLuips.Subnautica.Extensions;
/// <summary>
/// Extension methods for <see cref="Vector2"/>s and <see cref="Vector3"/>s.
/// </summary>
public static class VectorExtensions
{
	#region Vector2
	/// <summary>
	/// Rotates a <see cref="Vector2"/> by an <paramref name="angle"/> in degrees.
	/// </summary>
	/// <param name="vector"></param>
	/// <param name="angle">An angle in degrees.</param>
	/// <param name="pivot">The pivot point of the rotation. Defaults to <see cref="Vector2.zero"/>.</param>
	/// <returns>The rotated vector.</returns>
	public static Vector2 Rotate(this Vector2 vector, float angle, Vector2? pivot = null)
	{
		pivot ??= Vector2.zero;
		vector -= (Vector2)pivot;
		var rotation = Quaternion.Euler(0, 0, z: angle);
		vector = rotation * vector;
		vector += (Vector2)pivot;
		return vector;
	}

	/// <summary>
	/// Adds to a <see cref="Vector2"/> component-wise.
	/// </summary>
	/// <param name="vector"></param>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public static Vector2 Add(this Vector2 vector, float x = 0, float y = 0)
	{
		vector.x += x;
		vector.y += y;
		return vector;
	}

	/// <summary>
	/// Scales a <see cref="Vector2"/> by <paramref name="x"/> along the X-axis, and by <paramref name="y"/> along the Y-axis.
	/// </summary>
	/// <param name="vector"></param>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public static Vector2 ScaleComponents(this Vector2 vector, float x = 1, float y = 1)
	{
		vector.x *= x;
		vector.y *= y;
		return vector;
	}
	#endregion

	#region Vector3
	/// <summary>
	/// Rotates a <see cref="Vector3"/> <paramref name="by"/> the given rotation <paramref name="around"/> a pivot point.
	/// </summary>
	/// <param name="vector"></param>
	/// <param name="by"></param>
	/// <param name="around">The pivot point of the rotation.</param>
	/// <returns></returns>
	public static Vector3 Rotate(this Vector3 vector, Quaternion by, Vector3 around)
	{
		vector -= around;
		vector = by * vector;
		vector += around;
		return vector;
	}

	/// <summary>
	/// Adds to a vector component-wise.
	/// </summary>
	/// <param name="vector"></param>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="z"></param>
	/// <returns></returns>
	public static Vector3 Add(this Vector3 vector, float x = 0, float y = 0, float z = 0)
	{
		vector.x += x;
		vector.y += y;
		vector.z += z;
		return vector;
	}

	/// <summary>
	/// Scales a <see cref="Vector3"/> by <paramref name="x"/> along the X-axis, <paramref name="y"/> along the Y-axis, and <paramref name="z"/> along the Z-axis.
	/// </summary>
	/// <param name="vector"></param>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="z"></param>
	/// <returns></returns>
	public static Vector3 ScaleComponents(this Vector3 vector, float x = 1, float y = 1, float z = 1)
	{
		vector.x *= x;
		vector.y *= y;
		vector.z *= z;
		return vector;
	}
	#endregion
}
