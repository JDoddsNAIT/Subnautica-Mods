using System.Collections;
using FrootLuips.ChaosMod.Effects;

namespace FrootLuips.ChaosMod.Objects;

internal class ActiveEffect
{
	public float Timer { get; set; }
	public float Duration => this.Effect.Duration;
	public IChaosEffect Effect { get; private set; }
	public UnityEngine.Coroutine? Coroutine { get; private set; }

	public event OnEffectEnd OnEffectEnd = delegate { };

	public ActiveEffect(IChaosEffect effect)
	{
		Timer = 0;
		this.Effect = effect;
	}

	public void Start()
	{
		Timer = 0;
		Effect.OnStart();

		Coroutine ??= UWE.CoroutineHost.StartCoroutine(Routine());
	}

	public void Stop()
	{
		Timer = Duration;
		Effect.OnStop();
		OnEffectEnd(this);

		if (Coroutine != null)
		{
			UWE.CoroutineHost.StopCoroutine(Coroutine);
			Coroutine = null;
		}
	}

	public IEnumerator Routine()
	{
		while (Timer < Duration)
		{
			yield return null;
			Effect.Update(Timer);
			Timer += UnityEngine.Time.deltaTime;
		}

		Coroutine = null;
		Stop();
	}
}
