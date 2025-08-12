using FrootLuips.ChaosMod.Utilities;

namespace FrootLuips.ChaosMod.Objects;
[Serializable]
internal class ScaleData : IDistributable
{
	public const char DELIMITER = ';';

	public float Scale { get; set; }
	public int Weight { get; set; }

	public ScaleData(float scale, int weight)
	{
		this.Scale = scale;
		this.Weight = weight;
	}

	public ScaleData() : this(1f, 100) { }

	public override string ToString()
	{
		return $"{Scale},{Weight}";
	}

	public static ScaleData? Parse(string text)
	{
		try
		{
			var parts = text.Split(',');

			return new ScaleData() {
				Scale = float.Parse(parts[0]),
				Weight = int.Parse(parts[1])
			};
		}
		catch (Exception)
		{
			return default;
		}
	}

	public static ScaleData[] ParseMany(string text)
	{
		var parts = text.Split(DELIMITER);
		var results = new ScaleData?[parts.Length];
		SimpleQueries.Convert(parts, converter: Parse, ref results);
		SimpleQueries.FilterNulls(ref results);
		return results!;
	}
}
