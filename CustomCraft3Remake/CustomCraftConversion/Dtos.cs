namespace FrootLuips.CustomCraft3Remake.CustomCraftConversion;

internal class Ingredient
{
	public string Name { get; set; }
	public int Amount { get; set; }
}

internal class NewRecipe
{
	public string Name { get; set; }
	public string Description { get; set; }
	public int Width { get; set; }
	public int Height { get; set; }
	public string Icon { get; set; }
	public string Model { get; set; }
	public Ingredient[] Ingredients { get; set; }
	public string[] LinkedItems { get; set; }
	public int CraftAmount { get; set; }
	public string FabricatorType { get; set; }
	public string[] FabricatorPath { get; set; }
	public float CraftTimeSeconds { get; set; }
	public PdaGroupCategory PdaGroupCategory { get; set; }
}

internal class PdaGroupCategory
{
	public string TechCategory { get; set; }
	public string TechGroup { get; set; }
}

internal class Recipe
{
	public string Name { get; set; }
	public int CraftAmount { get; set; }
	public Ingredient[] Ingredients { get; set; }
	public string[] LinkedItems { get; set; }
}

internal class Size
{
	public string Name { get; set; }
	public int Width { get; set; }
	public int Height { get; set; }
}