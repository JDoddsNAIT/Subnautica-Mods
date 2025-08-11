using FrootLuips.ChaosMod.Utilities;
using Unity.Mathematics;
using UnityEngine;

namespace FrootLuips.ChaosMod.Effects;
internal class RainbowVehicles : BaseChaosEffect
{
	public RainbowVehicles() : base(ChaosEffect.RainbowVehicles, attributesExpected: 1, duration: 60f) { }

	public float? Speed { get; set; } = null;

	private Vehicle[]? _vehicles;
	private float3[][]? _hsv;
	private float _hueShift = 0;

	public override void OnStart()
	{
		_vehicles = UnityEngine.Object.FindObjectsOfType<global::Vehicle>();
		_hsv = new float3[_vehicles.Length][];
		_hueShift = 0;

		for (int i = 0; i < _vehicles.Length; i++)
		{
			var colors = _vehicles[i].subName.GetColors();
			_hsv[i] = new float3[colors.Length];
			for (int j = 0; j < colors.Length; j++)
			{
				var color = new Color(colors[j].x, colors[j].y, colors[j].z);
				Color.RGBToHSV(color, out var h, out var s, out var v);
				_hsv[i][j] = new float3(h, s, v);
			}
		}
	}

	public override void Update(float time)
	{
		_hueShift += (float)(Speed! * Time.deltaTime);
		this.SetHues();
	}

	private void SetHues(bool preserveSV = false)
	{
		for (int i = 0; i < _vehicles!.Length; i++)
		{
			for (int j = 0; j < _hsv![i].Length; j++)
			{
				var hsv = _hsv![i][j];
				float hue = Mathf.Repeat(_hueShift + hsv.x, 1f);
				float sat = preserveSV ? hsv.y : 1;
				float val = preserveSV ? hsv.z : 1;
				_vehicles[i].subName.SetColor(j, Vector3.one, Color.HSVToRGB(hue, sat, val));
			}
		}
	}

	public override void OnStop()
	{
		_hueShift = 0;
		SetHues(preserveSV: true);

		_vehicles = null;
		_hsv = null;
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
				attribute.ParseAttribute(float.Parse, out var speed);
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
}
