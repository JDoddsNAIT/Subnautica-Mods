using FrootLuips.ChaosMod.Utilities;
using UnityEngine;

namespace FrootLuips.ChaosMod.Effects;
internal class RainbowVehicles : BaseChaosEffect
{
	public RainbowVehicles() : base(ChaosEffect.RainbowVehicles, attributesExpected: 1, duration: 60f) { }

	public float? Speed { get; set; } = null;

	private readonly List<VehicleColours> _vehicleColours = new();
	private float _hueShift = 0;

	public override void OnStart()
	{
		_hueShift = 0;

		static VehicleColours converter(SubName name) => new(name, name.GetColors());
		SimpleQueries.Convert(EntityDB<SubName>.Entities, converter, _vehicleColours);
	}

	public override void Update(float time)
	{
		SimpleQueries.Filter(_vehicleColours, filter: vc => SimpleQueries.NotNull(vc.SubName));

		_hueShift += (float)(Speed! * Time.deltaTime);
		SetVehicleColours(_vehicleColours, _hueShift);
	}

	public override void OnStop()
	{
		SimpleQueries.Filter(_vehicleColours, filter: vc => SimpleQueries.NotNull(vc.SubName));

		_hueShift = 0;
		SetVehicleColours(_vehicleColours, _hueShift);
		_vehicleColours.Clear();
	}

	public override void FromData(Effect data, StatusCallback callback)
	{
		Speed = null;
		base.FromData(data, callback);
	}

	protected override void ParseAttribute(Effect.Attribute attribute)
	{
		switch (attribute.Name)
		{
			case nameof(Speed):
				attribute.ParseValue(float.Parse, out var speed);
				Speed = speed;
				break;
			default:
				throw attribute.Invalid();
		}
	}

	protected override bool GetSuccess() => this.Speed != null;

	public override Effect ToData() => new() {
		Id = this.Id.ToString(),
		Duration = this.Duration,
		Weight = this.Weight,
		Attributes = new[] {
			new Effect.Attribute(nameof(Speed), Speed.ToString()),
		}
	};

	private static void SetVehicleColours(IReadOnlyList<VehicleColours> colours, float hueShift)
	{
		for (int i = 0; i < colours.Count; i++)
		{
			for (int j = 0; j < colours[i].Colours.Length; j++)
			{
				var name = colours[i].SubName;
				name.SetColor(j, Vector3.one, colours[i][j].Shift(h: hueShift));
			}
		}
	}

	private record struct VehicleColours(SubName SubName)
	{
		public ColorHSV[] Colours { get; set; } = Array.Empty<ColorHSV>();

		public readonly ColorHSV this[int index] => Colours[index];

		public VehicleColours(SubName subName, Vector3[] hsv) : this(subName)
		{
			var colours = new ColorHSV[hsv.Length];
			SimpleQueries.Convert(hsv, converter: c => (ColorHSV)c, ref colours);
			Colours = colours;
		}
	}
	private record struct ColorHSV(float H, float S, float V)
	{
		public readonly ColorHSV Shift(float h = 0, float s = 0, float v = 0)
		{
			return new(ShiftScalar(H, by: h), ShiftScalar(S, by: s), ShiftScalar(V, by: v));
		}

		private static float ShiftScalar(float value, float by)
		{
			value += by;
			while (value is < 0f or > 1f)
				value -= Mathf.Sign(value);
			return value;
		}

		public static implicit operator ColorHSV(Vector3 vector)
			=> new(H:  vector.x, S: vector.y, V: vector.z);
		public static implicit operator Vector3(ColorHSV colour)
			=> new(colour.H, colour.S, colour.V);

		public static implicit operator ColorHSV(Color color)
		{
			Color.RGBToHSV(color, out var h, out var s, out var v);
			return new(h, s, v);
		}
		public static implicit operator Color(ColorHSV colour)
			=> Color.HSVToRGB(colour.H, colour.S, colour.V);
	}
}
