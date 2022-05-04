using System;
using System.Collections.Generic;

namespace ThrowOptimizer.Utils
{
	public static class CollectionUtils
	{
		public static IEnumerable<T> Merge<T>(this IEnumerable<IEnumerable<T>> enumerables)
		{
			foreach (IEnumerable<T> enumerable in enumerables)
				foreach (T value in enumerable)
					yield return value;
		}

		public static IEnumerable<int> IndexOfAll<T>(this IReadOnlyList<T> collection, T value)
		{
			int index = -1;
			while (true)
			{
				index = collection.IndexOf(value, index + 1);
				if (index == -1)
					yield break;
				yield return index;
			}
		}

		public static int IndexOf<T>(this IReadOnlyList<T> readOnlyList, T value, int startIndex)
		{
			switch (readOnlyList)
			{
				case T[] array:
					return Array.IndexOf(array, value, startIndex);
				case List<T> list:
					return list.IndexOf(value, startIndex);
			}

			for (int i = startIndex; i < readOnlyList.Count; i++)
				if (EqualityComparer<T>.Default.Equals(readOnlyList[i], value))
					return i;
			return -1;
		}

		public static T At<T>(this ArraySegment<T> segment, int index)
		{
			return segment.Array![segment.Offset + index];
		}

		public static ArraySegment<T> Slice<T>(this T[] array, int startIndex)
		{
			return new(array, startIndex, array.Length - startIndex);
		}

		public static ArraySegment<T> Slice<T>(this ArraySegment<T> segment, int startIndex)
		{
			return new(segment.Array!, segment.Offset + startIndex, segment.Count - startIndex);
		}

		public static ArraySegment<T> Slice<T>(this T[] array, int startIndex, int count)
		{
			return new(array, startIndex, count);
		}

		public static ArraySegment<T> Slice<T>(this ArraySegment<T> segment, int startIndex, int count)
		{
			return new(segment.Array!, segment.Offset + startIndex, count);
		}
	}
}