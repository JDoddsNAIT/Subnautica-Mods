using Nautilus.Options.Attributes;

namespace FrootLuips.Playground;

[Menu(PluginInfo.PLUGIN_NAME)]
public class ModOptions : Nautilus.Json.ConfigFile
{
	[Toggle(Label = "Developer Logs")]
	public bool EnableLogs = true;

	[Slider(Label = "Poison Knife Base Damage", DefaultValue = 10.0f, Min = 0.0f, Max = 100.0f, Step = 1.0f)]
	public float PoisonKnifeBaseDamage = 10.0f;

	[Slider(Label = "Poison Effect Duration", DefaultValue = 60.0f, Min = 1.0f, Max = 360.0f, Step = 0.5f)]
	public float PoisonEffectDuration = 60.0f;

	[Slider(Label = "Poison Effect Damage", DefaultValue = 5.0f, Min = 0.1f, Max = 1000.0f, Step = 1.0f)]
	public float PoisonEffectDamage = 5.0f;
}
