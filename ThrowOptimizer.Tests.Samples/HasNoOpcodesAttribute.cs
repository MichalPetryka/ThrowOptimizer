using Mono.Cecil.Cil;
using System;

namespace ThrowOptimizer.Tests.Samples
{
	[AttributeUsage(AttributeTargets.Method)]
	public class HasNoOpcodesAttribute : Attribute
	{
		public Code[] Opcodes { get; }

		public HasNoOpcodesAttribute(params Code[] opcodes)
		{
			Opcodes = opcodes;
		}
	}
}
