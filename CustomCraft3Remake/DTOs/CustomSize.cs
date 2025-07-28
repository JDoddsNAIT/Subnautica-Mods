using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Nautilus.Handlers;

namespace FrootLuips.CustomCraft3Remake.DTOs;
#nullable enable
internal sealed class CustomSizeData : IRegisterable<CustomSize>
{
	// notice messages
	const string _BAD_ID = "Cannot set size";

	public string ItemId { get; set; }
	public int Width { get; set; }
	public int Height { get; set; }

	[Newtonsoft.Json.JsonConstructor]
	public CustomSizeData(string itemId, int width = 1, int height = 1)
	{
		this.ItemId = itemId;
		this.Width = width;
		this.Height = height;
	}

	public CustomSizeData(TechType item, int width = 1, int height = 1)
		: this(item.ToString(), width, height)
	{ }

	public bool TryConvert([NotNull] in List<string> errors, [MaybeNullWhen(false)] out CustomSize result)
	{
		if (Validation.TryParseTechType(ItemId, out var techType).Fails())
		{
			errors.Add(Validation.InvalidIDMessage(ItemId));
			result = null;
			return false;
		}

		result = new CustomSize(techType, Width, Height);
		return true;
	}

	public string GetId() => ItemId;

	public void Register(List<string> errors, CustomSize item)
	{
		CraftDataHandler.SetItemSize(item.ItemId, item.Width, item.Height);
	}
}

internal sealed record class CustomSize(TechType ItemId, int Width, int Height);
