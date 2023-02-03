using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;
using System.Reflection;
using ThrowOptimizer.Tests.Samples;
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
		public void HasOpcodeTest()
		{
			foreach (MethodDefinition method in AssemblyCode.Instance.Assembly.Modules
														.Select(definition => definition.Types).Merge()
														.Select(definition => definition.Methods).Merge()
														.Where(definition => definition.HasAttribute<HasOpcodesAttribute>()))
			{
				_output.WriteLine(method.FullName);
				foreach (Code opcode in ((IEnumerable<CustomAttributeArgument>)method.CustomAttributes.First(attribute =>
							attribute.AttributeType.TypeEquals<HasOpcodesAttribute>()).ConstructorArguments[0].Value).Select(argument => (Code)argument.Value))
				{
					Assert.Contains(method.Body.Instructions.Select(instruction => instruction.OpCode.Code),
						code => code == opcode);
				}
			}
		}

		[Fact]
		public void HasNoOpcodeTest()
		{
			foreach (MethodDefinition method in AssemblyCode.Instance.Assembly.Modules
														.Select(definition => definition.Types).Merge()
														.Select(definition => definition.Methods).Merge()
														.Where(definition => definition.HasAttribute<HasNoOpcodesAttribute>()))
			{
				_output.WriteLine(method.FullName);
				foreach (Code opcode in ((IEnumerable<CustomAttributeArgument>)method.CustomAttributes.First(attribute =>
							attribute.AttributeType.TypeEquals<HasNoOpcodesAttribute>()).ConstructorArguments[0].Value).Select(argument => (Code)argument.Value))
				{
					Assert.DoesNotContain(method.Body.Instructions.Select(instruction => instruction.OpCode.Code),
						code => code == opcode);
				}
			}
		}
	}
}
