using Nautilus.Json.ExtensionMethods;
using UnityEngine;

namespace FrootLuips.ChaosMod.Effects;
internal class RandomTeleport : BaseChaosEffect
{
	public RandomTeleport() : base(ChaosEffect.RandomTeleport, attributesExpected: 0) { }

	public const string TELEPORTS = "teleports.json";

	public static readonly string teleportsPath = GetPluginPath(TELEPORTS);

	private RandomDistribution<Objects.TeleportPosition>? Distribution { get; set; } = null;

	public override void OnStart()
	{
		if (Distribution == null)
			LoadDistribution();

		Vector3 position = Distribution!.GetRandomItem().position;
		Vehicle vehicle = Player.main.currentMountedVehicle;

		if (vehicle != null)
		{
			vehicle.TeleportVehicle(position, vehicle.transform.rotation);
		}
		else
		{
			GotoConsoleCommand.main.GotoPosition(position, gotoImmediate: true);
		}
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

	// These methods are never called as base.FromData is never called.
	protected override void ParseAttribute(Effect.Attribute attribute) => throw new NotImplementedException();
	protected override bool GetSuccess() => throw new NotImplementedException();
}
