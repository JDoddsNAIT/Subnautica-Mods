using Nautilus.Options.Attributes;

namespace FrootLuips.ChaosMod;
[Menu(Plugin.NAME)]
internal class ModOptions : Nautilus.Json.ConfigFile
{
	[Toggle(Label = "Restore Default Effect Settings", Tooltip = "Effect settings will be restored to their default values when the mod is activated.")]
	public bool ResetEffects = false;

	[Slider(DefaultValue = 300f, Format ="{0:F0} seconds", Label = "Delay", Min = 5f, Max = 3600f, Step = 1,
		Tooltip = "The delay between events.")]
	public float Delay { get; set; }
}
