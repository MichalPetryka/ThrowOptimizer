using Mono.Cecil.Cil;
using System;

namespace ThrowOptimizer.Tests.Samples
{
	[AttributeUsage(AttributeTargets.Method)]
	public class HasOpcodesAttribute : Attribute
	{
		public Code[] Opcodes { get; }

		public HasOpcodesAttribute(params Code[] opcodes)
		{
			Opcodes = opcodes;
		}
	}
}
