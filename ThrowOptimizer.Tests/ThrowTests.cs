using Mono.Cecil;
using System.Linq;
using System.Reflection;
using ThrowOptimizer.Tests.Code;
using ThrowOptimizer.Utils;
using Xunit;
using Xunit.Abstractions;

namespace ThrowOptimizer.Tests
{
	public class ThrowTests
	{
		private readonly ITestOutputHelper _output;

		public ThrowTests(ITestOutputHelper output)
		{
			_output = output;
		}

		[Fact]
		public void ThrowsTest()
		{
			foreach (MethodInfo method in typeof(ThrowsAttribute).Assembly.Modules.Select(module => module.GetTypes()).Merge()
															.Select(type => type.GetMethods()).Merge())
			{
				ThrowsAttribute[] throwsAttributes = method.GetCustomAttributes<ThrowsAttribute>().ToArray();
				if (throwsAttributes.Length == 0)
					continue;
				_output.WriteLine($"{method.DeclaringType?.Name} {method}");
				foreach (ThrowsAttribute throwsAttribute in throwsAttributes)
					Assert.Equal(throwsAttribute.Type, Assert.ThrowsAny<TargetInvocationException>(
						() => method.Invoke(null, throwsAttribute.Parameters)).InnerException?.GetType());
			}
		}

		[Fact]
		public void DoesNotThrowTest()
		{
			foreach (MethodInfo method in typeof(DoesNotThrowAttribute).Assembly.Modules.Select(module => module.GetTypes()).Merge()
															.Select(type => type.GetMethods()).Merge())
			{
				DoesNotThrowAttribute[] doesNotThrowAttributes = method.GetCustomAttributes<DoesNotThrowAttribute>().ToArray();
				if (doesNotThrowAttributes.Length == 0)
					continue;
				_output.WriteLine($"{method.DeclaringType?.Name} {method}");
				foreach (DoesNotThrowAttribute doesNotThrowAttribute in doesNotThrowAttributes)
					method.Invoke(null, doesNotThrowAttribute.Parameters);
			}
		}

		[Fact]
		public void HasThrowTest()
		{
			foreach (MethodDefinition method in AssemblyCode.Instance.Assembly.Modules
														.Select(definition => definition.Types).Merge()
														.Select(definition => definition.Methods).Merge()
														.Where(definition => definition.HasAttribute<HasThrowAttribute>()))
			{
				_output.WriteLine(method.FullName);
				Assert.Contains(method.Body.Instructions.Select(instruction => instruction.OpCode.Code),
					code => code == Mono.Cecil.Cil.Code.Throw);
			}
		}

		[Fact]
		public void HasNoThrowTest()
		{
			foreach (MethodDefinition method in AssemblyCode.Instance.Assembly.Modules
														.Select(definition => definition.Types).Merge()
														.Select(definition => definition.Methods).Merge()
														.Where(definition => definition.HasAttribute<HasNoThrowAttribute>()))
			{
				_output.WriteLine(method.FullName);
				Assert.DoesNotContain(method.Body.Instructions.Select(instruction => instruction.OpCode.Code),
					code => code == Mono.Cecil.Cil.Code.Throw);
			}
		}
	}
}
