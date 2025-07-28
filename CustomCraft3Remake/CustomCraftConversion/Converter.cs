using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FrootLuips.CustomCraft3Remake.DTOs;
using Nautilus.Json.ExtensionMethods;

namespace FrootLuips.CustomCraft3Remake.CustomCraftConversion;

internal class Converter
{
	internal const string NEW_RECIPES = "NewRecipes.json",
		CUSTOM_SIZES = "CustomSizes.json",
		MODIFIED_RECIPES = "ModifiedRecipes.json",
		CONVERTED_FILENAME = "CustomCraft3.json",
		CONVERSION_FAILED_NOTICE = "Failed to convert CustomCraft3 data";

	private static readonly string _conversionDir = Utilities.PrependPluginPath(Plugin.WORKING_DIR, Plugin.CONVERSION_DIR);

	public void ConvertFiles()
	{
		if (!Directory.Exists(_conversionDir) || Directory.EnumerateFiles(_conversionDir).Count() == 0)
		{
			Plugin.Logger.LogDebug(new LogMessage(notice: "No files to convert", message: "Skipping"));
			return;
		}

		var stopwatch = System.Diagnostics.Stopwatch.StartNew();

		var customItems = new List<CustomItemData>();
		var customRecipes = new List<CustomRecipeData>();
		var customSizes = new List<CustomSizeData>();

		try
		{
			string newRecipesPath = Path.Combine(_conversionDir, NEW_RECIPES);
			ConvertNewRecipes(newRecipesPath, customItems, customRecipes, customSizes);
		}
		catch (Exception ex)
		{
			Plugin.Logger.LogError(new LogMessage(notice: CONVERSION_FAILED_NOTICE, message: ex.ToString()));
		}

		try
		{
			string modifiedRecipesPath = Path.Combine(_conversionDir, MODIFIED_RECIPES);
			ConvertModifiedRecipes(modifiedRecipesPath, customRecipes);
		}
		catch (Exception ex)
		{
			Plugin.Logger.LogError(new LogMessage(notice: CONVERSION_FAILED_NOTICE, message: ex.ToString()));
		}

		try
		{
			string sizesPath = Path.Combine(_conversionDir, CUSTOM_SIZES);
			ConvertSizes(sizesPath, customSizes);
		}
		catch (Exception ex)
		{
			Plugin.Logger.LogError(new LogMessage(notice: CONVERSION_FAILED_NOTICE, message: ex.ToString()));
		}

		Finalize(customItems, customRecipes, customSizes);
		stopwatch.Stop();
		Plugin.Logger.LogDebug(new LogMessage(notice: "Conversion complete").WithMessage("Converted CC3 files in ", stopwatch.ElapsedMilliseconds, " ms."));
	}

	private void ConvertNewRecipes(string filePath, List<CustomItemData> customItems, List<CustomRecipeData> customRecipes, List<CustomSizeData> customSizes)
	{
		if (!File.Exists(filePath))
			return;

		List<NewRecipe> newRecipes = new();
		newRecipes.LoadJson(filePath);

		for (int i = 0; i < newRecipes.Count; i++)
		{
			var newRecipe = newRecipes[i];

			var customItem = new CustomItemData(
				itemId: newRecipe.Name,
				modelId: newRecipe.Model,
				iconId: newRecipe.Icon,
				displayName: newRecipe.Name,
				description: newRecipe.Description,
				data: new DTOs.PDAData(newRecipe.PdaGroupCategory.TechGroup,
					newRecipe.PdaGroupCategory.TechCategory)
			);
			customItems.Add(customItem);

			var customRecipe = new CustomRecipeData(
				itemId: newRecipe.Name,
				ingredients: newRecipe.Ingredients.SimpleSelect(ing => new DTOs.Ingredient(ing.Name, ing.Amount)),
				craftAmount: newRecipe.CraftAmount,
				craftSeconds: newRecipe.CraftTimeSeconds,
				requiredTech: new string[0],
				fabricator: newRecipe.FabricatorType,
				fabricatorPath: newRecipe.FabricatorPath
			);
			customRecipes.Add(customRecipe);

			var customSize = new CustomSizeData(
				itemId: newRecipe.Name,
				width: newRecipe.Width,
				height: newRecipe.Height
			);
			customSizes.Add(customSize);
		}

		if (Plugin.Cfg.RemoveConvertedFiles)
			File.Delete(filePath);
	}

	private void ConvertModifiedRecipes(string filePath, List<CustomRecipeData> customRecipes)
	{
		if (!File.Exists(filePath))
			return;

		var recipes = new List<Recipe>();
		recipes.LoadJson(filePath);

		for (int i = 0; i < recipes.Count; i++)
		{
			var recipe = recipes[i];

			var customRecipe = new CustomRecipeData(
				itemId: recipe.Name,
				ingredients: recipe.Ingredients.SimpleSelect(ing => new DTOs.Ingredient(ing.Name, ing.Amount)),
				craftAmount: recipe.CraftAmount,
				craftSeconds: -1
			);
			customRecipes.Add(customRecipe);
		}

		if (Plugin.Cfg.RemoveConvertedFiles)
			File.Delete(filePath);
	}

	private void ConvertSizes(string filePath, List<CustomSizeData> customSizes)
	{
		if (!File.Exists(filePath))
			return;

		var sizes = new List<Size>();
		sizes.LoadJson(filePath);

		for (int i = 0; i < sizes.Count; i++)
		{
			var size = sizes[i];

			var customSize = new CustomSizeData(size.Name, size.Width, size.Height);
			customSizes.Add(customSize);
		}

		if (Plugin.Cfg.RemoveConvertedFiles)
			File.Delete(filePath);
	}

	private void Finalize(List<CustomItemData> customItems, List<CustomRecipeData> customRecipes, List<CustomSizeData> customSizes)
	{
		string filePath;
		if (!customItems.IsNullOrEmpty())
		{
			filePath = Path.Combine(Plugin.itemsDir, CONVERTED_FILENAME);
			customItems.SaveJson(filePath);
		}

		if (!customRecipes.IsNullOrEmpty())
		{
			filePath = Path.Combine(Plugin.recipesDir, CONVERTED_FILENAME);
			customRecipes.SaveJson(filePath);
		}

		if (!customSizes.IsNullOrEmpty())
		{
			filePath = Path.Combine(Plugin.sizeDir, CONVERTED_FILENAME);
			customSizes.SaveJson(filePath);
		}
	}
}
