namespace FrootLuips.ChaosMod.Effects;

/// <summary>
/// Provides a virtual implementation of <see cref="IChaosEffect"/>
/// </summary>
internal abstract class BaseChaosEffect : IChaosEffect
{
	public ChaosEffect Id { get; }
	public string? Description { get; set; }
	public float Duration { get; set; }
	public int Weight { get; set; }

	protected int _expectedAttributeCount;

	protected BaseChaosEffect(ChaosEffect id, int attributesExpected, float duration = 0, int weight = 100)
	{
		Id = id;
		_expectedAttributeCount = attributesExpected;
		Description = "";
		Duration = duration;
		Weight = weight;
	}

	public virtual string BeforeStart() => this.Description!;
	public virtual void OnStart() { }
	public virtual void Update(float time) { }
	public virtual void OnStop() { }

	public virtual void FromData(Effect data, StatusCallback callback)
	{
		this.Duration = data.Duration;
		this.Weight = data.Weight;

		List<string> errors = new();
		try
		{
			Validate(ValidateAttributes(data.Attributes));
		}
		catch (AggregateException agg)
		{
			foreach (var ex in agg.InnerExceptions)
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
			bool success = Description != null && this.GetSuccess();
			callback(errors, success);
		}
	}

	protected IEnumerator<Exception> ValidateAttributes(Effect.Attribute[] attributes)
	{
		ExpectAttributeCount(attributes, _expectedAttributeCount);

		for (int i = 0; i < attributes.Length; i++)
		{
			var attribute = attributes[i];
			Exception? exception = null;
			try
			{
				this.ParseAttribute(attribute);
			}
			catch (Exception ex)
			{
				exception = ex;
			}

			if (exception != null)
				yield return exception;
		}
	}

	protected virtual void ParseAttribute(Effect.Attribute attribute) => throw attribute.Invalid();
	protected virtual bool GetSuccess() => _expectedAttributeCount == 0;

	public virtual Effect ToData()
	{
		return new Effect() {
			Id = this.Id.ToString(),
			Duration = this.Duration,
			Weight = this.Weight,
		};
	}
}
