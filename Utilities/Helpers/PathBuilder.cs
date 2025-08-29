using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using FrootLuips.Subnautica.Extensions;
using FrootLuips.Subnautica.Validation;

namespace FrootLuips.Subnautica.Helpers;

/// <summary>
/// Helper class for constructing file paths.
/// </summary>
public class PathBuilder : ArrayBuilder<string>, IArrayBuilder<PathBuilder, string>
{
	private static readonly PathValidator _validator = new();

	/// <inheritdoc cref="ArrayBuilder{T}.ArrayBuilder(int, T[])"/>
	public PathBuilder(int capacity, params string[] initialValues) : base(capacity, initialValues)
	{
	}

	/// <inheritdoc cref="ArrayBuilder{T}.ArrayBuilder(int)"/>
	public PathBuilder(int capacity) : base(capacity)
	{
	}

	/// <inheritdoc cref="ArrayBuilder{T}.ArrayBuilder(T[])"/>
	public PathBuilder(params string[] initialValues) : base(initialValues)
	{
	}

	/// <inheritdoc cref="ArrayBuilder{T}.ArrayBuilder()"/>
	public PathBuilder() : base()
	{
	}

	/// <inheritdoc/>
	public new PathBuilder Append(params string[] values)
	{
		return (PathBuilder)base.Append(values);
	}

	/// <inheritdoc/>
	public override string ToString() => Combine();

	/// <summary>
	/// Constructs a file path.
	/// </summary>
	/// <returns></returns>
	/// <exception cref="AggregateException"></exception>
	public string Combine()
	{
		var parts = this.ToArray();

		var result = parts.Validate(_validator);
		result.ThrowIfFailed();
		return result.Result;
	}

	/// <summary>
	/// <inheritdoc cref="Combine"/>
	/// </summary>
	/// <param name="path"></param>
	/// <returns></returns>
	public bool TryCombine([NotNullWhen(true)] out string? path)
	{
		return Validator.Try(this.Combine, out path);
	}
}

internal class PathValidator : IValidator<string[], string>
{
	private const string
		_INVALID_CHAR_MESSAGE = "Contains an invalid {0} character.",
		_PARAM_NAME = "Path part #{0} ({1})";

	public string? FilePath { get; private set; } = null;

	public bool Callback(string[]? obj,
		IReadOnlyCollection<Exception> issues,
		[NotNullWhen(true)] out string? result)
	{
		result = FilePath;
		return !string.IsNullOrWhiteSpace(FilePath);
	}

	public IEnumerator<Exception> Validate(string[]? parts)
	{
		FilePath = null;

		if (parts == null)
			throw new ArgumentNullException(nameof(parts));
		if (parts.Length == 0)
			throw new ArgumentException(nameof(parts));

		for (int i = 0; i < parts.Length; i++)
		{
			if (parts[i].ContainsAny(Path.GetInvalidPathChars()))
				yield return new ArgumentException(
					message: string.Format(_INVALID_CHAR_MESSAGE, "path"),
					paramName: string.Format(_PARAM_NAME, i + 1, parts[i]));

			else if (parts[i].ContainsAny(Path.GetInvalidFileNameChars()))
				yield return new ArgumentException(
					message: string.Format(_INVALID_CHAR_MESSAGE, "file name"),
					paramName: string.Format(_PARAM_NAME, i + 1, parts[i]));
		}

		FilePath = Path.Combine(parts);
	}
}
