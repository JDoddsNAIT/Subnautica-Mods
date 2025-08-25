using BepInEx;
using FrootLuips.Subnautica.Tests;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = FrootLuips.Subnautica.Logging.Logger;

namespace FrootLuips.Subnautica;

/// <summary>
/// Main plugin class.
/// </summary>
[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public sealed class Plugin : BaseUnityPlugin
{
	private static Logger? _logger;
	/// <summary>
	/// Project-scoped logger instance.
	/// </summary>
	internal static new Logger Logger { get => _logger!; private set => _logger = value; }

	internal void Awake()
	{
		Logger = new Logger(base.Logger);

		SceneManager.sceneLoaded += this.SceneManager_sceneLoaded;
	}

	private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
	{
		SceneManager.sceneLoaded -= this.SceneManager_sceneLoaded;
		var obj = new GameObject(nameof(ConsoleCommandListener)).AddComponent<ConsoleCommandListener>();
		DontDestroyOnLoad(obj);
	}
}
