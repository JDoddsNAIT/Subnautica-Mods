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
		effect = effect.ToLower();
		
		if (Enum.TryParse(effect, ignoreCase: true, out ChaosEffect effectId))
		{
			string? msg = null;
			void callback(string message) => msg = message;
			EffectManager.AddEffect(callback, effectId);
			return msg ?? throw new CommandFailedException(nameof(Command.Trigger) + " " + effect);
		}
		else
		{
			return EffectNotFound(effect);
		}
	}

	private static string Clear(string effect)
	{
		string? msg = null;
		void callback(string message) => msg = message;
		switch (effect)
		{
			case null or "":
				EffectManager.RemoveEffect(callback);
				return msg ?? throw new CommandFailedException(nameof(Command.Clear));
			default:
				if (Enum.TryParse(effect, ignoreCase: true, out ChaosEffect chaosEffect))
				{
					EffectManager.RemoveEffect(callback, chaosEffect);
					return msg ?? throw new CommandFailedException(nameof(Command.Clear) + " " +  chaosEffect);
				}
				else
				{
					return EffectNotFound(effect);
				}
		}
	}

	private static string Help(string help)
	{
		string defaultMessage = "Commands: " + string.Join(", ", Enum.GetNames(typeof(Command))) + "\n" +
			$"Use \"{COMMAND_NAME} help {{CommandName}}\" to show more details about that command, or \"{COMMAND_NAME} help effects\" to list all effect IDs.";

		return help switch {
			_ when Enum.TryParse(help, ignoreCase: true, out Command command) => GetCommandInfo(defaultMessage, command),
			"effects" => "Effect IDs: " + string.Join(", ", Enum.GetNames(typeof(ChaosEffect))),
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
			Command.Trigger => " {EffectID}: Triggers the specified effect.",
			Command.Clear => " <EffectID>: Stops the an effect. Stops all if none specified.",
			Command.Help => " <Command>: Shows details about a command, or lists all commands if none specified.",
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
