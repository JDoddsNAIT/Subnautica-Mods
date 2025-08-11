using UnityEngine;

namespace FrootLuips.Subnautica.Extensions;
public static class QuaternionExtensions
{
	public static Quaternion Add(this Quaternion a, Quaternion b)
	{
		return Quaternion.Euler(a.eulerAngles + b.eulerAngles);
	}

	public static Quaternion Scale(this Quaternion quaternion, float scale)
	{
		return Quaternion.Euler(quaternion.eulerAngles * scale);
	}
}
