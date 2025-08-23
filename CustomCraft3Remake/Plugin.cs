using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using FrootLuips.CustomCraft3Remake.DTOs;
using Nautilus.Handlers;

namespace FrootLuips.CustomCraft3Remake;
[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus")]
internal sealed partial class Plugin : BaseUnityPlugin
{
	internal const string
		SAMPLES_DIR = "Samples",
		CONVERSION_DIR = "CustomCraft3",
		REFERENCES_DIR = "References",
		WORKING_DIR = "Working Files",
		CRAFT_TREE_DIR = "Custom Groups",
		ITEMS_DIR = "Custom Items",
		SIZE_DIR = "Custom Sizes",
		RECIPES_DIR = "Custom Recipes";

	private const string _JSON = ".json";

	private readonly string[] _sampleFileNames = new[] {
		"Sample Groups" + _JSON,
		"Sample Items" + _JSON,
		"Sample Sizes" + _JSON,
		"Sample Recipes" + _JSON,
		"TechType Reference" + _JSON,
		"Fabricator Reference" + _JSON,
		"TechGroup Reference" + _JSON,
		"TechCategory Reference" + _JSON
	};

	public new static ManualLogSource Logger { get; private set; }
	public static CustomCraftService Service { get; private set; }
	public static PluginOptions Cfg { get; private set; }

	public static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

	private static bool _convert_firstLoad = true, _items_firstLoad = true, _craftTree_firstLoad = true;

	internal static string samplesDir, craftTreeDir, itemsDir, sizeDir, recipesDir;

	internal void Awake()
	{
		Cfg = OptionsPanelHandler.RegisterModOptions<PluginOptions>();

		Logger = base.Logger;
		Service = new(Logger);

		GetFilePaths();
		CreateMissingFolders();
		GenerateSamples();

		WaitScreenHandler.RegisterEarlyLoadTask(PluginInfo.PLUGIN_NAME, ConvertCC3Data, "Converting Files");
		WaitScreenHandler.RegisterEarlyLoadTask(PluginInfo.PLUGIN_NAME, RegisterCraftTree, "Modifying Craft Tree");
		WaitScreenHandler.RegisterEarlyLoadTask(PluginInfo.PLUGIN_NAME, RegisterItems, "Registering New Items");
		WaitScreenHandler.RegisterEarlyLoadTask(PluginInfo.PLUGIN_NAME, RegisterRecipes, "Assigning Recipes");
		WaitScreenHandler.RegisterEarlyLoadTask(PluginInfo.PLUGIN_NAME, RegisterSizes, "Setting Custom Sizes");

		Logger.LogInfo($"Initialization complete.");
	}

	private void GetFilePaths()
	{
		samplesDir = Utilities.PrependPluginPath(SAMPLES_DIR);

		craftTreeDir = Utilities.PrependPluginPath(WORKING_DIR, CRAFT_TREE_DIR);
		itemsDir = Utilities.PrependPluginPath(WORKING_DIR, ITEMS_DIR);
		sizeDir = Utilities.PrependPluginPath(WORKING_DIR, SIZE_DIR);
		recipesDir = Utilities.PrependPluginPath(WORKING_DIR, RECIPES_DIR);

		for (int i = 0; i < _sampleFileNames.Length; i++)
		{
			string subDir = i switch {
				< 0 => throw new ArgumentOutOfRangeException(nameof(i)),
				0 => CRAFT_TREE_DIR,
				1 => ITEMS_DIR,
				2 => SIZE_DIR,
				3 => RECIPES_DIR,
				>= 4 => REFERENCES_DIR,
			};

			_sampleFileNames[i] = Path.Combine(samplesDir, subDir, _sampleFileNames[i]);
		}
	}

	private void CreateMissingFolders()
	{
		var created = Utilities.CreateFoldersIfMissing(craftTreeDir, itemsDir, sizeDir, recipesDir);

		Logger.LogDebug($"Created {created} missing folders.");
	}

	private void GenerateSamples()
	{
		try
		{
			Utilities.GenerateSamples(_sampleFileNames);
		}
		catch (Exception ex)
		{
			Logger.LogWarning(new LogMessage(context: ex.Message, notice: "Failed to generate samples", message: "Skipping"));
		}
	}

	private void ConvertCC3Data(WaitScreenHandler.WaitScreenTask task)
	{
		if (!_convert_firstLoad)
			return;

		CustomCraftConversion.Converter.ConvertFiles();

		_convert_firstLoad = false;
	}

	private void RegisterCraftTree(WaitScreenHandler.WaitScreenTask task)
	{
		if (!_craftTree_firstLoad)
			return;

		Logger.LogDebug($"Registering Fabricator Groups...");
		var stopwatch = System.Diagnostics.Stopwatch.StartNew();

		Validation.AssertDirectoryExists(craftTreeDir);
		List<string> errors = new();
		List<DTOs.CustomGroupData> nodes = new();

		foreach (var filePath in Directory.EnumerateFiles(craftTreeDir))
		{
			Service.Internal_LoadData<CustomGroupData, CustomGroup>(filePath, errors, nodes);
		}

		stopwatch.Stop();
		Logger.LogDebug($"Registered Fabricator Groups in {stopwatch.ElapsedMilliseconds} ms.");

		_craftTree_firstLoad = false;
	}

	private void RegisterItems(WaitScreenHandler.WaitScreenTask task)
	{
		if (!_items_firstLoad)
			return;

		Logger.LogDebug($"Registering Custom Items...");
		var stopwatch = System.Diagnostics.Stopwatch.StartNew();

		Validation.AssertDirectoryExists(itemsDir);
		List<string> errors = new();
		List<CustomItemData> items = new();

		foreach (var filePath in Directory.EnumerateFiles(itemsDir))
		{
			Service.Internal_LoadData<CustomItemData, CustomItem>(filePath, errors, items);
		}

		stopwatch.Stop();
		Logger.LogDebug($"Registered item data in {stopwatch.ElapsedMilliseconds} ms.");

		_items_firstLoad = false;
	}

	private void RegisterSizes(WaitScreenHandler.WaitScreenTask task)
	{
		Logger.LogDebug($"Registering Custom Sizes...");
		var stopwatch = System.Diagnostics.Stopwatch.StartNew();

		Validation.AssertDirectoryExists(sizeDir);
		List<string> errors = new();
		List<DTOs.CustomSizeData> sizes = new();

		foreach (var filePath in Directory.EnumerateFiles(sizeDir))
		{
			Service.Internal_LoadData<CustomSizeData, CustomSize>(filePath, errors, sizes);
		}

		stopwatch.Stop();
		Logger.LogDebug($"Registered custom size data in {stopwatch.ElapsedMilliseconds} ms.");
	}

	private void RegisterRecipes(WaitScreenHandler.WaitScreenTask task)
	{
		Logger.LogDebug($"Registering Custom Recipes...");
		var stopwatch = System.Diagnostics.Stopwatch.StartNew();

		Validation.AssertDirectoryExists(recipesDir);
		var errors = new List<string>();
		var recipes = new List<DTOs.CustomRecipeData>();

		foreach (var filePath in Directory.EnumerateFiles(recipesDir))
		{
			Service.Internal_LoadData<CustomRecipeData, CustomRecipe>(filePath, errors, recipes);
		}

		stopwatch.Stop();
		Logger.LogDebug($"Registered recipe data in {stopwatch.ElapsedMilliseconds} ms.");
	}
}
