using UnityEngine;

namespace FrootLuips.Subnautica.Extensions;
public static class VectorExtensions
{
	#region Vector2
	public static Vector2 Rotate(this Vector2 vector, float angle, Vector2? center = null)
	{
		center ??= Vector2.zero;
		vector -= (Vector2)center;
		var rotation = Quaternion.Euler(0, 0, z: angle);
		vector = rotation * vector;
		vector += (Vector2)center;
		return vector;
	}

	public static Vector2 Add(this Vector2 vector, float x = 0, float y = 0)
	{
		vector.x += x;
		vector.y += y;
		return vector;
	}

	public static Vector2 ScaleComponents(this Vector2 vector, float x = 1, float y = 1)
	{
		vector.x *= x;
		vector.y *= y;
		return vector;
	}
	#endregion

	#region Vector3
	public static Vector3 Rotate(this Vector3 vector,
		float angleX = 0, float angleY = 0, float angleZ = 0,
		Vector3? center = null)
	{
		return Rotate(vector, new Vector3(angleX, angleY, angleZ), center);
	}

	public static Vector3 Rotate(this Vector3 vector, Vector3 eulers, Vector3? center = null)
	{
		center ??= Vector3.zero;
		vector -= (Vector3)center;
		var rotation = Quaternion.Euler(eulers);
		vector = rotation * vector;
		vector += (Vector3)center;
		return vector;
	}

	public static Vector3 Add(this Vector3 vector, float x = 0, float y = 0, float z = 0)
	{
		vector.x += x;
		vector.y += y;
		vector.z += z;
		return vector;
	}

	public static Vector3 ScaleComponents(this Vector3 vector, float x = 1, float y = 1, float z = 0)
	{
		vector.x *= x;
		vector.y *= y;
		vector.z *= z;
		return vector;
	}
	#endregion
}
