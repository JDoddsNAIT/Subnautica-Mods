using Nautilus.Json.ExtensionMethods;

namespace FrootLuips.ChaosMod.Effects;
internal class RandomTeleport : BaseChaosEffect
{
	public const string TELEPORTS = "teleports.json";

	public static readonly string teleportsPath = GetPluginPath(TELEPORTS);

	public override ChaosEffect Id { get; } = ChaosEffect.RandomTeleport;
	public override string? Description { get; set; } = "";
	public override float Duration { get; set; } = 0;
	public override int Weight { get; set; } = 100;

	private RandomDistribution<Objects.TeleportPosition>? Distribution { get; set; } = null;

	/*
	 * Teleports:
	 * safe1: 0, 0, 0
	 * safe2: 22, -16, 240
	 * safe3: 56, -13, -72
	 * kelp1: 313, -50, -60
	 * kelp2: -325, -115, 270
	 * kelp3: -37, -10, 418
	 * grassy1: -688, -105, -51
	 * grassy2: 339, -100, 316
	 * jellyshroom: -715, -178, 7
	 * sparse: -684, -221, -686
	 * grand: -396, -360, -1344
	 * mushroom: -837, -115, 564
	 * auroraback: 575, 0, -612
	 * koosh: 1251, -215, 606
	 * underisles: -40, -174, 746
	 * bloodkelp: -467, -500, 1337
	 * 
	 * gargantuanfossil: -780, -746, -239
	 * 
	 * precursorbase_gun: 
	 * precursorbase_lost: -230, -789, 315
	 * precursorbase_lava: 169, -1247, 278
	 * precursorcache1: -1252, -397, 1094
	 * precursorcache2: 
	 * precursorcache3: 
	 * 
	 * degasi1: -761, 15, -1105
	 * degasi2: 100, -250, -350
	 * degasi3: -645, -500, -954
	 */

	public override void OnStart()
	{
		GotoConsoleCommand.main.GotoPosition(Distribution!.GetRandomItem().position);
	}

	public override void FromData(Effect data, StatusCallback callback)
	{
		this.Duration = data.Duration;
		this.Weight = data.Weight;
		this.Distribution = null;

		List<string> errors = new();
		try
		{
			var positions = new List<Objects.TeleportPosition>();
			positions.LoadJson(teleportsPath);
			this.Distribution = new(positions);
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

	public override Effect ToData()
	{
		return new Effect() {
			Id = this.Id.ToString(),
			Duration = this.Duration,
			Weight = this.Weight,
		};
	}
}
