using FrootLuips.ChaosMod.Effects;
using UnityEngine;

namespace FrootLuips.ChaosMod.Utilities;
internal static class Utils
{
	public static string GetPluginPath(string filename = "")
	{
		return string.IsNullOrWhiteSpace(filename)
			? Path.Combine(BepInEx.Paths.PluginPath, PluginInfo.PLUGIN_GUID)
			: Path.Combine(BepInEx.Paths.PluginPath, PluginInfo.PLUGIN_GUID, filename);
	}

	public static string GetConfigPath(string filename = "")
	{
		return string.IsNullOrWhiteSpace(filename)
			? Path.Combine(BepInEx.Paths.ConfigPath, PluginInfo.PLUGIN_GUID)
			: Path.Combine(BepInEx.Paths.ConfigPath, PluginInfo.PLUGIN_GUID, filename);
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

	public static Exception Invalid(this Effect.Attribute attribute)
	{
		return new InvalidAttributeException(attribute);
	}

	public static void ParseAttribute<T>(this Effect.Attribute attribute, Func<string, T> parseFunc, out T value)
	{
		try
		{
			value = parseFunc(attribute.Value);
		}
		catch (Exception ex)
		{
			throw new InvalidAttributeException(attribute, ex);
		}
	}

	public static void ExpectAttributeCount(Effect.Attribute[] attributes, int count)
	{
		if (attributes.Length != count)
			throw new UnexpectedAttributesException(count);
	}

	public static void EnsurePlayerExists()
	{
		if (Player.main == null)
			throw new PlayerNotFoundException();
	}

	public static ActiveEffect Activate(this IChaosEffect effect) => new(effect);

	public static void InstantiateTechType(TechType techType, Vector3 position, Vector3? eulers = null, Vector3? scale = null)
	{
		UWE.CoroutineHost.StartCoroutine(InstantiateTechType_Routine(techType, position, eulers, scale));
	}

	private static System.Collections.IEnumerator InstantiateTechType_Routine(TechType techType, Vector3 position, Vector3? eulers = null, Vector3? scale = null)
	{
		eulers ??= Vector3.zero;
		scale ??= Vector3.one;

		var task = CraftData.GetPrefabForTechTypeAsync(techType);
		yield return task;
		var prefab = task.GetResult();

		// This mirrors the functionality of the "spawn" console command.
		GameObject clone = UnityEngine.Object.Instantiate(prefab, position, Quaternion.Euler((Vector3)eulers));
		clone.transform.localScale = (Vector3)scale;
		clone.SetActive(true);

		LargeWorldEntity.Register(clone);
		CrafterLogic.NotifyCraftEnd(clone, techType);
		clone.SendMessage("StartConstruction", SendMessageOptions.DontRequireReceiver);
	}
}
