using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;

namespace ChaosMod;
[BepInPlugin(GUID, NAME, VERSION)]
[BepInDependency("com.snmodding.nautilus")]
public class Plugin : BaseUnityPlugin
{
	public const string GUID = "com.github.frootluips.ChaosMod";
	public const string NAME = "FrootLuips' Chaos Mod";
	public const string VERSION = "1.0.0";

	private void Awake()
	{
		
	}
}
