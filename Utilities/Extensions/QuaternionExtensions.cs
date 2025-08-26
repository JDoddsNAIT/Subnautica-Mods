using UnityEngine;

namespace FrootLuips.Subnautica.Extensions;
/// <summary>
/// Extension methods for <see cref="Quaternion"/>.
/// </summary>
public static class QuaternionExtensions
{
	/// <summary>
	/// Adds two quaternions together.
	/// </summary>
	/// <param name="lhs"></param>
	/// <param name="rhs"></param>
	/// <returns></returns>
	public static Quaternion Add(this Quaternion lhs, Quaternion rhs)
	{
		return Quaternion.Euler(lhs.eulerAngles + rhs.eulerAngles);
	}

	/// <summary>
	/// Subtracts <paramref name="rhs"/> from <paramref name="lhs"/>.
	/// </summary>
	/// <param name="lhs"></param>
	/// <param name="rhs"></param>
	/// <returns></returns>
	public static Quaternion Subtract(this Quaternion lhs, Quaternion rhs)
	{
		return Quaternion.Euler(lhs.eulerAngles - rhs.eulerAngles);
	}

	/// <summary>
	/// Scales a <paramref name="quaternion"/> by the given <paramref name="factor"/>.
	/// </summary>
	/// <param name="quaternion"></param>
	/// <param name="factor"></param>
	/// <returns></returns>
	public static Quaternion Scale(this Quaternion quaternion, float factor)
	{
		return Quaternion.Euler(quaternion.eulerAngles * factor);
	}
}
