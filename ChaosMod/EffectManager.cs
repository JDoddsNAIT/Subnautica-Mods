using System.Collections;
using FrootLuips.ChaosMod.Effects;
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

		if (!LoadEffects(callback))
			return;

		_mainRoutine = UWE.CoroutineHost.StartCoroutine(MainRoutine());
		callback(STARTUP_MESSAGE);
	}

	private static bool LoadEffects(Callback callback)
	{
		if (ChaosEffects.EffectsLoaded)
			return true;

		Plugin.Console.LogDebug("Loading Effects...");
		try
		{
			ChaosEffects.Load(effectsFilePath);
			Plugin.Console.LogDebug("Loaded effects!");
			return true;
		}
		catch (Exception ex)
		{
			callback(Logging.LogMessage.FromException(ex)
				.WithNotice("Failed to load effects, Mod could not be started."));
			return false;
		}
	}

	private static IEnumerator MainRoutine()
	{
		while (true)
		{
			yield return new UnityEngine.WaitForSeconds(Plugin.Options.Delay);
			try
			{
				AddEffect(callback: Plugin.Console.LogInfo);
			}
			catch (Exception ex)
			{
				Plugin.Console.LogError(Logging.LogMessage.FromException(ex)
					.AddNotice("Failed to trigger an effect"));
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

		static void callback2(string message) => Plugin.Console.LogDebug(message);
		RemoveEffect(callback2);

		UWE.CoroutineHost.StopCoroutine(_mainRoutine);
		_mainRoutine = null;
		callback(SHUTDOWN_MESSAGE);
	}

	/// <summary>
	/// Adds all the given <paramref name="effects"/>. Adds a random one if none are specified.
	/// </summary>
	/// <param name="effect"></param>
	/// <param name="callback"></param>
	public static void AddEffect(Callback callback, params ChaosEffect[] effects)
	{
		List<ChaosEffect> inactiveEffects;

		if (effects.Length > 0)
		{
			inactiveEffects = new List<ChaosEffect>(effects);
			SimpleQueries.Filter(inactiveEffects, static e => !_activeEffects.ContainsKey(e));
			SimpleQueries.FilterDuplicates(inactiveEffects);
		}
		else
		{
			ChaosEffects.GetRandomDistribution(exclusions: _activeEffects.Keys);
			var effect = ChaosEffects.RandomDistribution.GetRandomItem().Id;
			inactiveEffects = new List<ChaosEffect>() {
				effect
			};
		}

		if (!LoadEffects(callback))
			return;

		for (int i = 0; i < inactiveEffects.Count; i++)
		{
			TriggerEffect(ChaosEffects.Effects[inactiveEffects[i]]);
		}

		string message = NullOrEmptyCollection(inactiveEffects)
			? "No effects were triggered. Two of the same effect cannot be active at the same time."
			: $"Triggered effect(s): {string.Join(", ", inactiveEffects)}";
		callback?.Invoke(message);
	}

	private static void TriggerEffect(IChaosEffect effect)
	{
		string description = effect.BeforeStart();

		ActiveEffect activeEffect = new(effect);

		string message = string.IsNullOrWhiteSpace(description)
			? effect.Id.ToString()
			: description;
		Plugin.Game.LogMessage(message);

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
	public static void RemoveEffect(Callback? callback, params ChaosEffect[] effects)
	{
		List<ChaosEffect> effectsToClear;
		if (effects.Length == 0)
		{
			effectsToClear = _activeEffects.Keys.ToList();
		}
		else
		{
			effectsToClear = new(effects);
			SimpleQueries.Filter(effectsToClear, _activeEffects.ContainsKey);
		}

		for (int i = 0; i < effectsToClear.Count; i++)
		{
			_activeEffects[effectsToClear[i]].Stop();
		}

		string message = NullOrEmptyCollection(effectsToClear)
			? "No effects were removed."
			: $"Effects removed: {string.Join(", ", effectsToClear)}";
		callback?.Invoke(message);
	}

	public static IEnumerable<ChaosEffect> GetActiveEffects(Callback? callback)
	{
		var result = _activeEffects.Select(kvp => new EffectString(kvp.Key, kvp.Value.Duration - kvp.Value.Timer));

		string message = NullOrEmptyCollection(_activeEffects)
			? "No effects are active."
			: $"Active effects: {string.Join(", ", result)}";
		callback?.Invoke(message);

		return _activeEffects.Keys;
	}

	internal readonly record struct EffectString(ChaosEffect Effect, float Remaining)
	{
		public override string ToString() => $"{Effect} ({Remaining:0}s remaining)";
	}
}
