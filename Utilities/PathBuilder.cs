using System;
using System.Collections.Generic;
using System.IO;
using FrootLuips.Subnautica.Extensions;

namespace FrootLuips.Subnautica;

/// <summary>
/// Helper class for constructing file paths.
/// </summary>
public class PathBuilder : ArrayBuilder<string>, IArrayBuilder<PathBuilder, string>
{
	private const string _INVALID_CHAR_MESSAGE = "Contains an invalid {0} character.",
		_PARAM_NAME = "Path part #{0} ({1})";

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
	public string Combine()
	{
		var parts = this.ToArray();

		string? path = null;
		Validation.Validate(validatePath(parts), callback).ThrowIfFailed();
		return path!;

		bool callback() => !string.IsNullOrEmpty(path);
		IEnumerator<Exception> validatePath(string[] parts)
		{
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

			path = Path.Combine(parts);
		}
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
