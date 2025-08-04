using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FrootLuips.ChaosMod.Effects;
using FrootLuips.ChaosMod.Utilities;
using static FrootLuips.ChaosMod.Utilities.Utilities;

namespace FrootLuips.ChaosMod;

/// <summary>
/// Handles the activation timer, effect coroutines, and more.
/// </summary>
internal static class ChaosMod
{
	public const string
		START_MESSAGE = "Let the Chaos begin!",
		STOP_MESSAGE = "Stopping the chaos.";

	public const string EFFECTS_CONFIG = "effects.json";

	private static readonly Dictionary<ChaosEffect, UnityEngine.Coroutine> _activeEffects = new();
	private static UnityEngine.Coroutine? _main;

	public static readonly string effectsFilePath = GetPluginPath(EFFECTS_CONFIG);

	public static void Start(bool showInGame = true)
	{
		Plugin.Logger.LogDebug("Loading Effects...");

		try
		{
			ChaosEffects.Load(effectsFilePath);
			Plugin.Logger.LogDebug("Loaded effects!");
		}
		catch (Exception ex)
		{
			Plugin.Logger.LogInGame(
				Logging.LogMessage.FromException(ex).WithNotice("Failed to load effects"),
				BepInEx.Logging.LogLevel.Error);
			return;
		}

		_main = UWE.CoroutineHost.StartCoroutine(RunChaosMod());

		if (showInGame)
		{
			Plugin.Logger.LogInGame(START_MESSAGE);
		}
	}

	public static void Stop(bool showInGame = true)
	{
		if (_main != null)
		{
			UWE.CoroutineHost.StopCoroutine(_main);
			_main = null;
		}

		if (showInGame)
		{
			Plugin.Logger.LogInGame(STOP_MESSAGE);
		}
	}

	public static IEnumerable<string> GetActiveEffectIds()
	{
		if (_activeEffects.Count == 0)
		{
			yield return "No effects are currently active.";
		}
		else
		{
			foreach (var effect in _activeEffects.Keys)
			{
				yield return effect.ToString();
			}
		}
	}

	public static void TriggerEffect(IChaosEffect effect, bool showInGame = true)
	{
		if (!_activeEffects.ContainsKey(effect.Id))
			throw new Exception("Two effects cannot be active at the same time");

		var routine = UWE.CoroutineHost.StartCoroutine(effect.Activate());
		UWE.CoroutineHost.StartCoroutine(RemoveEffectAfterDuration(effect));
		_activeEffects.Add(effect.Id, routine);

		if (showInGame)
		{
			float duration = effect.Duration > 0 ? effect.Duration : 5f;
			Plugin.Logger.LogInGame(effect.Description, BepInEx.Logging.LogLevel.Info, duration);
		}
	}

	public static void ClearEffects(params ChaosEffect[] effects)
	{
		if (effects.Length == 0)
		{
			ClearEffects(_activeEffects.Keys.ToArray());
		}
		else
		{
			for (int i = 0; i < effects.Length; i++)
			{
				if (_activeEffects.TryGetValue(effects[i], out var coroutine))
				{
					UWE.CoroutineHost.StopCoroutine(coroutine);
					_activeEffects.Remove(effects[i]);
				}
			}
		}
	}

	private static IEnumerator RunChaosMod()
	{
		while (true)
		{
			yield return new UnityEngine.WaitForSeconds(Plugin.Options.Delay);
			TriggerEffect(ChaosEffects.GetRandomDistribution(exclusions: _activeEffects.Keys).GetRandomItem());
		}
	}

	private static IEnumerator RemoveEffectAfterDuration(IChaosEffect effect)
	{
		yield return new UnityEngine.WaitForSeconds(effect.Duration);
		ClearEffects(effect.Id);
	}
}
