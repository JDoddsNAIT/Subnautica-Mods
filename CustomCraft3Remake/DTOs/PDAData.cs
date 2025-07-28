using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FrootLuips.CustomCraft3Remake.DTOs;
#nullable enable
internal sealed class PDAData : IJsonData<ValidPDAData>
{
	public bool UnlockAtStart { get; set; }
	public string Group { get; set; }
	public string Category { get; set; }

	[Newtonsoft.Json.JsonConstructor]
	public PDAData(string group, string category, bool unlockAtStart = true)
	{
		this.UnlockAtStart = unlockAtStart;
		this.Group = group;
		this.Category = category;
	}

	public PDAData(TechGroup group, TechCategory category, bool unlockAtStart = true)
		: this(group.ToString(), category.ToString(), unlockAtStart)
	{ }

	public bool TryConvert([NotNull] in List<string> errors, [MaybeNullWhen(false)] out ValidPDAData result)
	{
		result = null;

		TechGroup? group;
		if (Group is null)
		{
			group = null;
		}
		else if (Enum.TryParse(Group, out TechGroup techGroup))
		{
			group = techGroup;
		}
		else
		{
			errors.Add(Validation.InvalidIDMessage(Group));
			return false;
		}

		TechCategory? category;
		if (Category is null)
		{
			category = null;
		}
		else if (Enum.TryParse(Category, out TechCategory techCategory))
		{
			category = techCategory;
		}
		else
		{
			errors.Add(Validation.InvalidIDMessage(Category));
			return false;
		}

		result = new ValidPDAData(group, category, UnlockAtStart);
		return true;
	}
}

internal sealed record class ValidPDAData(TechGroup? Group, TechCategory? Category, bool UnlockAtStart);
