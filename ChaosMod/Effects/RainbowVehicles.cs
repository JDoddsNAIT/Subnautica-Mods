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

		static VehicleColours converter(Vehicle vehicle) => new(vehicle, vehicle.subName.GetColors());
		EntityDB<Vehicle>.Update();
		SimpleQueries.Convert(EntityDB<Vehicle>.Entities, converter, _vehicleColours);
	}

	public override void Update(float time)
	{
		SimpleQueries.Filter(_vehicleColours, filter: vc => SimpleQueries.NotNull(vc.Vehicle));

		_hueShift += (float)(Speed! * Time.deltaTime);
		SetVehicleColours(_vehicleColours, _hueShift);
	}

	public override void OnStop()
	{
		SimpleQueries.Filter(_vehicleColours, filter: vc => SimpleQueries.NotNull(vc.Vehicle));

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
				var vehicle = colours[i].Vehicle;
				vehicle.subName.SetColor(j, colours[i][j].Shift(h: hueShift), Color.white);
			}
		}
	}

	private record struct VehicleColours(Vehicle Vehicle)
	{
		public ColorHSV[] Colours { get; set; } = Array.Empty<ColorHSV>();

		public readonly ColorHSV this[int index] => Colours[index];

		public VehicleColours(Vehicle vehicle, Vector3[] hsv) : this(vehicle)
		{
			var colours = new ColorHSV[hsv.Length];
			SimpleQueries.Convert(hsv, converter: c => (ColorHSV)c, ref colours);
			Colours = colours;
		}
	}
	private record struct ColorHSV(float H, float S, float V)
	{
		public ColorHSV Shift(float h = 0, float s = 0, float v = 0)
		{
			h = Mathf.Repeat(H + h, 1f);
			s = Mathf.Repeat(S + s, 1f);
			v = Mathf.Repeat(V + v, 1f);
			return this with { H = h, S = s, V = v };
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
