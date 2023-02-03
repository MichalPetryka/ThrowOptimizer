using System;

namespace ThrowOptimizer.Tests.Samples
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class ThrowsAttribute : Attribute
	{
		public readonly Type Type;
		public readonly object[] Parameters;
		public ThrowsAttribute(Type type, params object[] parameters)
		{
			Type = type;
			Parameters = parameters;
		}
	}
}
