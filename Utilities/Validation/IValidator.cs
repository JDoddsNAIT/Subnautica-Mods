using System;
using System.Collections.Generic;

namespace FrootLuips.Subnautica.Validation;
/// <summary>
/// Defines how <typeparamref name="T"/> values are validated.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IValidator<in T>
{
	/// <summary>
	/// Validated a <typeparamref name="T"/> object.
	/// </summary>
	/// <param name="obj"></param>
	/// <returns></returns>
	public IEnumerator<Exception> Validate(T? obj);
	/// <summary>
	/// Final check for successful validation.
	/// </summary>
	/// <param name="obj"></param>
	/// <param name="issues"></param>
	/// <returns></returns>
	public bool GetSuccess(T? obj, IReadOnlyCollection<Exception> issues);

	/// <summary>
	/// Success delegate for validation.
	/// </summary>
	/// <param name="obj">The object being validated.</param>
	/// <param name="issues"></param>
	/// <returns></returns>
	public delegate bool SuccessCallback(T? obj, IReadOnlyCollection<Exception> issues);
}

/// <summary>
/// <inheritdoc cref="IValidator{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Validator<T> : IValidator<T>
{
	/// <inheritdoc/>
	public abstract IEnumerator<Exception> Validate(T? obj);

	/// <inheritdoc/>
	public virtual bool GetSuccess(T? obj, IReadOnlyCollection<Exception> issues)
	{
		return obj != null && (issues == null || issues.Count == 0);
	}
}
