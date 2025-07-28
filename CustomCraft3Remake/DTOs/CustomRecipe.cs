using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Nautilus.Crafting;
using Nautilus.Handlers;

namespace FrootLuips.CustomCraft3Remake.DTOs;
#nullable enable
internal sealed class CustomRecipeData : IRegisterable<CustomRecipe>
{
	private const string _INVALID_RECIPE = "Recipe is invalid";

	public static readonly string[] DefaultFabricatorPath = new[] { "Custom" };

	public string ItemId { get; set; }

	public float? CraftSeconds { get; set; }
	public int? CraftAmount { get; set; }
	public Ingredient[] Ingredients { get; set; }
	public string[] LinkedItems { get; set; }
	public string[] RequiredTech { get; set; }

	public string? Fabricator { get; set; }
	public string[] FabricatorPath { get; set; }

	[Newtonsoft.Json.JsonConstructor]
	public CustomRecipeData(
		string itemId,
		Ingredient[] ingredients,
		string[]? linkedItems = null,
		int craftAmount = 1,
		float craftSeconds = 1.0f,
		string[]? requiredTech = null,
		string? fabricator = null,
		string[]? fabricatorPath = null)
	{
		this.ItemId = itemId;
		this.Ingredients = ingredients ?? new Ingredient[0];

		this.LinkedItems = linkedItems ?? new string[0];

		this.RequiredTech = requiredTech ?? new string[0];

		this.CraftSeconds = craftSeconds;
		this.CraftAmount = craftAmount;

		this.Fabricator = fabricator;
		FabricatorPath = new string[DefaultFabricatorPath.Length];
		if (fabricatorPath is null)
			DefaultFabricatorPath.CopyTo(this.FabricatorPath, 0);
		else
			this.FabricatorPath = fabricatorPath;
	}

	public CustomRecipeData(string itemId, RecipeData recipeData, float craftSeconds, CraftTree.Type fabricator, string[] fabricatorPath, TechType[] requiredTech)
		: this(
			itemId: itemId,
			ingredients: recipeData.Ingredients.SimpleSelect(i => (Ingredient)i),
			linkedItems: recipeData.LinkedItems.ToStringArray(),
			craftAmount: recipeData.craftAmount,
			craftSeconds: craftSeconds,
			requiredTech: requiredTech.ToStringArray(),
			fabricator: fabricator.ToString(),
			fabricatorPath: fabricatorPath
		)
	{ }

	public bool TryConvert([NotNull] in List<string> errors, [MaybeNullWhen(false)] out CustomRecipe result)
	{
		result = null;

		// ItemId
		if (Validation.TryParseTechType(ItemId, out var item).Fails())
		{
			errors.Add(Validation.InvalidIDMessage(ItemId, notice: _INVALID_RECIPE));
			return false;
		}

		// Ingredients
		var subErrors = new List<string>();
		Ingredients ??= Array.Empty<Ingredient>();
		var validIngredients = Ingredients.TrySelect((Ingredient i, out CraftData.Ingredient? r) => ValidateIngredient(i, out r, subErrors));

		if (validIngredients.Count == 0)
		{
			errors.Add(new LogMessage(notice: _INVALID_RECIPE, message: "Recipe has no ingredients."));
			return false;
		}
		if (subErrors.Count > 0)
		{
			errors.Add(new LogMessage(
				notice: $"Some {nameof(Ingredients)} have been ignored",
				message: Utilities.JoinErrors(subErrors)
			));
		}

		// LinkedItems
		subErrors.Clear();
		LinkedItems ??= Array.Empty<string>();
		var validLinkedItems = LinkedItems.TrySelect((string itemId, out TechType item1) => ValidateItem(itemId, out item1, subErrors));
		if (subErrors.Count > 0)
		{
			errors.Add(new LogMessage(
				notice: $"Some {nameof(LinkedItems)} have been ignored",
				message: Utilities.JoinErrors(subErrors)
			));
		}

		var recipe = new RecipeData() {
			craftAmount = CraftAmount ?? -1,
			Ingredients = validIngredients,
			LinkedItems = validLinkedItems
		};

		// RequiredTech
		subErrors.Clear();
		RequiredTech ??= Array.Empty<string>();
		var validRequiredTech = RequiredTech.TrySelect((string itemId, out TechType item1) => ValidateItem(itemId, out item1, subErrors)).ToArray();
		if (subErrors.Count > 0)
		{
			errors.Add(new LogMessage(
				notice: $"Some {nameof(RequiredTech)} has been ignored",
				message: Utilities.JoinErrors(subErrors)
			));
		}

		// Fabricator
		if (Validation.ParseFabricator(Fabricator, out var fabricator).Fails())
		{
			errors.Add(Validation.InvalidFabricatorMessage(Fabricator));
		}
		FabricatorPath ??= Array.Empty<string>();

		result = new CustomRecipe(item, recipe, CraftSeconds ?? -1, validRequiredTech, fabricator, FabricatorPath);
		return true;
	}

	private static bool ValidateItem(string itemId, out TechType item, List<string> subErrors)
	{
		bool validItem = Validation.TryParseTechType(itemId, out item);
		if (!validItem)
		{
			subErrors.Add(Validation.InvalidIDMessage(itemId));
		}
		return validItem;
	}

	private static bool ValidateIngredient(Ingredient ingredient, out CraftData.Ingredient? result, List<string> subErrors)
	{
		result = null;
		return ingredient is not null && ingredient.TryConvert(subErrors, out result);
	}

	public string GetId() => this.ItemId;

	public void Register(List<string> errors, CustomRecipe item)
	{
		if (item.Fabricator != CraftTree.Type.None)
		{
			Utilities.CreateMissingTabs(item.Fabricator, item.FabricatorPath, errors);
			CraftTreeHandler.AddCraftingNode(item.Fabricator, item.ItemId, item.FabricatorPath);
		}

		CraftDataHandler.SetRecipeData(item.ItemId, item.Recipe);

		if (item.RequiredTech.Length > 0)
		{
			KnownTechHandler.RemoveAllCurrentAnalysisTechEntry(item.ItemId);
			KnownTechHandler.SetCompoundUnlock(item.ItemId, new(item.RequiredTech));
		}

		if (item.CraftTime > 0)
		{
			CraftDataHandler.SetCraftingTime(item.ItemId, item.CraftTime);
		}
	}
}

internal sealed record class CustomRecipe(TechType ItemId, RecipeData Recipe, float CraftTime, TechType[] RequiredTech, CraftTree.Type Fabricator, string[] FabricatorPath);
