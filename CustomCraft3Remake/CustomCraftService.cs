using System;
using System.Collections.Generic;
using System.IO;
using BepInEx.Logging;
using FrootLuips.CustomCraft3Remake.DTOs;
using Nautilus.Json.ExtensionMethods;

namespace FrootLuips.CustomCraft3Remake;

/// <summary>
/// Used for loading custom craft data.
/// </summary>
public sealed class CustomCraftService
{
	private const string _FILE_EXTENSION = ".json";

	public ManualLogSource Logger { get; }

	public CustomCraftService(ManualLogSource logger) => Logger = logger;

	private bool ValidatePath(in string jsonFilePath)
	{
		const string emptyOrNullPath = "Path cannot be a null or blank string.",
			nonJsonOrNotInPlugins = "Path must point to a .json file in the plugins directory.";

		try
		{
			if (string.IsNullOrWhiteSpace(jsonFilePath))
				throw new ArgumentNullException(nameof(jsonFilePath), emptyOrNullPath);
			if (!jsonFilePath.StartsWith(BepInEx.Paths.PluginPath))
				throw new ArgumentException(nonJsonOrNotInPlugins, nameof(jsonFilePath));
			if (!jsonFilePath.EndsWith(_FILE_EXTENSION))
				throw new ArgumentException(nonJsonOrNotInPlugins, nameof(jsonFilePath));
			if (!File.Exists(jsonFilePath))
				throw new FileNotFoundException("No file exists at the specified path.", jsonFilePath);
			return true;
		}
		catch (Exception ex)
		{
			var msg = new LogMessage(ex)
				.WithContext(nameof(CustomCraftService))
				.WithNotice("Cannot load file");
			Logger.LogError(msg.ToString());
			return false;
		}
	}

	/// <summary>
	/// Loads all custom fabricator groups defined in a file. This should be be called before loading any recipes that use a custom groups.
	/// </summary>
	/// <param name="jsonFilePath"></param>
	/// <returns></returns>
	public bool LoadGroups(in string jsonFilePath) => Internal_LoadData<CustomGroupData, CustomGroup>(jsonFilePath);

	/// <summary>
	/// Loads all custom items defined in a file. This should be called before loading any recipes that reference a custom item.
	/// </summary>
	/// <param name="jsonFilePath"></param>
	/// <returns></returns>
	public bool LoadItems(in string jsonFilePath) => Internal_LoadData<CustomItemData, CustomItem>(jsonFilePath);

	/// <summary>
	/// Loads all custom size data defined in a file. This should be called before any of the defined items are instantiated into the world.
	/// </summary>
	/// <param name="jsonFilePath"></param>
	/// <returns></returns>
	public bool LoadSizes(in string jsonFilePath) => Internal_LoadData<CustomSizeData, CustomSize>(jsonFilePath);

	/// <summary>
	/// Loads all custom recipes defined in a file. This should only be called after all custom fabricator groups and custom items are loaded.
	/// </summary>
	/// <param name="jsonFilePath"></param>
	/// <returns></returns>
	public bool LoadRecipes(in string jsonFilePath) => Internal_LoadData<CustomRecipeData, CustomRecipe>(jsonFilePath);

	internal bool Internal_LoadData<TData, T>(in string jsonFilePath, List<string> errors = null, List<TData> items = null)
		where TData : IRegisterable<T> where T : class
	{
		if (!ValidatePath(jsonFilePath))
			return false;

		string itemType = typeof(T).Name;

		errors ??= new();
		items ??= new();

		Logger.LogDebug(new LogMessage(context: nameof(CustomCraftService)).WithMessage("Loading ", jsonFilePath[BepInEx.Paths.PluginPath.Length..]));

		if (!this.TryLoadData<TData, T>(jsonFilePath, items, itemType))
			return false;

		for (int i = 0; i < items.Count; i++)
		{
			var item = items[i];
			errors.Clear();

			bool validItem;
			try
			{
				validItem = item.TryConvert(errors, out T result);
				if (validItem)
				{
					item.Register(errors, result);
				}
			}
			catch (Exception ex)
			{
				validItem = false;
				errors.Clear();
				errors.Add(ex.ToString());
			}

			LogLevel level = validItem switch {
				false => LogLevel.Error,
				true when errors.Count > 0 => LogLevel.Warning,
				true => LogLevel.Info,
			};

			string notice = validItem
				? "Registered"
				: "Failed to register";
			notice += $" {itemType} '{item.GetId()}'";
			string message = errors.Count > 0
				? string.Join(Validation.ERROR_SEPARATOR, errors)
				: "No issues.";

			Logger.Log(level, new LogMessage(context: nameof(CustomCraftService), notice, message));
		}
		return true;
	}

	private bool TryLoadData<TData, T>(string jsonFilePath, List<TData> items, string itemType)
		where TData : IRegisterable<T> where T : class
	{
		items.Clear();
		try
		{
			items.LoadJson(jsonFilePath);
			return true;
		}
		catch (Exception ex)
		{
			Logger.LogError(new LogMessage().WithContext(nameof(CustomCraftService))
				.WithNotice("Failed to load ", itemType, "s")
				.WithMessage(ex));
			return false;
		}
	}
}
