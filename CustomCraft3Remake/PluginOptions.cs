using Nautilus.Options.Attributes;

namespace FrootLuips.CustomCraft3Remake;

[Menu(name: "Custom Craft 3 - Remake")]
internal class PluginOptions : Nautilus.Json.ConfigFile
{
	public enum RegenSamples { Never, RestoreMissing, RepairExisting, Always }

	[Toggle(Label = "Remove Converted CustomCraft3 Files", Tooltip = "Whether any CustomCraft3 files get removed after being converted. On by default.")]
	public bool RemoveConvertedFiles = true;

	[Choice(Label = "Regenerate Samples",
		Options = new[] {
			"Never",
			"Restore Missing",
			"Repair Existing",
			"Always"
		},
		Tooltip = "Determines how often sample files are regenerated.\n" +
		"Never - Samples are never regenerated.\n" +
		"Restore Missing - Missing files are restored, existing files remain untouched. This is the default behaviour.\n" +
		"Repair Existing - Existing files are reverted to their original state.\n" +
		"Always - All sample files are regenerated when the game launches.")]
	public RegenSamples RegenerateSamples = RegenSamples.RestoreMissing;
}
