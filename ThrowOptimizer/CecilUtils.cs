using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;

namespace ThrowOptimizer
{
	public static class CecilUtils
	{
		public static bool TypeEquals<T>(this TypeReference t) => t.TypeEquals(typeof(T));
		public static bool TypeEquals(this TypeReference t, Type type) => t?.FullName == type.FullName;

		public static bool HasAttribute<T>(this ICustomAttributeProvider t) where T : Attribute =>
			t.HasCustomAttributes && t.CustomAttributes.Any(attribute => attribute.AttributeType.TypeEquals<T>());
		public static bool IsStatic(this TypeDefinition t) => t.IsAbstract && t.IsSealed;

		public static bool Is<T>(this TypeReference t) => t.Is(typeof(T));
		public static bool Is(this TypeReference t, Type type)
		{
			if (t.TypeEquals(type))
				return true;

			TypeDefinition baseType = t.Resolve().BaseType?.Resolve();
			return baseType != null && baseType.Is(type) ||
					type.IsInterface && t.Resolve().HasInterfaces && t.Resolve().Interfaces.Any(implementation => implementation.InterfaceType.Resolve().Is(type)) ||
					type.IsGenericTypeDefinition && t is GenericInstanceType genericInstance && genericInstance.Resolve().Is(type);
		}

		public static AssemblyDefinition ReadAssembly(string path, bool readWrite, out bool symbolsAvailable, IAssemblyResolver resolver = null)
		{
			try
			{
				symbolsAvailable = true;
				return AssemblyDefinition.ReadAssembly(path, new ReaderParameters
				{ ReadSymbols = true, ReadWrite = readWrite, AssemblyResolver = resolver });
			}
			catch (SymbolsNotFoundException)
			{
				symbolsAvailable = false;
				return AssemblyDefinition.ReadAssembly(path, new ReaderParameters
				{ ReadSymbols = false, ReadWrite = readWrite, AssemblyResolver = resolver });
			}
		}
	}
}
