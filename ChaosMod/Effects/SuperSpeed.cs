using FrootLuips.ChaosMod.Utilities;

namespace FrootLuips.ChaosMod.Effects;
internal class SuperSpeed : BaseChaosEffect
{
	public SuperSpeed() : base(ChaosEffect.SuperSpeed, duation: 60f) { }

	public float? Multiplier { get; set; } = null;

	public override void OnStart()
	{
		throw new NotImplementedException();
		EnsurePlayerExists();
		MultiplySpeeds(Player.main.playerController, (float)Multiplier!);
	}

	public override void OnStop()
	{
		throw new NotImplementedException();
		EnsurePlayerExists();
		MultiplySpeeds(Player.main.playerController, 1f / (float)Multiplier!);
	}

	public override void FromData(Effect data, StatusCallback callback)
	{
		base.FromData(data, callback);

		Multiplier = null;

		List<string> errors = new();
		try
		{
			Validate(ValidateAttributes(data.Attributes));
		}
		catch (AggregateException agg)
		{
			foreach (var ex in agg)
			{
				errors.Add(ex.Message);
			}
		}
		catch (Exception ex)
		{
			errors.Add(ex.Message);
		}
		finally
		{
			bool success = Multiplier != null;
			callback(errors, success);
		}
	}

	private IEnumerator<Exception> ValidateAttributes(Effect.Attribute[] attributes)
	{
		ExpectAttributeCount(attributes, count: 1);
		var attribute = attributes[0];
		Exception? exception = null;

		try
		{
			switch (attribute.Name)
			{
				case nameof(Multiplier):
					attribute.ParseAttribute(float.Parse, out var mult);
					Multiplier = mult;
					break;
				default:
					throw attribute.Invalid();
			}
		}
		catch (Exception ex)
		{
			exception = ex;
		}

		if (exception != null)
			yield return exception;
	}

	public override Effect ToData()
	{
		return new() {
			Id = this.Id.ToString(),
			Duration = this.Duration,
			Weight = this.Weight,
			Attributes = new[] {
				new Effect.Attribute(nameof(Multiplier), Multiplier!.ToString()),
			}
		};
	}

	public static void MultiplySpeeds(PlayerController controller, float multiplier)
	{
		// Swim speeds
		controller.swimForwardMaxSpeed *= multiplier;
		controller.swimBackwardMaxSpeed *= multiplier;
		controller.swimStrafeMaxSpeed *= multiplier;
		controller.swimVerticalMaxSpeed *= multiplier;

		controller.swimWaterAcceleration *= multiplier;
		controller.defaultSwimDrag *= multiplier;

		// Seaglide speeds
		controller.seaglideForwardMaxSpeed *= multiplier;
		controller.seaglideBackwardMaxSpeed *= multiplier;
		controller.seaglideStrafeMaxSpeed *= multiplier;
		controller.seaglideVerticalMaxSpeed *= multiplier;

		controller.seaglideWaterAcceleration *= multiplier;
		controller.seaglideSwimDrag *= multiplier;

		// Walk/Run speeds
		controller.walkRunForwardMaxSpeed *= multiplier;
		controller.walkRunBackwardMaxSpeed *= multiplier;
		controller.walkRunStrafeMaxSpeed *= multiplier;

		// Update speeds
		Player.main.UpdateMotorMode();
	}
}


