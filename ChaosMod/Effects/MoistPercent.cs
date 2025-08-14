using UnityEngine;

namespace FrootLuips.ChaosMod.Effects;
internal class MoistPercent : BaseChaosEffect
{
	public MoistPercent() : base(ChaosEffect.Moist, attributesExpected: 0, duration: 60f) { }

	public static Ocean? Ocean {
		get => _ocean;
		set {
			_ocean = value;
			if (value != null)
				_oceanPosition = value.transform.position;
		}
	}

	public const int MAX_DEPTH = -8192;
	private static Vector3 _oceanPosition;
	private static Ocean? _ocean;

	private bool _started = false;

	public override void OnStart()
	{
		EnsurePlayerExists();

		if (Ocean == null)
		{
			throw new Exception("Ocean was not initialized.");
		}

		_oceanPosition = Ocean.transform.position;
		Ocean.transform.position = _oceanPosition with { y = MAX_DEPTH };
		_started = true;
	}

	public override void OnStop()
	{
		if (_started && Ocean != null)
		{
			Ocean.transform.position = _oceanPosition;
			_started = false;
		}
	}

	protected override void ParseAttribute(Effect.Attribute attribute) { }
	protected override bool GetSuccess() => true;
}
