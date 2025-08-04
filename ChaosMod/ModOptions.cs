using Nautilus.Options.Attributes;

namespace FrootLuips.ChaosMod;
[Menu(Plugin.NAME)]
internal class ModOptions : Nautilus.Json.ConfigFile
{
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

	[Choice(Label = "Frequency", Tooltip = "How often events are triggered.",
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
	public FrequencyEnum Frequency { get; set; }

	public float Delay => (ushort)Frequency;
}
