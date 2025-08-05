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

	public static bool NullOrEmptyCollection<T>(IReadOnlyCollection<T>? list) => list == null || list.Count is 0;

	/// <summary>
	/// Validates the values of an object using the give <paramref name="enumerator"/>, and throws an <see cref="AggregateException"/> if there are any issues.
	/// </summary>
	/// <param name="enumerator"></param>
	/// <returns><see langword="true"/> unless an exception is thrown.</returns>
	/// <exception cref="AggregateException"></exception>
	public static bool Validate(IEnumerator<Exception> enumerator)
	{
		var errors = new List<Exception>();

		try
		{
			while (enumerator.MoveNext())
			{
				errors.Add(enumerator.Current);
			}
		}
		catch (Exception ex)
		{
			errors.Add(ex);
		}

		if (errors.Count > 0)
			throw new AggregateException(errors.ToArray());
		else
			return true;
	}
}
