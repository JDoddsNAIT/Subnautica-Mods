using FrootLuips.ChaosMod.Logging;

namespace FrootLuips.ChaosMod.Effects;
internal static class ChaosEffects
{
	public static bool EffectsLoaded { get; private set; } = false;

	public static Dictionary<ChaosEffect, IChaosEffect> Effects { get; private set; } = ResetEffects();

	public static Dictionary<ChaosEffect, IChaosEffect> ResetEffects() => Effects = new() {
		[ChaosEffect.ReaperRain] = new ReaperRain() {
			Description = "It's raining reapers!",
			Height = 100, SpawnsPerSecond = 1, SpawnRadius = 50,
		},
		[ChaosEffect.RandomTeleport] = new RandomTeleport() { 
			Description = "Teleport",
		},
		[ChaosEffect.ExplodeShip] = new ExplodeShip() {
			Description = "Explode the Aurora",
		},
		[ChaosEffect.SpawnGhost] = new SpawnGhost() {
			Description = "Say hi to Casper",
			SpawnDistance = 12f
		},
		//[ChaosEffect.SuperSpeed] = new SuperSpeed() {
		//	Description = "Activate Super Speed",
		//	Multiplier = 5f
		//},
		[ChaosEffect.Mushrooms] = new Mushrooms() {
			Description = "Oops, all mushrooms!"
		},
		[ChaosEffect.Fly] = new Fly() {
			Description = "Enable fly cheat",
		},
		[ChaosEffect.DamageVehicles] = new DamageVehicles() {
			Description = "Insurance Claim",
			DamageDealt = 50f
		},
		//[ChaosEffect.RainbowVehicles] = new RainbowVehicles() {
		//	Description = "Rainbow Vehicles",
		//	Speed = 1f
		//},
		[ChaosEffect.Moist] = new MoistPercent() {
			Description = "Moist%",
		},
		//[ChaosEffect.Lootbox]
		//[ChaosEffect.FakeTeleport]
		//[ChaosEffect.FakeCrash]
		[ChaosEffect.ScalePlayer] = new ScalePlayer() {
			Description = "{0} Player",
			Scales = new[] {
				new ScaleData(0.5f, 50),
				new ScaleData(2.0f, 50),
			}
		},
		[ChaosEffect.ScaleCreatures] = new ScaleCreatures() {
			Description = "{0} Nearby Creatures",
			Scales = new[] {
				new ScaleData(0.5f, 50),
				new ScaleData(2.0f, 50),
			},
			Range = 20f
		}
	};

	public static RandomDistribution<IChaosEffect> RandomDistribution { get; private set; }
		= GetRandomDistribution(Array.Empty<ChaosEffect>());

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
		const string context = $"{nameof(ChaosEffects)}.{nameof(Load)}";
		var effects = new List<Effect>();

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
				bool calledBack = false;
				Effects[effect].FromData(effects[i], statusCallback);
				if (!calledBack)
				{
					Plugin.Logger.LogWarn($"No callback received from '{effect}'. Data integrity is unknown.");
				}
				continue;

				void statusCallback(List<string> issues, bool status)
				{
					calledBack = true;
					StatusCallback(issues, status, context, effect);
				}
			}
		}
		EffectsLoaded = true;
	}

	private static void StatusCallback(List<string> issues, bool status, string context, ChaosEffect effect)
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

	public static void Save(string filePath)
	{
		const string context = $"{nameof(ChaosEffects)}.{nameof(Save)})";
		var effects = Effects.Values.Select(static v => v.ToData());
		effects.SaveJson(filePath);
	}

	public static bool TryGetEffect(string effect, out ChaosEffect effectId)
	{
		return Enum.TryParse(effect, ignoreCase: true, out effectId) && ChaosEffects.Effects.ContainsKey(effectId);
	}
}

internal enum ChaosEffect
{
	ReaperRain,
	RandomTeleport,
	// "Explode the Aurora" - explodes the aurora
	ExplodeShip,
	// "Say Hi to Casper!" - spawns a ghost leviathan in front of the player
	SpawnGhost,
	// TODO: "{Multiplier}x Move Speed" - Multiplies the player's move speed for some time.
	SuperSpeed,
	// "Oops, all Mushrooms!" - Fills the player's inventory with acid mushrooms
	Mushrooms,
	// "Fly" - Enables the 'Fly' cheat for some time
	Fly,
	// "Insurance claim" - Deals 50% damage to all vehicles on the map
	DamageVehicles,
	// "Rainbow Vehicles" - All vehicles continuously change colours for some time.
	RainbowVehicles,
	// "Moist%" - Turns off the water for the duration
	Moist,
	// TODO: "Lootbox" - Spawn and pickup a time capsule in front of the player
	Lootbox,
	// TODO: "Fake Teleport" - Teleports the player to a random location, waits a bit, then teleports them back.
	FakeTeleport,
	// TODO: "Fake Crash" - Freezes the game for the duration
	FakeCrash,
	// "Shrink/Grow Player" - Scales the player
	ScalePlayer,
	// "Shrink/Grow Nearby Creatures" - scales all nearby creatures.
	ScaleCreatures,
}
