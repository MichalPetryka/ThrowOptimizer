using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace ThrowOptimizer.Utils
{
	internal static class ListPool<T>
	{
		private static readonly ConcurrentQueue<List<T>> Pool = new();
		// ReSharper disable once StaticMemberInGenericType
		private static volatile int _poolCounter;

		private const int DefaultCapacity = 512;
		private const int MaxCapacity = 512;
		private const int PoolSize = 512;

		public static List<T> Rent()
		{
			Interlocked.Decrement(ref _poolCounter);
			return Pool.TryDequeue(out List<T> list) ? list : new List<T>(DefaultCapacity);
		}

		public static List<T> Rent(int capacity)
		{
			Interlocked.Decrement(ref _poolCounter);

			if (Pool.TryDequeue(out List<T> list))
			{
				if (list.Capacity < capacity)
					list.Capacity = capacity;
				return list;
			}

			return new List<T>(Math.Max(DefaultCapacity, capacity));
		}

		public static List<T> Rent(T first)
		{
			Interlocked.Decrement(ref _poolCounter);

			if (Pool.TryDequeue(out List<T> list))
			{
				list.Add(first);
				return list;
			}

			return new List<T>(DefaultCapacity) { first };
		}

		public static List<T> Rent(T first, T second)
		{
			Interlocked.Decrement(ref _poolCounter);

			if (Pool.TryDequeue(out List<T> list))
			{
				list.Add(first);
				list.Add(second);
				return list;
			}

			return new List<T>(DefaultCapacity) { first, second };
		}

		public static List<T> Rent(T first, T second, T third)
		{
			Interlocked.Decrement(ref _poolCounter);

			if (Pool.TryDequeue(out List<T> list))
			{
				list.Add(first);
				list.Add(second);
				list.Add(third);
				return list;
			}

			return new List<T>(DefaultCapacity) { first, second, third };
		}

		public static List<T> Rent(params T[] values)
		{
			Interlocked.Decrement(ref _poolCounter);

			List<T> list = Pool.TryDequeue(out List<T> pooledList) ? pooledList : new List<T>(DefaultCapacity);
			list.AddRange(values);
			return list;
		}

		public static void Return(List<T> list)
		{
			if (list.Capacity > MaxCapacity || Interlocked.Increment(ref _poolCounter) > PoolSize)
				return;
			list.Clear();
			Pool.Enqueue(list);
		}
	}
}
