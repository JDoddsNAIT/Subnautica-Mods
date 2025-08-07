using Nautilus.Json.ExtensionMethods;

namespace FrootLuips.ChaosMod.Effects;
internal class RandomTeleport : BaseChaosEffect
{
	public RandomTeleport() : base(ChaosEffect.RandomTeleport) { }

	public const string TELEPORTS = "teleports.json";

	public static readonly string teleportsPath = GetPluginPath(TELEPORTS);

	private RandomDistribution<Objects.TeleportPosition>? Distribution { get; set; } = null;

	public override void OnStart()
	{
		if (Distribution == null)
			LoadDistribution();

		GotoConsoleCommand.main.GotoPosition(Distribution!.GetRandomItem().position, gotoImmediate: true);
	}

	public override void FromData(Effect data, StatusCallback callback)
	{
		this.Duration = data.Duration;
		this.Weight = data.Weight;
		this.Distribution = null;

		List<string> errors = new();
		try
		{
			this.LoadDistribution();
		}
		catch (Exception ex)
		{
			errors.Add(ex.Message);
		}
		finally
		{
			bool success = this.Distribution != null;
			callback(errors, success);
		}
	}

	private void LoadDistribution()
	{
		var positions = new List<Objects.TeleportPosition>();
		positions.LoadJson(teleportsPath, createIfNotExist: false);
		this.Distribution = new(positions);
	}
}
