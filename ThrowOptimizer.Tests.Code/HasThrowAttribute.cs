using System;

namespace ThrowOptimizer.Tests.Code
{
	[AttributeUsage(AttributeTargets.Method)]
	public class HasThrowAttribute : Attribute
	{
	}
}
