using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FrootLuips.Subnautica.Validation;
/// <summary>
/// Represents the results from validating a <typeparamref name="T"/> object.
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly record struct ValidationResult<T> : IEnumerable<Exception>
{
	/// <summary>
	/// Creates a <see cref="ValidationResult{T}"/> object.
	/// </summary>
	/// <param name="success"></param>
	/// <param name="issues"></param>
	/// <param name="result"></param>
	internal ValidationResult(bool success, IReadOnlyCollection<Exception> issues, T? result)
	{
		this.Success = success;
		this.Issues = issues;
		this.Result = result;
	}

	/// <summary>
	/// Whether validation was successful.
	/// </summary>
	[MemberNotNullWhen(true, nameof(Result))]
	public readonly bool Success { get; }
	/// <summary>
	/// Any issues that may have occurred.
	/// </summary>
	public readonly IReadOnlyCollection<Exception> Issues { get; }
	/// <summary>
	/// The resulting object.
	/// </summary>
	public readonly T? Result { get; }

	/// <summary>
	/// Evaluates a <paramref name="result"/> for <see cref="Success"/>.
	/// </summary>
	/// <param name="result"></param>
	public static implicit operator bool(ValidationResult<T> result) => result.Success;

	/// <summary>
	/// <inheritdoc cref="Result"/>
	/// </summary>
	/// <param name="result"></param>
	public static explicit operator T?(ValidationResult<T> result) => result.Result;

	/// <summary>
	/// Throws an <see cref="AggregateException"/> if validation was not successful.
	/// </summary>
	[MemberNotNull(nameof(Result))]
	public void ThrowIfFailed()
	{
		if (!Success)
			throw Issues.ToAggregate();
	}

	/// <inheritdoc/>
	public IEnumerator<Exception> GetEnumerator() => this.Issues.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)this.Issues).GetEnumerator();
}
