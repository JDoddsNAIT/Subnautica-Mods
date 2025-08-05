using System.Collections;
using FrootLuips.ChaosMod.Effects;
using FrootLuips.ChaosMod.Objects;
using FrootLuips.ChaosMod.Utilities;

namespace FrootLuips.ChaosMod;
internal static class EffectManager
{
	public const string
		STARTUP_MESSAGE = "Let the Chaos begin!",
		SHUTDOWN_MESSAGE = "Stopping ChaosMod";

	public const string EFFECTS_CONFIG = "effects.json";

	public static readonly string effectsFilePath = GetPluginPath(EFFECTS_CONFIG);

	private static readonly Dictionary<ChaosEffect, ActiveEffect> _activeEffects = new();
	private static UnityEngine.Coroutine? _mainRoutine;

	/// <summary>
	/// Starts the main routine that runs the mod.
	/// </summary>
	/// <param name="callback"></param>
	public static void StartRoutine(Callback callback)
	{
		if (_mainRoutine != null)
		{
			callback("Mod is already running!");
			return;
		}

		Plugin.Logger.LogDebug("Loading Effects...");
		try
		{
			ChaosEffects.Load(effectsFilePath);
			Plugin.Logger.LogDebug("Loaded effects!");
		}
		catch (Exception ex)
		{
			callback(Logging.LogMessage.FromException(ex)
				.WithNotice("Failed to load effects, Mod could not be started."));
			return;
		}

		_mainRoutine = UWE.CoroutineHost.StartCoroutine(MainRoutine());
		callback(STARTUP_MESSAGE);
	}

	private static IEnumerator MainRoutine()
	{
		while (true)
		{
			yield return new UnityEngine.WaitForSeconds(Plugin.Options.Delay);
			try
			{
				ChaosEffects.GetRandomDistribution(exclusions: _activeEffects.Keys);
				TriggerEffect(ChaosEffects.RandomDistribution.GetRandomItem());
			}
			catch (Exception ex)
			{
				Plugin.Logger.LogError(Logging.LogMessage.FromException(ex).AddNotice("Failed to trigger an effect"));
				continue;
			}
		}
	}

	/// <summary>
	/// Stops the mod.
	/// </summary>
	/// <param name="callback"></param>
	public static void StopRoutine(Callback callback)
	{
		if (_mainRoutine == null)
		{
			callback("Mod is not running!");
			return;
		}

		static void callback2(string message) => Plugin.Logger.LogDebug(message);
		RemoveEffect(callback2);

		UWE.CoroutineHost.StopCoroutine(_mainRoutine);
		_mainRoutine = null;
		callback(SHUTDOWN_MESSAGE);
	}

	/// <summary>
	/// Adds all the given <paramref name="effects"/>.
	/// </summary>
	/// <param name="effect"></param>
	/// <param name="callback"></param>
	public static void AddEffect(Callback callback, params ChaosEffect[] effects)
	{
		var inactiveEffects = effects.SimpleWhere(e => !_activeEffects.ContainsKey(e)).SimpleDistinct();

		for (int i = 0; i < inactiveEffects.Count; i++)
		{
			TriggerEffect(ChaosEffects.Effects[inactiveEffects[i]]);
		}

		string message = NullOrEmptyCollection(inactiveEffects)
			? "No effects were triggered. Two of the same effect cannot be active at the same time."
			: $"Triggered effect(s): {string.Join(", ", inactiveEffects)}";
		callback(message);
	}

	private static void TriggerEffect(IChaosEffect effect)
	{
		ActiveEffect activeEffect = new(effect);

		string message = string.IsNullOrWhiteSpace(effect.Description)
			? effect.Id.ToString()
			: effect.Description!;
		Plugin.Logger.LogInGame(message);

		activeEffect.OnEffectEnd += OnEffectEnd;
		_activeEffects.Add(effect.Id, activeEffect);
		activeEffect.Start();
	}

	private static void OnEffectEnd(ActiveEffect effect)
	{
		_activeEffects.Remove(effect.Effect.Id);
		effect.OnEffectEnd -= OnEffectEnd;
	}

	/// <summary>
	/// Removes all given <paramref name="effects"/>. If none are specified, all effects will be removed.
	/// </summary>
	/// <param name="callback"></param>
	/// <param name="effects"></param>
	public static void RemoveEffect(Callback callback, params ChaosEffect[] effects)
	{
		List<ChaosEffect> activeEffects = effects.SimpleWhere(_activeEffects.ContainsKey);
		if (effects.Length == 0)
		{
			activeEffects = _activeEffects.Keys.ToList();
		}

		for (int i = 0; i < activeEffects.Count; i++)
		{
			_activeEffects[activeEffects[i]].Stop();
		}

		string message = NullOrEmptyCollection(activeEffects)
			? "No effects were removed."
			: $"Effects removed: {string.Join(", ", activeEffects)}";
		callback(message);
	}

	public static IEnumerable<ChaosEffect> GetActiveEffects(Callback callback)
	{
		var result = _activeEffects.Keys;

		string message = NullOrEmptyCollection(_activeEffects)
			? "No effects are active."
			: $"Active effects: {string.Join(", ", result)}";
		callback(message);

		return result;
	}
}
