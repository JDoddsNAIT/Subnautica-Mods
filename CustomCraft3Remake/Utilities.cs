using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using FrootLuips.CustomCraft3Remake.DTOs;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Json.ExtensionMethods;

namespace FrootLuips.CustomCraft3Remake;

internal static class Utilities
{
	public const TechType TECH_TYPE_FALLBACK = TechType.None;

	private static string _rootDir = null;

	public static string PrependPluginPath(params string[] path)
	{
		_rootDir ??= Path.Combine(Paths.PluginPath, PluginInfo.PLUGIN_GUID);
		Validation.AssertDirectoryExists(_rootDir);

		var fullPath = path.Length > 1 ? Path.Combine(path) : path[0];
		fullPath = Path.Combine(_rootDir, fullPath);
		return fullPath;
	}

	public static int CreateFoldersIfMissing(params string[] paths)
	{
		if (paths.Count(string.IsNullOrEmpty) > 0)
			throw new ArgumentNullException(nameof(paths));

		return paths.Count(dir => {
			bool created = !Directory.Exists(dir);
			if (created)
				Directory.CreateDirectory(dir);
			return created;
		});
	}

	public static string JoinErrors(List<string> errors)
	{
		return string.Join(Validation.ERROR_SEPARATOR, errors);
	}

