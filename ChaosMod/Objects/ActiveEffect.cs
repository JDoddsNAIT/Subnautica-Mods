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

		try
		{
			Effect.OnStart();
		}
		catch (Exception ex)
		{
			Plugin.Logger.LogWarn(Logging.LogMessage.FromException(ex)
				.WithContext(Effect.Id)
				.WithNotice("An error occurred while triggering the effect"));
		}

		Coroutine ??= UWE.CoroutineHost.StartCoroutine(Routine());
	}

	public IEnumerator Routine()
	{
		while (Timer < Duration)
		{
			yield return null;
			try
			{
				Effect.Update(Timer);
			}
			catch (Exception ex)
			{
				Plugin.Logger.LogError(Logging.LogMessage.FromException(ex)
					.WithContext(Effect.Id)
					.WithNotice("An error occurred while updating the effect"));
				Stop();
				yield break;
			}
			Timer += UnityEngine.Time.deltaTime;
		}

		Coroutine = null;
		Stop();
	}

	public void Stop()
	{
		Timer = Duration;

		try
		{
			Effect.OnStop();
		}
		catch (Exception ex)
		{
			Plugin.Logger.LogWarn(Logging.LogMessage.FromException(ex)
				.WithContext(Effect.Id)
				.WithNotice("An error occurred while stopping the effect"));
		}

		OnEffectEnd(this);

		if (Coroutine != null)
		{
			UWE.CoroutineHost.StopCoroutine(Coroutine);
			Coroutine = null;
		}
	}
}
