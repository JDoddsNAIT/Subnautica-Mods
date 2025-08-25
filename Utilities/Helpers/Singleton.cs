namespace FrootLuips.Subnautica.Helpers;
/// <summary>
/// Provides access to a static instance of <typeparamref name="T"/> that is never <see langword="null"/>.
/// </summary>
/// <remarks>
/// <typeparamref name="T"/> must be a reference type with an empty constructor to use this class.
/// </remarks>
/// <typeparam name="T"></typeparam>
public class Singleton<T> where T : class, new()
{
	private static bool _initialized = false;

	/// <summary>
	/// Underlying value of <see cref="Main"/>.
	/// </summary>
	protected static T? _instance = null;

	/// <summary>
	/// The main <typeparamref name="T"/> instance.
	/// </summary>
	/// <remarks>
	/// Creates a new instance of <typeparamref name="T"/> whenever the value is considered <see langword="null"/>
	/// </remarks>
	public static T Main => _instance switch {
		null => CreateNew(),
		UnityEngine.Object obj when obj == null => CreateNew(),
		_ => _instance,
	};

	private static T CreateNew()
	{
		if (_initialized)
		{
			Plugin.Logger.LogWarning(new Logging.LogMessage()
				.WithContext(typeof(Singleton<T>))
				.WithMessage("Main instance has been destroyed. Fixing..."));
		}
		_instance = new T();

		if (_instance is ISingleton singleton)
		{
			singleton.OnSingletonInit();
		}

		_initialized = true;
		return _instance;
	}
}

/// <summary>
/// Interface used for defining methods used by <see cref="Singleton{T}"/>
/// </summary>
public interface ISingleton
{
	/// <summary>
	/// Invoked when a new instance is set as <see cref="Singleton{T}.Main"/>.
	/// </summary>
	public void OnSingletonInit();
}
