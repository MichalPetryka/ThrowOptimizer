using System;

namespace ThrowOptimizer.Tests.Code
{
	public static class PropertyThrow
	{
		public static int CallThrowReturnPrimitiveException
		{
			[Throws(typeof(Exception))]
			[HasNoThrow]
			get => ThrowReturnPrimitiveException;
			[Throws(typeof(Exception), 0)]
			[HasNoThrow]
			set => ThrowReturnPrimitiveException = value;
		}

		public static int ThrowReturnPrimitiveException
		{
			[Throws(typeof(Exception))]
			[HasThrow]
			get => throw new Exception();
			[Throws(typeof(Exception), 0)]
			[HasThrow]
			set => throw new Exception();
		}

		public static string CallThrowReturnReferenceException
		{
			[Throws(typeof(Exception))]
			[HasNoThrow]
			get => ThrowReturnReferenceException;
			[Throws(typeof(Exception), "")]
			[HasNoThrow]
			set => ThrowReturnReferenceException = value;
		}

		public static string ThrowReturnReferenceException
		{
			[Throws(typeof(Exception))]
			[HasThrow]
			get => throw new Exception();
			[Throws(typeof(Exception), "")]
			[HasThrow]
			set => throw new Exception();
		}

		public static int CallThrowReturnDefaultPrimitiveException
		{
			[Throws(typeof(Exception))]
			[HasNoThrow]
			get
			{
				ThrowException();
				return default;
			}
			[Throws(typeof(Exception), 0)]
			[HasNoThrow]
			// ReSharper disable once ValueParameterNotUsed
			set => ThrowException();
		}

		public static string CallThrowReturnDefaultReferenceException
		{
			[Throws(typeof(Exception))]
			[HasNoThrow]
			get
			{
				ThrowException();
				return default;
			}
			[Throws(typeof(Exception), "")]
			[HasNoThrow]
			// ReSharper disable once ValueParameterNotUsed
			set => ThrowException();
		}

		[HasThrow]
		public static void ThrowException() => throw new Exception();
	}
}
