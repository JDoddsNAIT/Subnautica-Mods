using Nautilus.Options.Attributes;

namespace FrootLuips.ChaosMod;

[Menu(name: PluginInfo.PLUGIN_NAME)]
internal class ModOptions : Nautilus.Json.ConfigFile
{
	public const string
		FREQUENCY_LABEL = "Frequency",
		FREQUENCY_TOOLTIP = "How often events are triggered.",

		EFFECTCOUNT_LABEL = "Effects Triggered",
		EFFECTCOUNT_TOOLTIP = "The amount of effects triggered at once. It is recommended to adjust the frequency instead to increase or reduce difficulty.";

	/// <summary>
	/// The frequency at which events are triggered.
	/// </summary>
	/// <remarks>
	/// Values represent a time in seconds.
	/// </remarks>
	public enum FrequencyEnum : ushort
	{
		FiveSeconds = 5,
		TenSeconds = 10,
		ThirtySeconds = 30,
		OneMinute = 60,
		TwoMinutes = 120,
		FiveMinutes = 300,
		TenMinutes = 600,
		ThirtyMinutes = 1800,
		OneHour = 3600,
	}


	[Choice(Label = FREQUENCY_LABEL, Tooltip = FREQUENCY_TOOLTIP,
		Options = new[] {
			"5 Seconds",
			"10 Seconds",
			"30 Seconds",
			"1 Minute",
			"2 Minutes",
			"5 Minutes",
			"10 Minutes",
			"30 Minutes",
			"1 Hour"
		})]
	public FrequencyEnum Frequency { get; set; } = FrequencyEnum.FiveMinutes;

	[Slider(Label = EFFECTCOUNT_LABEL, Tooltip = EFFECTCOUNT_TOOLTIP, DefaultValue = 1, Min = 1, Max = 10)]
	public int EffectCount { get; set; } = 1;

	[Toggle(Label = "(Debug) Reset Effects", Tooltip = $"Debug option to always regenerate {EffectManager.EFFECTS_CONFIG} when starting the game.")]
	public bool DebugResetEffects { get; set; } = false;

	[IgnoreMember]
	public float Delay => (ushort)Frequency;
}
