using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrootLuips.RPGElements.Utilities;
internal static partial class Utilities
{
	public static bool IsNullOrEmpty<T>(IReadOnlyCollection<T> collection)
	{
		return collection is null || collection.Count is 0;
	}
}
