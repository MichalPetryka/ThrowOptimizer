using System;
using System.Linq;
using System.Reflection;
using ThrowOptimizer.Tests.Code;
using Xunit;
using Xunit.Abstractions;

namespace ThrowOptimizer.Tests
{
	public partial class IlVerifyTests
	{
		private readonly ITestOutputHelper _output;

		public IlVerifyTests(ITestOutputHelper output)
		{
			_output = output;
		}

		[Fact]
		public void VerifyTest()
		{
			string references = "";
			foreach (Assembly referencedAssembly in AppDomain.CurrentDomain.GetAssemblies().Where(ass => !ass.IsDynamic))
				references += $" -r \"{referencedAssembly.Location}\"";

			string arguments = $"ilverify \"{typeof(ThrowHelper).Assembly.Location}\" -s \"{typeof(object).Assembly.GetName().Name}\" {references}";
			int exitCode = ProcessUtils.GetProcessOutput("dotnet", arguments, out string output, out string error);
			_output.WriteLine($"Output: {output}");
			_output.WriteLine($"Error: {error}");
			Assert.Equal(0, exitCode);
		}
	}
}
