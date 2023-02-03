using Mono.Cecil.Cil;
using System;

namespace ThrowOptimizer.Tests.Samples
{
	public static class Goto
	{
		[Throws(typeof(Exception), -2)]
		[Throws(typeof(Exception), -1)]
		[Throws(typeof(Exception), 0)]
		[Throws(typeof(Exception), 1)]
		[HasOpcodes(Code.Throw, Code.Newobj)]
		public static void GotoAlwaysThrow(int i)
		{
			string s;

			if (i < 0)
			{
				s = "b";
				goto label;
			}

			s = "a";
		label:
			throw new Exception(s);
		}

		[Throws(typeof(Exception))]
		[HasOpcodes(Code.Throw, Code.Newobj)]
		public static void GotoAlwaysThrowBadLayout()
		{
			goto label1;
		label2:
			throw new Exception();
		label1:
			goto label2;
		}


		[Throws(typeof(Exception), -2)]
		[Throws(typeof(Exception), -1)]
		[HasNoOpcodes(Code.Throw, Code.Newobj, Code.Ret)]
		public static void GotoInfiniteLoop(int i)
		{
			while (true)
			{
				if (i < 0)
					goto label;
			}
		label:
			throw new Exception();
		}

		[Throws(typeof(Exception), -2)]
		[Throws(typeof(Exception), -1)]
		[DoesNotThrow(0)]
		[DoesNotThrow(1)]
		[HasNoOpcodes(Code.Throw, Code.Newobj, Code.Ret)]
		public static void GotoThrow(int i)
		{
			if (i < 0)
			{
				goto label;
			}

			return;
		label:
			throw new Exception();
		}
	}
}
