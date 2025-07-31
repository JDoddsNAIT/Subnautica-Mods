using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using UnityEngine;

namespace HandheldDrill.Items.Equipment;
internal static class HandheldDrillPrefab
{
	public static PrefabInfo Info { get; } = PrefabInfo.WithTechType("HandheldDrillTool", unlockAtStart: true)
		.WithSizeInInventory(new Vector2int(x: 2, y: 2))
		.WithIcon(SpriteManager.Get(TechType.ExosuitDrillArmModule));

	public static void Register()
	{
		var prefab = new CustomPrefab(Info);

		var prefabObj = new CloneTemplate(Info, TechType.ExosuitDrillArmModule);
		prefabObj.ModifyPrefab += (GameObject obj) => {
			var drillArm = obj.GetComponent<ExosuitDrillArm>();
			HandheldDrill handheldDrill = obj.AddComponent<HandheldDrill>();
			handheldDrill.DrillArm = drillArm;
		};

		prefab.SetGameObject(prefabObj);
		prefab.SetEquipment(EquipmentType.Hand);
		prefab.Register();
	}
}

public class HandheldDrill : PlayerTool
{
	public ExosuitDrillArm DrillArm { get; set; }

	public override void OnToolUseAnim(GUIHand guiHand)
	{
		(DrillArm as IExosuitArm).OnUseDown(out _);
	}
}
