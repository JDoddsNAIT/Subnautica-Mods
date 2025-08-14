#pragma warning disable IDE0130 // Namespace does not match folder structure
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace System.Runtime.CompilerServices;

public static partial class RuntimeHelpers
{
	internal static T[] GetSubArray<T>(this T[] array, Range range)
	{
		(int offset, int length) = range.GetOffsetAndLength(array.Length);
		var result = new T[length];
		for (int i = 0; i < length; i++)
		{
			result[i] = array[offset + i];
		}
		return result;
	}
}