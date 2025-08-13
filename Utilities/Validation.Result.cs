using System;
using System.Collections.Generic;

namespace FrootLuips.Subnautica;
public static partial class Validation
{
	/// <summary>
	/// The represents the result of a validation process.
	/// </summary>
	/// <param name="Success">Whether validation succeeded.</param>
	/// <param name="Errors">List of issues.</param>
	public readonly record struct Result(bool Success, List<Exception> Errors)
	{
		/// <param name="obj"></param>
		public static implicit operator bool(Result obj) => obj.Success;

		/// <summary>
		/// Throws an <see cref="AggregateException"/> of all <see cref="Errors"/> if <see cref="Success"/> is <see langword="false"/>.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="AggregateException"/>
		public bool ThrowIfFailed() => Success ? true : throw Errors.ToAggregate();
	}
}
