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

	private readonly Dictionary<string, CustomGroup> _managedGroups = new();
	private readonly Dictionary<string, CustomItem> _managedItems = new();
	private readonly Dictionary<string, CustomRecipe> _managedRecipes = new();
	private readonly Dictionary<string, CustomSize> _managedSizes = new();

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

		const string
			modifiedMessage = "Data has been modified. Game must be restarted to apply changes.",
			missingMessage = "Data for '{0}' has been removed. Game must be restarted to apply changes.";

		List<string> itemsRegistered = new(items.Count);
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
					itemsRegistered.Add(item.GetId());
					if (IsUnchanged(item, result))
						item.Register(errors, result);
					else
						errors.Add(modifiedMessage);
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

		if (typeof(T) == typeof(CustomGroup))
		{
			foreach (var key in _managedGroups.Keys)
			{
				if (!itemsRegistered.Contains(key))
				{
					Logger.LogWarning(string.Format(missingMessage, key));
				}
			}
		}
		else if (typeof(T) == typeof(CustomItem))
		{
			foreach (var key in _managedItems.Keys)
			{
				if (!itemsRegistered.Contains(key))
				{
					Logger.LogWarning(string.Format(missingMessage, key));
				}
			}
		}
		else if (typeof(T) == typeof(CustomRecipe))
		{
			foreach (var key in _managedRecipes.Keys)
			{
				if (!itemsRegistered.Contains(key))
				{
					Logger.LogWarning(string.Format(missingMessage, key));
				}
			}
		}
		else if (typeof(T) == typeof(CustomSize))
		{
			foreach (var key in _managedSizes.Keys)
			{
				if (!itemsRegistered.Contains(key))
				{
					Logger.LogWarning(string.Format(missingMessage, key));
				}
			}
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

	private bool IsUnchanged<TData, T>(TData data, T value)
		where TData : IRegisterable<T> where T : class
	{
		string id = data.GetId();
		return value switch {
			CustomGroup => !_managedGroups.ContainsKey(id) || data.Equals(_managedGroups[id] as T, value),
			CustomItem => !_managedItems.ContainsKey(id) || data.Equals(_managedItems[id] as T, value),
			CustomRecipe => !_managedRecipes.ContainsKey(id) || data.Equals(_managedRecipes[id] as T, value),
			CustomSize => !_managedSizes.ContainsKey(id) || data.Equals(_managedSizes[id] as T, value),
			_ => throw new InvalidOperationException(),
		};
	}
}
