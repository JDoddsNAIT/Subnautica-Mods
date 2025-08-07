using FrootLuips.ChaosMod.Effects;
using FrootLuips.ChaosMod.Logging;
using FrootLuips.ChaosMod.Utilities;
using Nautilus.Commands;
using Nautilus.Json.ExtensionMethods;

namespace FrootLuips.ChaosMod;

internal static class ConsoleCommands
{
	public const string COMMAND_NAME = "chaosmod";

	public enum Command
	{
		Start, Stop, List, Trigger, Clear, Help
	}

	[ConsoleCommand("saveteleports")]
	public static string GetTeleports()
	{
		GotoConsoleCommand.main.data.locations.SimpleSelect(Utilities.Utils.ToPosition).SaveJson(RandomTeleport.teleportsPath);
		return "Saved to " + RandomTeleport.teleportsPath;
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
			Command.List => List(),
			Command.Trigger => Trigger(arg2),
			Command.Clear => Clear(arg2),
			Command.Help => Help(arg2),
			_ => CommandNotFound(arg1),
		};
	}

	public static string Start()
	{
		string? msg = null;
		void callback(string message) => msg = message;
		EffectManager.StartRoutine(callback);
		return msg ?? throw new CommandFailedException(nameof(Command.Start));
	}

	public static string Stop()
	{
		string? msg = null;
		void callback(string message) => msg = message;
		EffectManager.StopRoutine(callback);
		return msg ?? throw new CommandFailedException(nameof(Command.Stop));
	}

	public static string List()
	{
		string? msg = null;
		void callback(string message) => msg = message;
		EffectManager.GetActiveEffects(callback);
		return msg ?? throw new CommandFailedException(nameof(Command.List));
	}

	public static string Trigger(string effect)
	{
		var ex = CommandFailed(Command.Trigger, effect);
		return effect.ToLower() switch {
			null or "" or "random" => AddEffect() ?? throw ex,
			_ when ChaosEffects.TryGetEffect(effect, out var effectId) => AddEffect(effectId) ?? throw ex,
			_ => EffectNotFound(effect)
		};
	}

	private static string? AddEffect(params ChaosEffect[] effects)
	{
		string? msg = null;
		void callback(string message) => msg = message;
		EffectManager.AddEffect(callback, effects);
		return msg;
	}

	private static string Clear(string effect)
	{
		var ex = CommandFailed(Command.Clear, effect);
		return effect.ToLower() switch {
			null or "" or "all" => RemoveEffect() ?? throw ex,
			_ when ChaosEffects.TryGetEffect(effect, out var effectId) => RemoveEffect(effectId) ?? throw ex,
			_ => EffectNotFound(effect)
		};
	}

	private static string? RemoveEffect(params ChaosEffect[] effects)
	{
		string? msg = null;
		void callback(string message) => msg = message;
		EffectManager.RemoveEffect(callback, effects);
		return msg;
	}

	private static string Help(string help)
	{
		string defaultMessage = "Commands: " + string.Join(", ", Enum.GetNames(typeof(Command))) + "\n" +
			$"Use \"{COMMAND_NAME} help {{CommandName}}\" to show more details about that command, or \"{COMMAND_NAME} help effects\" to list all effect IDs.";

		return help switch {
			_ when Enum.TryParse(help, ignoreCase: true, out Command command) => GetCommandInfo(defaultMessage, command),
			"effects" => "Effect IDs: " + string.Join(", ", ChaosEffects.Effects.Keys),
			_ => defaultMessage,
		};
	}

	private static string GetCommandInfo(string defaultMessage, Command command)
	{
		string message = $"{COMMAND_NAME} {command}";
		return message + command switch {
			Command.Start => ": Enables the mod.",
			Command.Stop => ": Disables the mod.",
			Command.List => ": Lists all active chaos effects.",
			Command.Trigger => " <EffectID>: Triggers an effect. Triggers a random one if none specified.",
			Command.Clear => " <EffectID>: Stops an effect. Stops all if none specified.",
			Command.Help => " <Command>: Shows details about a command, or lists all commands if none specified.",
			_ => defaultMessage,
		};
	}

	private static Exception CommandFailed(Command command, string? arg2 = null)
	{
		string commandString = string.IsNullOrWhiteSpace(arg2)
			? command + ""
			: command + " " + arg2;
		return new CommandFailedException(commandString);
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
