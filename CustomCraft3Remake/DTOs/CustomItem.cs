using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FrootLuips.CustomCraft3Remake.DTOs;
#nullable enable
internal sealed class CustomItemData : IRegisterable<CustomItem>
{
	private const string _BAD_MODEL = "Invalid Model";
	private const string _BAD_ICON = "Unknown icon";

	public string ItemId { get; set; }
	public string DisplayName { get; set; }
	public string Description { get; set; }
	public string ModelId { get; set; }
	public string? IconId { get; set; }
	public PDAData? PDAData { get; set; }

	[Newtonsoft.Json.JsonConstructor]
	public CustomItemData(
		string itemId,
		string modelId,
		string? iconId = null,
		string? displayName = null,
		string description = "",
		PDAData? data = null)
	{
		this.ItemId = itemId;
		this.DisplayName = displayName ?? itemId;
		this.Description = description;
		this.ModelId = modelId;
		this.IconId = iconId;
		this.PDAData = data;
	}

	public CustomItemData(string itemId, TechType model, TechType icon = TechType.None, string? displayName = null, string description = "", PDAData? data = null)
		: this(
			itemId: itemId,
			modelId: model.ToString(),
			iconId: icon is TechType.None ? "" : icon.ToString(),
			displayName: displayName ?? itemId,
			description: description,
			data: data)
	{ }

	public bool TryConvert([NotNull] in List<string> errors, [MaybeNullWhen(false)] out CustomItem result)
	{
		result = null;

		if (Validation.TryParseTechType(ModelId, out var model).Fails())
		{
			errors.Add(Validation.InvalidIDMessage(ModelId, notice: _BAD_MODEL));
			return false;
		}

		var subErrors = new List<string>();
		if (PDAData == null || !PDAData.TryConvert(subErrors, out var data))
		{
			errors.Add(new LogMessage()
				.WithMessage(nameof(PDAData), " will be ignored: ", Utilities.JoinErrors(subErrors)));
			data = null;
		}
		else if (subErrors.Count > 0)
		{
			errors.Add(Utilities.JoinErrors(subErrors));
		}

		if (Validation.TryParseTechType(IconId, out var icon).Fails())
		{
			errors.Add(Validation.InvalidIDMessage(IconId, notice: _BAD_ICON));
			icon = Validation.DEFAULT_TECH_TYPE;
		}

		if (string.IsNullOrEmpty(DisplayName))
		{
			DisplayName = ItemId;
		}

		Description ??= string.Empty;

		result = new CustomItem(ItemId, model, icon, DisplayName, Description, data);
		return true;
	}

	public string GetId() => this.ItemId;

	public void Register(List<string> errors, CustomItem item) => Item.Register(item, System.Reflection.Assembly.GetExecutingAssembly());
}

internal sealed record class CustomItem(string ItemId, TechType Model, TechType Icon, string DisplayName, string Description, ValidPDAData? PDAData);
