using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Nautilus.Handlers;

namespace FrootLuips.CustomCraft3Remake;

internal static class Validation
{
	public const TechType DEFAULT_TECH_TYPE = TechType.None;
	public const CraftTree.Type DEFAULT_FABRICATOR = CraftTree.Type.None;
	public const string ERROR_SEPARATOR = "; ";

	public static bool TryParseTechType(string text, out TechType techType)
	{
		techType = DEFAULT_TECH_TYPE;
		if (string.IsNullOrWhiteSpace(text)) return false;

		return Enum.TryParse(text, out techType) || EnumHandler.TryGetValue(text, out techType);
	}

	public static bool ParseFabricator(string text, out CraftTree.Type fabricator)
	{
		fabricator = DEFAULT_FABRICATOR;
		if (string.IsNullOrWhiteSpace(text))
		{
			return false;
		}
		else
		{
			return Enum.TryParse(text, out fabricator) || EnumHandler.TryGetValue(text, out fabricator);
		}
	}

	public static bool Fails(this bool value) => !value;

	public static string InvalidIDMessage(string techType, string notice = "")
	{
		var sb = new StringBuilder();
		if (!string.IsNullOrWhiteSpace(notice))
			sb.Append(notice).Append(": ");
		sb.Append("'").Append(techType).Append("' is not a valid id");
		return sb.ToString();
	}

	public static string InvalidFabricatorMessage(string fabricatorType, string notice = "")
	{
		var sb = new StringBuilder();
		if (!string.IsNullOrWhiteSpace(notice))
			sb.Append(notice).Append(": ");
		sb.Append("Fabricator type '").Append(fabricatorType).Append("' is empty or invalid, default value will be used");
		return sb.ToString();
	}

	public static string NoValidIngredientsMessage(string notice = "")
	{
		return $"{notice}: Recipe has no ingredients";
	}

	[Obsolete($"Logging this message is handled by the {nameof(CustomCraftService)} class.")]
	public static string NotAJsonFileMessage(string filePath, string notice = "")
	{
		var sb = new StringBuilder();
		if (!string.IsNullOrWhiteSpace(notice))
			sb.Append(notice).Append(" - ");
		sb.Append(Path.GetFileName(filePath))
			.Append(" is not a .json file. Ensure that all files in the ")
			.Append(Plugin.WORKING_DIR)
			.Append(" directory are .json files");
		return sb.ToString();
	}

	[Obsolete($"Logging this message is handled by the {nameof(CustomCraftService)} class.")]
	public static string CatchExceptionDuringLoadMessage(string filePath, Exception exception, string notice = "")
	{
		var sb = new StringBuilder();
		if (!string.IsNullOrWhiteSpace(notice))
			sb.Append(notice).Append(" - ");
		sb.Append("An error occurred while loading from '")
			.Append(filePath[filePath.IndexOf(Plugin.WORKING_DIR)..])
			.Append("'. See below for details.")
			.Append(exception);
		return sb.ToString();
	}

	public static bool Assert(bool condition, string message)
	{
		if (!condition)
			throw new AssertionFailedException(message);
		else
			return true;
	}

	public static bool AssertDirectoryExists(string directory)
	{
		return Assert(Directory.Exists(directory), $"Expected directory '{directory}' could not be found.");
	}
}

[Serializable]
public class AssertionFailedException : Exception
{
	private const string _ASSERTION_FAILED = "Assertion Failed: ";

	public AssertionFailedException() { }
	public AssertionFailedException(string message) : base(_ASSERTION_FAILED + message) { }
	public AssertionFailedException(string message, Exception inner) : base(_ASSERTION_FAILED + message, inner) { }
	protected AssertionFailedException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
