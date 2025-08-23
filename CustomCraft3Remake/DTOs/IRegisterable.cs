using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FrootLuips.CustomCraft3Remake.DTOs;

internal interface IJsonData<TResult> where TResult : class
{
	/// <summary>
	/// Attempts to convert this object into a <typeparamref name="TResult"/> value.
	/// </summary>
	/// <param name="errors">List of error messages detailing any issues that may have occurred during conversion. Expect this value to not be null.</param>
	/// <param name="result">The resulting <typeparamref name="TResult"/> value. Expect this value to be <see langword="default"/> when this method returns <see langword="false"/>.</param>
	/// <returns><see langword="true"/> if conversion was successful and <paramref name="result"/> has been initialized to a nontrivial value, otherwise <see langword="false"/>.</returns>
	bool TryConvert([NotNull] in List<string> errors, [MaybeNullWhen(false)] out TResult result);
}

internal interface IRegisterable<TResult> : IJsonData<TResult>, IEqualityComparer<TResult>
	where TResult : class
{
	string GetId();
	void Register(List<string> errors, TResult item);
}