using System.Reflection;
using FrootLuips.CustomCraft3Remake.DTOs;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;

namespace FrootLuips.CustomCraft3Remake;

internal static partial class Item
{
	public static void Register(CustomItem data, Assembly assembly)
	{
		if (data is null) throw new System.ArgumentNullException(nameof(data));

		Validation.Assert(data.Model is not TechType.None, "A null model is not allowed.");

		var info = PrefabInfo.WithTechType(
			data.ItemId,
			data.DisplayName,
			data.Description,
			unlockAtStart: data.PDAData is null || data.PDAData.UnlockAtStart,
			techTypeOwner: assembly);

		UnityEngine.Sprite icon = data.Icon is TechType.None
			? SpriteManager.defaultSprite : SpriteManager.Get(data.Icon);
		Validation.Assert(icon is not null, "A null icon is not allowed");

		info.WithIcon(icon);

		var prefab = new CustomPrefab(info);
		prefab.SetGameObject(new CloneTemplate(info, data.Model));

		if (data.PDAData is not null && data.PDAData.Group.HasValue && data.PDAData.Category.HasValue)
		{
			prefab.SetPdaGroupCategory(data.PDAData.Group.Value, data.PDAData.Category.Value);
		}

		prefab.Register();
	}
}
