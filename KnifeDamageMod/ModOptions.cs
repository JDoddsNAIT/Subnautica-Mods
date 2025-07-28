using Nautilus.Options.Attributes;

namespace FrootLuips.KnifeDamageMod
{
	[Menu(KnifeDamagePlugin.PLUGIN_NAME)]
	public class ModOptions : Nautilus.Json.ConfigFile
	{
		[Toggle(Label = "Mod Enabled")]
		public bool ModEnabled = true;

		[Slider(label: "Damage Multiplier", min: -100.0f, max: 100.0f, DefaultValue = 1.0f, Format = "{0:F2}")]
		public float DamageMultiplier = 1.0f;
	}
}
