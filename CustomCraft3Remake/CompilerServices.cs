namespace System.Runtime.CompilerServices;
public static partial class RuntimeHelpers
{
	internal static T[] GetSubArray<T>(this T[] values, System.Range range)
	{
		(int offset, int length) = range.GetOffsetAndLength(values.Length);
		var result = new T[length];
		for (int i = 0; i < length; i++)
		{
			result[i] = values[i + offset];
		}
		return result;
	}
}