	public static void GenerateSamples(string[] sampleFileNames)
	{
		Plugin.Logger.LogDebug($"Generating samples...");
		var stopwatch = System.Diagnostics.Stopwatch.StartNew();

		// TechType reference is always generated
		int samplesGenerated = 1;

		static bool shouldRegenerate(string path) => Plugin.Cfg.RegenerateSamples switch {
			PluginOptions.RegenSamples.Never => false,
			PluginOptions.RegenSamples.RestoreMissing => !File.Exists(path),
			PluginOptions.RegenSamples.RepairExisting => File.Exists(path),
			PluginOptions.RegenSamples.Always => true,
			_ => throw new AssertionFailedException("Enum value is out of range."),
		};

		// Craft Tree
		if (shouldRegenerate(sampleFileNames[0]))
		{
			List<DTOs.CustomGroupData> treeSample = new() {
				new("Cyclops", icon: TechType.CyclopsBlueprint, fabricator: CraftTree.Type.Workbench)
			};
			treeSample.SaveJson(sampleFileNames[0]);
			samplesGenerated++;
		}

		const string hullKitId = "CyclopsHullKit";
		const string bridgeKitId = "CyclopsBridgeKit";
		const string engineKitId = "CyclopsEngineKit";

		// New Items
		if (shouldRegenerate(sampleFileNames[1]))
		{
			var pdaData = new DTOs.PDAData(TechGroup.Cyclops, TechCategory.Cyclops, unlockAtStart: false);

			List<CustomItemData> newItemSample = new() {
				new(itemId: hullKitId,
					model: TechType.SmallStorage,
					icon: TechType.CyclopsHullBlueprint,
					displayName: "Cyclops Hull Kit",
					description: "A kit that contains structural materials necessary to build the Cyclops.",
					data: pdaData
				),

				new(itemId: bridgeKitId,
					model: TechType.SmallStorage,
					icon: TechType.CyclopsBridgeBlueprint,
					displayName: "Cyclops Bridge Kit",
					description: "A kit that contains electrical components necessary to build the Cyclops.",
					data: pdaData
				),

				new(itemId:  engineKitId,
					model: TechType.SmallStorage,
					icon: TechType.CyclopsEngineBlueprint,
					displayName: "Cyclops Engine Kit",
					description: "A kit that contains machine parts necessary to build the Cyclops.",
					data: pdaData
				),
			};
			newItemSample.SaveJson(sampleFileNames[1]);
			samplesGenerated++;
		}

		// Custom Sizes
		if (shouldRegenerate(sampleFileNames[2]))
		{
			List<CustomSizeData> sizeSample = new() {
				new(TechType.Seaglide, 3, 2),
				new(TechType.PowerCell, 1, 2),
				new(TechType.PrecursorIonPowerCell, 1, 2),

				new(hullKitId, 2, 2),
				new(bridgeKitId, 2, 2),
				new(engineKitId, 2, 2),
			};
			sizeSample.SaveJson(sampleFileNames[2]);
			samplesGenerated++;
		}

		// Custom Recipes
		if (shouldRegenerate(sampleFileNames[3]))
		{
			var fabricatorPath = new string[0];
			var cyclopsRequiredTech = new[] {
				TechType.CyclopsHullBlueprint,
				TechType.CyclopsEngineBlueprint,
				TechType.CyclopsBridgeBlueprint,
			};

			List<CustomRecipeData> recipeSample = new() {
				// Hull Kit
				new CustomRecipeData(itemId: hullKitId,
					recipeData: new RecipeData() {
						craftAmount = 1,
						Ingredients = new() {
							new(TechType.SmallStorage, 1),
							new(TechType.TitaniumIngot, 1),
							new(TechType.PlasteelIngot, 1),
							new(TechType.Lead, 1),
							new(TechType.Diamond, 1),
							new(TechType.EnameledGlass, 1),
						},
						LinkedItems = new(),
					},
					craftSeconds: 20.0f,
					fabricator: CraftTree.Type.Workbench,
					fabricatorPath: fabricatorPath,
					requiredTech: cyclopsRequiredTech
				),
				
				// Bridge kit
				new CustomRecipeData(itemId: bridgeKitId,
					recipeData: new RecipeData() {
						Ingredients = new() {
							new(TechType.SmallStorage, 1),
							new(TechType.SeamothSonarModule, 1),
							new(TechType.ComputerChip, 2),
							new(TechType.CopperWire, 2),
							new(TechType.AdvancedWiringKit, 2),
							new(TechType.PowerCell, 6)
						},
					},
					craftSeconds: 10.0f,
					fabricator: CraftTree.Type.Workbench,
					fabricatorPath: fabricatorPath,
					requiredTech: cyclopsRequiredTech
				),

				// Engine Kit
				new CustomRecipeData(itemId: engineKitId,
					recipeData: new RecipeData() {
						Ingredients = new() {
							new(TechType.SmallStorage, 1),
							new(TechType.Lubricant, 4),
							new(TechType.AramidFibers, 2),
							new(TechType.Silicone, 2),
							new(TechType.Aerogel, 4)
						}
					},
					craftSeconds: 30.0f,
					fabricator: CraftTree.Type.Workbench,
					fabricatorPath: fabricatorPath,
					requiredTech: cyclopsRequiredTech
				),

				// Cyclops
				new CustomRecipeData(itemId: TechType.Cyclops.ToString(),
				ingredients: new[] {
					new DTOs.Ingredient(hullKitId, 4),
					new DTOs.Ingredient(bridgeKitId, 1),
					new DTOs.Ingredient(engineKitId, 1)
				},
				craftAmount: -1,
				craftSeconds: -1)
			};
			recipeSample.SaveJson(sampleFileNames[3]);
			samplesGenerated++;
		}

		// References
		var techTypes = new List<string>(GetNonObsoleteTechTypes().Select(t => t.ToString()));
		techTypes.SaveJson(sampleFileNames[4]);
		if (shouldRegenerate(sampleFileNames[5]))
		{
			GetFullCraftTree().SaveJson(sampleFileNames[5]);
			samplesGenerated++;
		}

		if (shouldRegenerate(sampleFileNames[6]))
		{
			Enum.GetNames(typeof(TechGroup)).SaveJson(sampleFileNames[6]);
			samplesGenerated++;
		}

		if (shouldRegenerate(sampleFileNames[7]))
		{
			Enum.GetNames(typeof(TechCategory)).SaveJson(sampleFileNames[7]);
			samplesGenerated++;
		}



		stopwatch.Stop();
		Plugin.Logger.LogDebug($"{samplesGenerated} samples generated in {stopwatch.ElapsedMilliseconds} ms.");
	}

#nullable enable
	public static Dictionary<CraftTree.Type, string[][]> GetFullCraftTree()
	{
		var craftTree = new Dictionary<CraftTree.Type, string[][]>();
		foreach (CraftTree.Type treeType in Enum.GetValues(typeof(CraftTree.Type)))
		{
			if (treeType is CraftTree.Type.None)
				continue;
			var tree = CraftTree.GetTree(treeType);
			if (tree == null)
				continue;

			var treePaths = new List<string>();
			string startingPath = treeType.ToString();

			TraverseTreeRecursive(tree.nodes, startingPath, treePaths);

			craftTree.Add(treeType, treePaths.SimpleSelect(p => p.Split('/')[1..]));
		}
		return craftTree;
	}

