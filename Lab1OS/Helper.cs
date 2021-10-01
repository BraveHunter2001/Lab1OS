using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1OS
{
	public static class Helper
	{
		public static uint EnumToUint<TValue>(this TValue value) where TValue : Enum
				=> (uint)(object)value;
		public static IEnumerable<T> ParseFlags<T>(uint fileSystemFlags, IEnumerable<T> allFlags = null) where T : Enum
		{
			if (allFlags == null)
				allFlags = (T[])Enum.GetValues(typeof(T));
			foreach (var flag in allFlags)
			{
				if ((fileSystemFlags & flag.EnumToUint()) == flag.EnumToUint())
					yield return flag;
			}
		}
	}
}
