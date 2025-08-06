namespace FrootLuips.ChaosMod.Effects;

/// <summary>
/// Provides a virtual implementation of <see cref="IChaosEffect"/>
/// </summary>
internal abstract class BaseChaosEffect : IChaosEffect
{
	public abstract ChaosEffect Id { get; }
	public abstract string? Description { get; set; }
	public abstract float Duration { get; set; }
	public abstract int Weight { get; set; }

	public virtual void BeforeStart() { }
	public virtual void OnStart() { }
	public virtual void Update(float time) { }
	public virtual void OnStop() { }

	public virtual void FromData(Effect data, StatusCallback callback)
	{
		this.Duration = data.Duration;
		this.Weight = data.Weight;
	}

	public virtual Effect ToData()
	{
		return new Effect() {
			Id = this.Id.ToString(),
			Duration = this.Duration,
			Weight = this.Weight,
		};
	}
}
