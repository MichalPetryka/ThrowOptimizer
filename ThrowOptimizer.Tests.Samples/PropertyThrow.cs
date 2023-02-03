using Mono.Cecil.Cil;
using System;

namespace ThrowOptimizer.Tests.Samples
{
	public static class PropertyThrow
	{
		public static int CallThrowReturnPrimitiveException
		{
			[Throws(typeof(Exception))]
			[HasNoOpcodes(Code.Throw, Code.Newobj)]
			get => ThrowReturnPrimitiveException;
			[Throws(typeof(Exception), 0)]
			[HasNoOpcodes(Code.Throw, Code.Newobj)]
			set => ThrowReturnPrimitiveException = value;
		}

		public static int ThrowReturnPrimitiveException
		{
			[Throws(typeof(Exception))]
			[HasOpcodes(Code.Throw, Code.Newobj)]
			get => throw new Exception();
			[Throws(typeof(Exception), 0)]
			[HasOpcodes(Code.Throw, Code.Newobj)]
			set => throw new Exception();
		}

		public static string CallThrowReturnReferenceException
		{
			[Throws(typeof(Exception))]
			[HasNoOpcodes(Code.Throw, Code.Newobj)]
			get => ThrowReturnReferenceException;
			[Throws(typeof(Exception), "")]
			[HasNoOpcodes(Code.Throw, Code.Newobj)]
			set => ThrowReturnReferenceException = value;
		}

		public static string ThrowReturnReferenceException
		{
			[Throws(typeof(Exception))]
			[HasOpcodes(Code.Throw, Code.Newobj)]
			get => throw new Exception();
			[Throws(typeof(Exception), "")]
			[HasOpcodes(Code.Throw, Code.Newobj)]
			set => throw new Exception();
		}

		public static int CallThrowReturnDefaultPrimitiveException
		{
			[Throws(typeof(Exception))]
			[HasNoOpcodes(Code.Throw, Code.Newobj)]
			get
			{
				ThrowException();
				return default;
			}
			[Throws(typeof(Exception), 0)]
			[HasNoOpcodes(Code.Throw, Code.Newobj)]
			// ReSharper disable once ValueParameterNotUsed
			set => ThrowException();
		}

		public static string CallThrowReturnDefaultReferenceException
		{
			[Throws(typeof(Exception))]
			[HasNoOpcodes(Code.Throw, Code.Newobj)]
			get
			{
				ThrowException();
				return default;
			}
			[Throws(typeof(Exception), "")]
			[HasNoOpcodes(Code.Throw, Code.Newobj)]
			// ReSharper disable once ValueParameterNotUsed
			set => ThrowException();
		}

		[HasOpcodes(Code.Throw, Code.Newobj)]
		public static void ThrowException() => throw new Exception();
	}
}
