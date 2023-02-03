using System;

namespace ThrowOptimizer.Tests.Samples
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class DoesNotThrowAttribute : Attribute
	{
		public readonly object[] Parameters;
		public DoesNotThrowAttribute(params object[] parameters)
		{
			Parameters = parameters;
		}
	}
}
