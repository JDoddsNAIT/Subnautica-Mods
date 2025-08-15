using System;
using System.Collections.Generic;

namespace FrootLuips.Subnautica.Validation;
/// <summary>
/// Represents the results from validating a <typeparamref name="T"/> object.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="Object">The object that was validated.</param>
/// <param name="Success">Whether validation was successful.</param>
/// <param name="Issues">Any issues that may have occurred.</param>
public readonly record struct ValidationResult<T>(T Object, bool Success, IReadOnlyList<Exception> Issues)
{
	/// <summary>
	/// Evaluates a <paramref name="result"/> for <see cref="Success"/>.
	/// </summary>
	/// <param name="result"></param>
	public static implicit operator bool(ValidationResult<T> result) => result.Success;

	/// <summary>
	/// Throws an <see cref="AggregateException"/> if validation was not successful.
	/// </summary>
	public void ThrowIfFailed()
	{
		if (!Success)
			throw Issues.ToAggregate();
	}
}
