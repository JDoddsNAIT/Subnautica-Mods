namespace FrootLuips.Subnautica;
/// <summary>
/// Provides access to a static instance of <typeparamref name="T"/>.
/// </summary>
/// <remarks>
/// <typeparamref name="T"/> must be a reference type with an empty constructor to use this class.
/// </remarks>
/// <typeparam name="T"></typeparam>
public static class Singleton<T> where T : class, new()
{
	private static T? _instance = new();

	/// <summary>
	/// The main <typeparamref name="T"/> instance.
	/// </summary>
	/// <remarks>
	/// Creates a new instance of <typeparamref name="T"/> whenever the value is considered <see langword="null"/>
	/// </remarks>
	public static T Main {
		get {
			if (_instance == null)
			{
				Plugin.Logger.LogWarning(new Logging.LogMessage().WithContext(nameof(Singleton<T>)).WithMessage("Main instance has been destroyed. Fixing..."));
				_instance = new T();
			}
			return _instance;
		}
	}
}