	public static void TraverseTreeRecursive(TreeNode node, string nodePath, List<string> treePaths)
	{
		if (node == null)
			throw new ArgumentNullException(nameof(node));

		nodePath = string.Join("/", nodePath, node.id);
		if (node.childCount == 0)
		{
			treePaths.Add(nodePath);
			return;
		}

		for (int i = 0; i < node.childCount; i++)
		{
			TraverseTreeRecursive(node[i], nodePath, treePaths);
		}
	}

	public static void CreateMissingTabs(CraftTree.Type fabricator, string[] path, List<string> errors)
	{
		CraftTree tree = CraftTree.GetTree(fabricator)!;
		TreeNode node = tree.nodes;

		for (int i = 0; i < path.Length; i++)
		{
			if (!node.ContainsChild(path[i], out var child))
			{
				errors.Add($"Created missing tab '{path[i]}' in {fabricator}/{string.Join("/", path[..i])}");

				CraftTreeHandler.AddTabNode(
					craftTree: fabricator,
					name: path[i],
					displayName: path[i],
					sprite: SpriteManager.defaultSprite,
					stepsToTab: path[..i]);

				child = new CraftNode(path[i], TreeAction.Expand);
				node.AddNode(child);
			}
			node = child;
		}
	}

	private static void CreateTabsRecursive(CraftTree.Type fabricator, TreeNode node, string[] path, int depth, Action<int> creationCallback)
	{
		if (depth >= path.Length)
		{
			return;
		}
		else if (node.ContainsChild(path[depth], out TreeNode child))
		{
			CreateTabsRecursive(fabricator, child, path, depth + 1, creationCallback);
		}
		else
		{
			// Create the tabs
			for (int i = 0; i < path.Length; i++)
			{
				var pathPart = path[i];
				CraftTreeHandler.AddTabNode(
					craftTree: fabricator,
					name: pathPart,
					displayName: pathPart,
					sprite: SpriteManager.defaultSprite,
					stepsToTab: path[..i]);
			}
			creationCallback(depth);
		}
	}
#nullable disable

	public static bool ContainsChild(this TreeNode node, string childName, out TreeNode child)
	{
		child = null;
		if (node.childCount == 0)
			return false;
		bool found = false;

		for (int i = 0; i < node.childCount && !found; i++)
		{
			if (node[i].id == childName)
			{
				child = node[i];
				found = true;
			}
		}
		return found;
	}

	public static DTOs.Ingredient ToIngredient(this TechType type, int count = 1) => new(type, count);

	public static IEnumerable<TechType> GetNonObsoleteTechTypes()
	{
		var type = typeof(TechType);
		foreach (TechType techType in Enum.GetValues(type))
		{
			if (!techType.IsObsolete())
				yield return techType;
		}
	}

	public static T[] GetSubArray<T>(this IReadOnlyList<T> values, System.Range range)
	{
		(int offset, int length) = range.GetOffsetAndLength(values.Count);
		var result = new T[length];
		for (int i = 0; i < length; i++)
		{
			result[i] = values[i + offset];
		}
		return result;
	}

	public static string[] ToStringArray<T>(this IReadOnlyList<T> array)
	{
		return array.SimpleSelect(x => x.ToString());
	}

	public static TResult[] SimpleSelect<TValue, TResult>(this IReadOnlyList<TValue> values, Func<TValue, TResult> selector)
	{
		if (selector is null) throw new ArgumentNullException(nameof(selector));

		var result = new TResult[values.Count];
		for (int i = 0; i < values.Count; i++)
		{
			result[i] = selector(values[i]);
		}
		return result;
	}

	public static List<TValue> SimpleWhere<TValue>(this IReadOnlyList<TValue> values, Predicate<TValue> predicate, List<TValue> list = null)
	{
		if (predicate is null) throw new ArgumentNullException(nameof(predicate));

		list ??= new List<TValue>();
		for (int i = 0; i < values.Count; i++)
		{
			if (predicate(values[i]))
				list.Add(values[i]);
		}
		return list;
	}

	public delegate bool TryFunc<in TValue, TResult>(TValue value, out TResult result);
	public static List<TResult> TrySelect<TValue, TResult>(this IReadOnlyList<TValue> values, TryFunc<TValue, TResult> selector, List<TResult> list = null)
	{
		if (selector is null) throw new ArgumentNullException(nameof(selector));

		list ??= new List<TResult>();
		for (int i = 0; i < values.Count; i++)
		{
			if (selector(values[i], out TResult result))
				list.Add(result);
		}
		return list;
	}

	public static bool IsNullOrEmpty<T>(this IReadOnlyCollection<T> values) => values == null || values.Count == 0;
}
