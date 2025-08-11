using UnityEngine;

namespace FrootLuips.ChaosMod;
#nullable disable
internal class ConsoleCommandListener : MonoBehaviour
{
	public void Awake()
	{
		DevConsole.RegisterConsoleCommand(this, ConsoleCommands.COMMAND_NAME, false, false);
	}

	public void OnDestroy()
	{
		ConsoleCommands.Stop();
	}

	public void OnConsoleCommand_chaosmod(NotificationCenter.Notification n)
	{
		if (n.data == null)
		{
			Plugin.Logger.LogInGame(ConsoleCommands.ChaosCommand(""));
		}
		else
		{
			string arg1 = n.data[0] as string;
			string arg2 = n.data[1] as string;

			Plugin.Logger.LogInGame(ConsoleCommands.ChaosCommand(arg1 ?? "", arg2 ?? ""));
		}
	}
}
