using System;
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

	public static Exception Invalid(this EffectData.Tag tag)
	{
		return new Exception($"{tag.Name} is an invalid data tag.");
	}

	public static void ParseTag<T>(this EffectData.Tag tag, SimpleQueries.TryFunc<string, T> tryParse, out T value)
		where T : struct
	{
		if (!tryParse(tag.Value, out value))
		{
			throw tag.Invalid();
		}
	}

	/// <summary>
	/// Used when setting the property value of a <see cref="IChaosEffect"/> from a tag.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="name"></param>
	/// <param name="getter"></param>
	/// <param name="setter"></param>
	/// <param name="value"></param>
	/// <exception cref="Exception"></exception>
	public static void SetProperty<T>(string name, Func<T?> getter, Action<T> setter, T value)
		where T : struct
	{
		if (getter() == null)
		{
			setter(value);
			throw new Exception($"the value for '{name}' has been overwritten by another.");
		}
		else
		{
			setter(value);
		}
	}
}
