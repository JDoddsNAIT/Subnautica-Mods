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

	protected BaseChaosEffect(ChaosEffect id, float duation = 0, int weight = 100)
	{
		Id = id;
		Description = "";
		Duration = duation;
		Weight = weight;
	}

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
