using System;
using System.Collections.Generic;

namespace ThrowOptimizer.Utils
{
	internal static class ListPoolHelpers
	{
		public static TReturn ToPooled<T, TReturn>(this IEnumerable<T> enumerable, Func<List<T>, TReturn> function)
		{
			List<T> list = ListPool<T>.Rent();
			list.AddRange(enumerable);
			TReturn ret = function(list);
			ListPool<T>.Return(list);
			return ret;
		}
	}
}