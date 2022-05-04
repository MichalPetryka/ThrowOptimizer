using System;
using System.Diagnostics;

namespace ThrowOptimizer.Tests.Code
{
	public static class ThrowHelper
	{
		[Throws(typeof(Exception))]
		[HasNoThrow]
		public static void CallThrowException() => ThrowException();
		[Throws(typeof(Exception))]
		[HasThrow]
		public static void ThrowException() => throw new Exception();
		[Throws(typeof(Exception))]
		[HasNoThrow]
		public static int CallThrowReturnPrimitiveException() => ThrowReturnPrimitiveException();
		[Throws(typeof(Exception))]
		[HasThrow]
		public static int ThrowReturnPrimitiveException() => throw new Exception();
		[Throws(typeof(Exception))]
		[HasNoThrow]
		public static string CallThrowReturnReferenceException() => ThrowReturnReferencException();
		[Throws(typeof(Exception))]
		[HasThrow]
		public static string ThrowReturnReferencException() => throw new Exception();

		[Throws(typeof(Exception))]
		[HasNoThrow]
		public static int CallThrowReturnDefaultPrimitiveException()
		{
			ThrowException();
			return default;
		}

		[Throws(typeof(Exception))]
		[HasNoThrow]
		public static string CallThrowReturnDefaultReferenceException()
		{
			ThrowException();
			return default;
		}

#if NET6_0 || NET6_0_OR_GREATER
		[Throws(typeof(Exception))]
		[HasNoThrow]
		public static void CallThrowExceptionStacktraceHidden() => ThrowException();
		[Throws(typeof(Exception))]
		[HasThrow]
		[StackTraceHidden]
		public static void ThrowExceptionStacktraceHidden() => throw new Exception();
		[Throws(typeof(Exception))]
		[HasNoThrow]
		public static int CallThrowReturnPrimitiveExceptionStacktraceHidden() => ThrowReturnPrimitiveException();
		[Throws(typeof(Exception))]
		[HasThrow]
		[StackTraceHidden]
		public static int ThrowReturnPrimitiveExceptionStacktraceHidden() => throw new Exception();
		[Throws(typeof(Exception))]
		[HasNoThrow]
		public static string CallThrowReturnReferenceExceptionStacktraceHidden() => ThrowReturnReferencException();
		[Throws(typeof(Exception))]
		[HasThrow]
		[StackTraceHidden]
		public static string ThrowReturnReferencExceptionStacktraceHidden() => throw new Exception();

		[Throws(typeof(Exception))]
		[HasNoThrow]
		public static int CallThrowReturnDefaultPrimitiveExceptionStacktraceHidden()
		{
			ThrowException();
			return default;
		}

		[Throws(typeof(Exception))]
		[HasNoThrow]
		public static string CallThrowReturnDefaultReferenceExceptionStacktraceHidden()
		{
			ThrowException();
			return default;
		}
#endif
	}
}
