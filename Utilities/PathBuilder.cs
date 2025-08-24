using System;
using System.Collections.Generic;
using System.IO;
using FrootLuips.Subnautica.Extensions;
using FrootLuips.Subnautica.Validation;

namespace FrootLuips.Subnautica;

/// <summary>
/// Helper class for constructing file paths.
/// </summary>
public class PathBuilder : ArrayBuilder<string>, IArrayBuilder<PathBuilder, string>
{
	private static readonly PathValidator _validator = new();

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

		string? path = null;
		parts.Validate(_validator).ThrowIfFailed();
		return path!;
	}

	/// <summary>
	/// <inheritdoc cref="Combine"/>
	/// </summary>
	/// <param name="path"></param>
	/// <returns></returns>
	public bool TryCombine(out string? path)
	{
		try
		{
			path = this.Combine();
			return true;
		}
		catch (AggregateException)
		{
			path = null;
			return false;
		}
	}
}

internal class PathValidator : IValidator<string[]>
{
	private const string
		_INVALID_CHAR_MESSAGE = "Contains an invalid {0} character.",
		_PARAM_NAME = "Path part #{0} ({1})";

	public string? FilePath { get; private set; } = null;

	public bool GetSuccess(string[]? obj, IReadOnlyCollection<Exception> issues)
	{
		return !string.IsNullOrWhiteSpace(FilePath);
	}

	public IEnumerator<Exception> Validate(string[]? parts)
	{
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
