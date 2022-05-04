﻿using System;

namespace ThrowOptimizer.Tests.Code
{
	public static class SimpleCondition
	{
		[Throws(typeof(Exception), -2)]
		[Throws(typeof(Exception), -1)]
		[DoesNotThrow(0)]
		[DoesNotThrow(1)]
		[DoesNotThrow(2)]
		[HasNoThrow]
		public static void PrimitiveBranch(int i)
		{
			if (i < 0)
				throw new Exception();
		}

		[Throws(typeof(Exception), -2)]
		[Throws(typeof(Exception), -1)]
		[DoesNotThrow(0)]
		[DoesNotThrow(1)]
		[DoesNotThrow(2)]
		[HasNoThrow]
		public static int PrimitiveBranchReturn(int i)
		{
			if (i < 0)
				throw new Exception();
			return i;
		}

		[Throws(typeof(Exception), -2)]
		[Throws(typeof(Exception), -1)]
		[DoesNotThrow(0)]
		[Throws(typeof(Exception), 1)]
		[Throws(typeof(Exception), 2)]
		[HasNoThrow]
		public static void PrimitiveBranchMessage(int i)
		{
			if (i != 0)
				throw new Exception(i < 0 ? "Must be zero, not negative" : "Must be zero, not positive");
		}

		[Throws(typeof(Exception), -2)]
		[Throws(typeof(Exception), -1)]
		[DoesNotThrow(0)]
		[Throws(typeof(Exception), 1)]
		[Throws(typeof(Exception), 2)]
		[HasNoThrow]
		public static int PrimitiveBranchMessageReturn(int i)
		{
			if (i != 0)
				throw new Exception(i < 0 ? "Must be zero, not negative" : "Must be zero, not positive");
			return i;
		}

		[Throws(typeof(Exception), -2)]
		[Throws(typeof(Exception), -1)]
		[DoesNotThrow(0)]
		[Throws(typeof(InvalidOperationException), 1)]
		[Throws(typeof(InvalidOperationException), 2)]
		[HasNoThrow]
		public static void PrimitiveBranchException(int i)
		{
			if (i != 0)
				throw i < 0 ? new Exception() : new InvalidOperationException();
		}

		[Throws(typeof(Exception), -2)]
		[Throws(typeof(Exception), -1)]
		[DoesNotThrow(0)]
		[Throws(typeof(InvalidOperationException), 1)]
		[Throws(typeof(InvalidOperationException), 2)]
		[HasNoThrow]
		public static int PrimitiveBranchExceptionReturn(int i)
		{
			if (i != 0)
				throw i < 0 ? new Exception() : new InvalidOperationException();
			return i;
		}

		[Throws(typeof(Exception), -2)]
		[Throws(typeof(Exception), -1)]
		[DoesNotThrow(0)]
		[Throws(typeof(InvalidOperationException), 1)]
		[Throws(typeof(InvalidOperationException), 2)]
		[HasNoThrow]
		public static void PrimitiveBranchThrow(int i)
		{
			if (i != 0)
				if (i < 0)
					throw new Exception();
				else
					throw new InvalidOperationException();
		}

		[Throws(typeof(Exception), -2)]
		[Throws(typeof(Exception), -1)]
		[DoesNotThrow(0)]
		[Throws(typeof(InvalidOperationException), 1)]
		[Throws(typeof(InvalidOperationException), 2)]
		[HasNoThrow]
		public static int PrimitiveBranchThrowReturn(int i)
		{
			if (i != 0)
				if (i < 0)
					throw new Exception();
				else
					throw new InvalidOperationException();
			return i;
		}

		[Throws(typeof(ArgumentOutOfRangeException), -2)]
		[Throws(typeof(ArgumentOutOfRangeException), -1)]
		[DoesNotThrow(0)]
		[DoesNotThrow(1)]
		[DoesNotThrow(2)]
		[HasNoThrow]
		public static void PrimitiveBranchParamater(int i)
		{
			if (i < 0)
				throw new ArgumentOutOfRangeException(nameof(i), i, "Must be positive");
		}

		[Throws(typeof(ArgumentOutOfRangeException), -2)]
		[Throws(typeof(ArgumentOutOfRangeException), -1)]
		[DoesNotThrow(0)]
		[DoesNotThrow(1)]
		[DoesNotThrow(2)]
		[HasNoThrow]
		public static int PrimitiveBranchReturnParameter(int i)
		{
			if (i < 0)
				throw new ArgumentOutOfRangeException(nameof(i), i, "Must be positive");
			return i;
		}
	}
}
