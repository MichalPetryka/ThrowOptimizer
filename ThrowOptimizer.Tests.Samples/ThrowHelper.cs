using Mono.Cecil.Cil;
using System;
using System.Diagnostics;

namespace ThrowOptimizer.Tests.Samples
{
	public static class ThrowHelper
	{
		[Throws(typeof(Exception))]
		[HasNoOpcodes(Code.Throw, Code.Newobj)]
		[HasOpcodes(Code.Call)]
		public static void CallThrowException() => ThrowException();
		[Throws(typeof(Exception))]
		[HasOpcodes(Code.Throw, Code.Newobj)]
		public static void ThrowException() => throw new Exception();
		[Throws(typeof(Exception))]
		[HasNoOpcodes(Code.Throw, Code.Newobj)]
		[HasOpcodes(Code.Call)]
		public static int CallThrowReturnPrimitiveException() => ThrowReturnPrimitiveException();
		[Throws(typeof(Exception))]
		[HasOpcodes(Code.Throw, Code.Newobj)]
		public static int ThrowReturnPrimitiveException() => throw new Exception();
		[Throws(typeof(Exception))]
		[HasNoOpcodes(Code.Throw, Code.Newobj)]
		[HasOpcodes(Code.Call)]
		public static string CallThrowReturnReferenceException() => ThrowReturnReferenceException();
		[Throws(typeof(Exception))]
		[HasOpcodes(Code.Throw, Code.Newobj)]
		public static string ThrowReturnReferenceException() => throw new Exception();

		[Throws(typeof(Exception))]
		[HasNoOpcodes(Code.Throw, Code.Newobj)]
		[HasOpcodes(Code.Call)]
		public static int CallThrowReturnDefaultPrimitiveException()
		{
			ThrowException();
			return default;
		}

		[Throws(typeof(Exception))]
		[HasNoOpcodes(Code.Throw, Code.Newobj)]
		[HasOpcodes(Code.Call)]
		public static string CallThrowReturnDefaultReferenceException()
		{
			ThrowException();
			return default;
		}

		[Throws(typeof(NullReferenceException))]
		[HasNoOpcodes(Code.Throw, Code.Ldnull)]
		[HasOpcodes(Code.Call)]
		public static void CallThrowNull() => ThrowNull();
		[Throws(typeof(NullReferenceException))]
		[HasOpcodes(Code.Throw, Code.Ldnull)]
		public static void ThrowNull() => throw null!;
		[Throws(typeof(NullReferenceException))]
		[HasNoOpcodes(Code.Throw, Code.Ldnull)]
		[HasOpcodes(Code.Call)]
		public static int CallThrowReturnPrimitiveNull() => ThrowReturnPrimitiveNull();
		[Throws(typeof(NullReferenceException))]
		[HasOpcodes(Code.Throw, Code.Ldnull)]
		public static int ThrowReturnPrimitiveNull() => throw null!;
		[Throws(typeof(NullReferenceException))]
		[HasNoOpcodes(Code.Throw, Code.Ldnull)]
		[HasOpcodes(Code.Call)]
		public static string CallThrowReturnReferenceNull() => ThrowReturnReferenceNull();
		[Throws(typeof(NullReferenceException))]
		[HasOpcodes(Code.Throw, Code.Ldnull)]
		public static string ThrowReturnReferenceNull() => throw null!;

		[Throws(typeof(NullReferenceException))]
		[HasNoOpcodes(Code.Throw, Code.Ldnull)]
		[HasOpcodes(Code.Call)]
		public static int CallThrowReturnDefaultPrimitiveNull()
		{
			ThrowNull();
			return default;
		}

		[Throws(typeof(NullReferenceException))]
		[HasNoOpcodes(Code.Throw, Code.Ldnull)]
		[HasOpcodes(Code.Call)]
		public static string CallThrowReturnDefaultReferenceNull()
		{
			ThrowNull();
			return default;
		}

#if NET6_0 || NET6_0_OR_GREATER
		[Throws(typeof(Exception))]
		[HasNoOpcodes(Code.Throw, Code.Newobj)]
		[HasOpcodes(Code.Call)]
		public static void CallThrowExceptionStacktraceHidden() => ThrowException();
		[Throws(typeof(Exception))]
		[HasOpcodes(Code.Throw, Code.Newobj)]
		[StackTraceHidden]
		public static void ThrowExceptionStacktraceHidden() => throw new Exception();
		[Throws(typeof(Exception))]
		[HasNoOpcodes(Code.Throw, Code.Newobj)]
		[HasOpcodes(Code.Call)]
		public static int CallThrowReturnPrimitiveExceptionStacktraceHidden() => ThrowReturnPrimitiveException();
		[Throws(typeof(Exception))]
		[HasOpcodes(Code.Throw, Code.Newobj)]
		[StackTraceHidden]
		public static int ThrowReturnPrimitiveExceptionStacktraceHidden() => throw new Exception();
		[Throws(typeof(Exception))]
		[HasNoOpcodes(Code.Throw, Code.Newobj)]
		[HasOpcodes(Code.Call)]
		public static string CallThrowReturnReferenceExceptionStacktraceHidden() => ThrowReturnReferenceException();
		[Throws(typeof(Exception))]
		[HasOpcodes(Code.Throw, Code.Newobj)]
		[StackTraceHidden]
		public static string ThrowReturnReferenceExceptionStacktraceHidden() => throw new Exception();

		[Throws(typeof(Exception))]
		[HasNoOpcodes(Code.Throw, Code.Newobj)]
		[HasOpcodes(Code.Call)]
		public static int CallThrowReturnDefaultPrimitiveExceptionStacktraceHidden()
		{
			ThrowException();
			return default;
		}

		[Throws(typeof(Exception))]
		[HasNoOpcodes(Code.Throw, Code.Newobj)]
		[HasOpcodes(Code.Call)]
		public static string CallThrowReturnDefaultReferenceExceptionStacktraceHidden()
		{
			ThrowException();
			return default;
		}
#endif
	}
}
