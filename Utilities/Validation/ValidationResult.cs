using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FrootLuips.Subnautica.Validation;
/// <summary>
/// Represents the results from validating an object.
/// </summary>
/// <typeparam name="TResult"></typeparam>
public sealed class ValidationResult<TResult> : IEnumerable<Exception>
{
	/// <summary>
	/// Creates a <see cref="ValidationResult{T}"/> object.
	/// </summary>
	/// <param name="passed"></param>
	/// <param name="issues"></param>
	/// <param name="result"></param>
	internal ValidationResult(bool passed, IReadOnlyList<Exception> issues, TResult? result)
	{
		this.Passed = passed;
		this.Issues = issues;
		this.Result = result;
	}

	/// <summary>
	/// Whether validation was successful.
	/// </summary>
	public bool Passed { get; }
	/// <summary>
	/// Any issues that may have occurred.
	/// </summary>
	public IReadOnlyList<Exception> Issues { get; }
	/// <summary>
	/// The resulting object.
	/// </summary>
	public TResult? Result { get; }

	/// <summary>
	/// Evaluates a <paramref name="result"/> for <see cref="Passed"/>.
	/// </summary>
	/// <param name="result"></param>
	public static implicit operator bool(ValidationResult<TResult> result) => result.Passed;

	/// <summary>
	/// <inheritdoc cref="Result"/>
	/// </summary>
	/// <param name="result"></param>
	public static explicit operator TResult?(ValidationResult<TResult> result) => result.Result;

	/// <summary>
	/// Throws an <see cref="AggregateException"/> if validation was not successful.
	/// </summary>
	public void ThrowIfFailed()
	{
		if (!Passed)
			throw Issues.ToAggregate();
	}

	/// <inheritdoc/>
	public IEnumerator<Exception> GetEnumerator() => this.Issues.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)this.Issues).GetEnumerator();
}
