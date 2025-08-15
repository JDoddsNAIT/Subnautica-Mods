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
		return Path.Combine(this.ToArray());
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
