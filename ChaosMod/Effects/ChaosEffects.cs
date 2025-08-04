using System;
using System.Collections.Generic;
using System.Linq;
using FrootLuips.ChaosMod.Logging;
using FrootLuips.ChaosMod.Objects;
using Nautilus.Json.ExtensionMethods;

namespace FrootLuips.ChaosMod.Effects;
internal static class ChaosEffects
{
	public static Dictionary<ChaosEffect, IChaosEffect> Effects { get; private set; } = ResetEffects();

	public static Dictionary<ChaosEffect, IChaosEffect> ResetEffects() => Effects = new() {
		[ChaosEffect.ReaperRain] = new ReaperRain() { Height = 100, SpawnsPerSecond = 1 },
	};

	public static RandomDistribution<IChaosEffect>? RandomDistribution { get; private set; }

	public static RandomDistribution<IChaosEffect> GetRandomDistribution(IEnumerable<ChaosEffect> exclusions)
	{
		var effects = Effects
			.Where(kvp => !exclusions.Contains(kvp.Key) && kvp.Value.Weight > 0)
			.Select(kvp => kvp.Value)
			.ToArray();
		return RandomDistribution = new RandomDistribution<IChaosEffect>(effects);
	}

	public static void Load(string filePath)
	{
		const string context = $"{nameof(ChaosEffects)}.{nameof(Load)})";
		var effects = new List<EffectData>();

		effects.LoadJson(filePath);

		for (int i = 0; i < effects.Count; i++)
		{
			if (!Enum.TryParse(effects[i].Id, out ChaosEffect effect))
			{
				Plugin.Logger.LogWarn(new LogMessage(context: context)
					.WithNotice("Effect ID '", effects[i].Id, "' is invalid")
					.WithMessage("Skipping"));
				continue;
			}
			else
			{
				Effects[effect].FromData(effects[i], statusCallback);
				continue;

				void statusCallback(List<string> issues, bool status)
				{
					LogMessage message = new(context: context);
					if (status)
					{
						message.WithNotice("Loaded settings for effect '", effect, "'");
						if (issues.Count > 0)
						{
							message.WithMessage("Some issues occurred.").WithRemarks(string.Join("\n", issues));
							Plugin.Logger.LogWarn(message);
						}
						else
						{
							message.WithMessage("No issues");
							Plugin.Logger.LogInfo(message);
						}
					}
					else
					{
						message.WithNotice("Failed to load settings for '", effect, "'")
							.WithMessage("Skipping")
							.WithRemarks(string.Join("\n", issues));
						Plugin.Logger.LogError(message);
					}
				}
			}
		}
	}

	public static void Save(string filePath)
	{
		const string context = $"{nameof(ChaosEffects)}.{nameof(Save)})";
		var effects = Effects.Values.Select(static v => v.ToData());
		effects.SaveJson(filePath);
	}
}

internal enum ChaosEffect
{
	ReaperRain,

}
