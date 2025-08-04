using System;
using FrootLuips.ChaosMod.Effects;
using FrootLuips.ChaosMod.Logging;
using FrootLuips.ChaosMod.Utilities;
using Nautilus.Commands;

namespace FrootLuips.ChaosMod;

internal static class ConsoleCommands
{
	public const string COMMAND_NAME = "chaosmod";

	public enum Command
	{
		Start, Stop, List, Trigger, Clear, Help
	}

	[ConsoleCommand(COMMAND_NAME)]
	public static string ChaosCommand(string arg1, string arg2 = "")
	{
		if (!Enum.TryParse(arg1, ignoreCase: true, out Command command))
		{
			return CommandNotFound(arg1);
		}

		return command switch {
			Command.Start => Start(),
			Command.Stop => Stop(),
			Command.List => string.Join(", ", ChaosMod.GetActiveEffectIds()),
			Command.Trigger => Trigger(arg2),
			Command.Clear => Clear(arg2),
			Command.Help => Help(arg2),
			_ => CommandNotFound(arg1),
		};
	}

	public static string Start()
	{
		ChaosMod.Start(showInGame: false);
		return ChaosMod.START_MESSAGE;
	}

	public static string Stop()
	{
		ChaosMod.Stop(showInGame: false);
		return ChaosMod.STOP_MESSAGE;
	}

	public static string Trigger(string effect)
	{
		effect = effect.ToLower();
		if (Enum.TryParse(effect, ignoreCase: true, out ChaosEffect effectId))
		{
			try
			{
				ChaosMod.TriggerEffect(ChaosEffects.Effects[effectId]);
				return $"Triggered effect {effectId}";
			}
			catch (AssertionFailedException ex)
			{
				return ex.Message;
			}
		}
		else
		{
			return EffectNotFound(effect);
		}
	}

	private static string Clear(string effect)
	{
		if (string.IsNullOrEmpty(effect))
		{
			ChaosMod.ClearEffects();
			return "Cleared all chaos effects";
		}
		else if (Enum.TryParse(effect, ignoreCase: true, out ChaosEffect clearEffect))
		{
			ChaosMod.ClearEffects(clearEffect);
			return $"Removed effect \"{clearEffect}\"";
		}
		else
		{
			return EffectNotFound(effect);
		}
	}

	private static string Help(string help)
	{
		string defaultMessage = "Commands: " + string.Join(", ", Enum.GetNames(typeof(Command))) + "\n" +
			$"Use \"{COMMAND_NAME} help {{CommandName}}\" to show more details about that command, or \"{COMMAND_NAME} help effects\" to list all effect IDs.";

		return help switch {
			_ when Enum.TryParse(help, ignoreCase: true, out Command command) => command switch {
				Command.Start => "Enables the mod.",
				Command.Stop => "Disables the mod.",
				Command.List => "Lists all active chaos effects.",
				Command.Trigger => "Triggers the specified effect.",
				Command.Clear => "Stops the specific effect. Stops all if none specified.",
				Command.Help => "Shows details on all commands.",
				_ => defaultMessage,
			},
			"effects" => "Effect IDs: " + string.Join(", ", Enum.GetNames(typeof(ChaosEffect))),
			_ => defaultMessage,
		};
	}

	private static LogMessage CommandNotFound(string command)
	{
		return new LogMessage()
			.WithNotice("Command not found: \"", command, "\"")
			.WithMessage("Expected ", string.Join("/", Enum.GetNames(typeof(Command))));
	}

	private static LogMessage EffectNotFound(string effect)
	{
		return new LogMessage()
			.WithNotice("Effect not found: \"", effect, "\"")
			.WithMessage("Use \"", COMMAND_NAME, " help effects\" for a list of all effects.");
	}
}
