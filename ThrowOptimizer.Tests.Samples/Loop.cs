using Mono.Cecil.Cil;
using System;

namespace ThrowOptimizer.Tests.Samples
{
	public static class Loop
	{
		[Throws(typeof(Exception), -2)]
		[Throws(typeof(Exception), -1)]
		[HasNoOpcodes(Code.Throw, Code.Newobj, Code.Ret)]
		public static void InfiniteLoop(int i)
		{
			while (true)
			{
				if (i < 0)
					throw new Exception();
			}
		}
	}
}
