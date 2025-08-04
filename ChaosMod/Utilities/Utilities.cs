using System;
using System.Collections.Generic;
using System.IO;

using FrootLuips.ChaosMod.Effects;
using FrootLuips.ChaosMod.Logging;
using FrootLuips.ChaosMod.Objects;

namespace FrootLuips.ChaosMod.Utilities;
internal static class Utilities
{
	public static string GetPluginPath(string filename = "")
	{
		return string.IsNullOrWhiteSpace(filename)
			? Path.Combine(BepInEx.Paths.PluginPath, PluginInfo.PLUGIN_NAME)
			: Path.Combine(BepInEx.Paths.PluginPath, PluginInfo.PLUGIN_NAME, filename);
	}

	public static string GetConfigPath(string filename = "")
	{
		return string.IsNullOrWhiteSpace(filename)
			? Path.Combine(BepInEx.Paths.ConfigPath, PluginInfo.PLUGIN_NAME)
			: Path.Combine(BepInEx.Paths.ConfigPath, PluginInfo.PLUGIN_NAME, filename);
	}
}
