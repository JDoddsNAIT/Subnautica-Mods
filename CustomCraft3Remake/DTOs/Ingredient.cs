using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FrootLuips.CustomCraft3Remake.DTOs;
#nullable enable
internal sealed class Ingredient : IJsonData<CraftData.Ingredient?>
{
	private const string _BAD_ID = "Ingredient ignored";

	public string ItemId { get; set; }
	public int Amount { get; set; }

	[Newtonsoft.Json.JsonConstructor]
	public Ingredient(string itemId, int amount = 1)
	{
		this.ItemId = itemId;
		this.Amount = amount;
	}

	public Ingredient(TechType itemId, int amount = 1) : this(itemId.ToString(), amount)
	{ }

	public bool TryConvert([NotNull] in List<string> errors, out CraftData.Ingredient? result)
	{
		if (Validation.TryParseTechType(ItemId, out var techType).Fails())
		{
			errors.Add(Validation.InvalidIDMessage(ItemId));
			result = null;
			return false;
		}

		result = new CraftData.Ingredient(techType, Amount);
		return true;
	}

	public static explicit operator Ingredient(CraftData.Ingredient ingredient)
	{
		return new Ingredient(ingredient.techType.ToString(), ingredient.amount);
	}
}
