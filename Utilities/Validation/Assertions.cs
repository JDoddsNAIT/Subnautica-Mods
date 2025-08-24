namespace FrootLuips.Subnautica.Validation;
/// <summary>
/// Helper class with methods for asserting conditions.
/// </summary>
public static class Assertions
{
	/// <summary>
	/// Asserts that <paramref name="condition"/> is <see langword="true"/>, throwing an <see cref="AssertionFailedException"/> if not.
	/// </summary>
	/// <param name="condition"></param>
	/// <param name="message"></param>
	/// <returns>The value of <paramref name="condition"/>.</returns>
	/// <exception cref="AssertionFailedException"></exception>
	public static bool Assert(this bool condition, string? message = null)
	{
		if (!condition)
			throw new AssertionFailedException(message);
		return condition;
	}

	/// <summary>
	/// Asserts that the given <paramref name="value"/> is not <see langword="null"/>.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="message"></param>
	/// <returns></returns>
	/// <exception cref="AssertionFailedException"></exception>
	public static bool AssertNotNull(object? value, string? message = null)
	{
		if (value == null)
			throw new AssertionFailedException(message);
		return true;
	}
}
