using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Nautilus.Handlers;

namespace FrootLuips.CustomCraft3Remake.DTOs;
#nullable enable
internal sealed class CustomGroupData : IRegisterable<CustomGroup>
{
	private const string _BAD_ICON = "Unknown icon";

	public string Id { get; set; }
	public string? DisplayName { get; set; }
	public string IconID { get; set; }
	public string Fabricator { get; set; }
	public string[] FabricatorPath { get; set; }

	[Newtonsoft.Json.JsonConstructor]
	public CustomGroupData(
			string id,
			string? displayName = null,
			string iconID = "",
			string fabricator = nameof(CraftTree.Type.Fabricator),
			string[]? fabricatorPath = null)
	{
		this.Id = id;
		this.DisplayName = displayName;
		this.IconID = iconID;
		this.Fabricator = fabricator;
		this.FabricatorPath = fabricatorPath ?? new string[0];
	}

	public CustomGroupData(
		string id,
		string? displayName = null,
		TechType icon = TechType.None,
		CraftTree.Type fabricator = CraftTree.Type.Fabricator,
		string[]? fabricatorPath = null)
		: this(id, displayName, icon.ToString(), fabricator.ToString(), fabricatorPath)
	{ }

	public bool TryConvert([NotNull] in List<string> errors, [MaybeNullWhen(false)] out CustomGroup result)
	{
		if (Validation.TryParseTechType(IconID, out var icon).Fails())
		{
			errors.Add(Validation.InvalidIDMessage(IconID, notice: _BAD_ICON));
			icon = Validation.DEFAULT_TECH_TYPE;
		}

		if (Validation.ParseFabricator(Fabricator, out var fabricator).Fails())
		{
			errors.Add(Validation.InvalidFabricatorMessage(Fabricator));
		}

		if (string.IsNullOrEmpty(DisplayName))
		{
			DisplayName = Id;
		}

		FabricatorPath ??= Array.Empty<string>();

		result = new CustomGroup(this.Id, this.DisplayName!, icon, fabricator, FabricatorPath.Append(Id).ToArray());
		return true;
	}

	public string GetId()
	{
		return FabricatorPath.Length == 0
			? string.Join("/", Fabricator, Id)
			: string.Join("/", Fabricator, FabricatorPath, Id);
	}

	public void Register(List<string> errors, CustomGroup item)
	{
		UnityEngine.Sprite icon = item.Icon is TechType.None
			? SpriteManager.defaultSprite
			: SpriteManager.Get(item.Icon);
		Validation.Assert(icon is not null, "A null icon is not allowed.");

		Utilities.CreateMissingTabs(item.Fabricator, item.Path, errors);
		CraftTreeHandler.AddTabNode(item.Fabricator, item.Id, item.DisplayName, icon, item.Path);
	}
}

internal sealed record class CustomGroup(string Id, string DisplayName, TechType Icon, CraftTree.Type Fabricator, params string[] Path);
