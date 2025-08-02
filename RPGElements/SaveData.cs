using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrootLuips.RPGElements.Stats;
using Nautilus.Json;

namespace FrootLuips.RPGElements;
internal class SaveData : SaveDataCache
{
	[NonSerialized]
	public static SaveData? Main;

	public int Id;
	public string ModVersion = string.Empty;

	public PlayerStats PlayerStats = new();

	public void Init()
	{
		OnFinishedLoading += OnSaveDataLoaded;
	}

	private void OnSaveDataLoaded(object sender, JsonFileEventArgs args)
	{

	}
}
