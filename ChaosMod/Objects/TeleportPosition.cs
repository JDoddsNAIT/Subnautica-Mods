using UnityEngine;

namespace FrootLuips.ChaosMod.Objects;

[Serializable]
public class TeleportPosition : IDistributable
{
	public string name = "";
	public Position position;
	public int weight;

	[Newtonsoft.Json.JsonIgnore]
	public int Weight => weight;
}

[Serializable]
public struct Position
{
	public float x, y, z;

	public static implicit operator Position(Vector3 vector) => new() {
		x = vector.x,
		y = vector.y,
		z = vector.z
	};
	public static implicit operator Vector3(Position position) => new() {
		x = position.x,
		y = position.y,
		z = position.z
	};
}
