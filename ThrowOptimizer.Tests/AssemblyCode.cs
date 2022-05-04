using Mono.Cecil;
using System;
using System.IO;
using System.Linq;
using ThrowOptimizer.Tests.Code;

namespace ThrowOptimizer.Tests
{
	public class AssemblyCode
	{
		public static readonly AssemblyCode Instance = new();
		public readonly AssemblyDefinition Assembly;
		private readonly DefaultAssemblyResolver _assemblyResolver;

		private AssemblyCode()
		{
			_assemblyResolver = new DefaultAssemblyResolver();
			foreach (string referencedAssembly in AppDomain.CurrentDomain.GetAssemblies()
														.Where(ass => !ass.IsDynamic)
														.Select(ass => Path.GetDirectoryName(ass.Location)).ToHashSet())
				_assemblyResolver.AddSearchDirectory(referencedAssembly);
			Assembly = CecilUtils.ReadAssembly(typeof(ThrowHelper).Assembly.Location, false, out _, _assemblyResolver);
		}

		~AssemblyCode()
		{
			Assembly.Dispose();
			_assemblyResolver.Dispose();
		}
	}
}
