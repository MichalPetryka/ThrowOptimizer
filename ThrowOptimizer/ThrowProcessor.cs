using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ThrowOptimizer.Configuration;
using ThrowOptimizer.Utils;

namespace ThrowOptimizer
{
	public sealed class ThrowProcessor : IDisposable
	{
		private readonly string _assemblyPath;
		private readonly string[] _references;
		private readonly LogHandler.ILogHandler _logHandler;
		private readonly ProcessingConfiguration _configuration;
		private readonly bool _symbolsAvailable;
		private readonly DefaultAssemblyResolver _resolver;
		private readonly AssemblyDefinition _assembly;
		private bool _successful = true;

		public ThrowProcessor(string assemblyPath, IEnumerable<string> references, LogHandler.ILogHandler logHandler, ProcessingConfiguration configuration)
		{
			_assemblyPath = assemblyPath;
			_references = references.ToArray();
			_logHandler = logHandler;
			_configuration = configuration;
			_resolver = new DefaultAssemblyResolver();
			foreach (string reference in _references)
				_resolver.AddSearchDirectory(reference);
			_assembly = CecilUtils.ReadAssembly(assemblyPath, true, out _symbolsAvailable, _resolver);
		}

		private void Log(string message, LogHandler.LogType logType)
		{
			if (logType == LogHandler.LogType.Error)
				_successful = false;
			_logHandler.Log(message, logType);
		}

		public void Process()
		{
			try
			{
				foreach (ModuleDefinition module in _assembly.Modules)
					foreach (TypeDefinition type in module.Types)
						ProcessType(type);
			}
			catch
			{
				_successful = false;
				throw;
			}
		}

		private void ProcessType(TypeDefinition type)
		{
			foreach (TypeDefinition nestedType in type.NestedTypes)
				ProcessType(nestedType);

			foreach (MethodDefinition method in type.Methods)
				ProcessMethod(method);
		}

		private void ProcessMethod(MethodDefinition method)
		{
			if (!method.HasBody)
				return;

			if (method.Body.Instructions.All(instruction => instruction.OpCode.Code != Code.Throw) || method.Body.Instructions.All(instruction => instruction.OpCode.Code != Code.Ret))
				return;

			Collection<Instruction> body = method.Body.Instructions;

			foreach (int throwIndex in body.Select(instruction => instruction.OpCode.Code).ToArray().IndexOfAll(Code.Throw).Reverse())
			{
				// ReSharper disable once HeapView.BoxingAllocation
				MethodDefinition throwHelper = new($"ThrowGeneratedException{method.Name}{throwIndex}", MethodAttributes.Static | MethodAttributes.Private, method.ReturnType)
				{
					NoInlining = _configuration.NoInline,
					Body =
					{
						InitLocals = _configuration.LocalsInitMode switch
						{
							LocalsInit.KeepOriginal => method.Body.InitLocals,
							LocalsInit.Skip => false,
							LocalsInit.Add => true,
							_ => throw new ArgumentOutOfRangeException()
						}
					}
				};

				method.DeclaringType.Methods.Add(throwHelper);

				Collection<Instruction> instructions = method.Body.Instructions;
				List<int> starts = ListPool<int>.Rent(first: 0);
				for (int i = 0; i < instructions.Count; i++)
				{
					Instruction instruction = instructions[i];

					switch (instruction.OpCode.Code)
					{

						//todo: try catch finally
						case Code.Br or Code.Br_S:
							starts.Add(((Instruction)instruction.Operand).Offset);
							break;
						case Code.Brfalse or Code.Brfalse_S or
							Code.Brtrue or Code.Brtrue_S or
							Code.Beq or Code.Beq_S or
							Code.Bge or Code.Bge_S or Code.Bge_Un or Code.Bge_Un_S or
							Code.Bgt or Code.Bgt_S or Code.Bgt_Un or Code.Bgt_Un_S or
							Code.Ble or Code.Ble_S or Code.Ble_Un or Code.Ble_Un or
							Code.Blt or Code.Blt_S or Code.Blt_Un or Code.Blt_Un_S or
							Code.Bne_Un or Code.Bne_Un_S:
							starts.Add(instruction.Offset);
							starts.Add(((Instruction)instruction.Operand).Offset);
							break;
						case Code.Switch:
							starts.Add(instruction.Offset);
							foreach (Instruction target in (Instruction[])instruction.Operand)
								starts.Add(target.Offset);
							break;
					}
				}
				/*List<Instruction> instructions = new();
				for (int i = throwIndex; i >= 0; i--)
				{
					if (method.Body.Instructions[i].OpCode.Code is Code.Br or Code.Br_S or
						Code.Brfalse or Code.Brfalse_S or
						Code.Brtrue or Code.Brtrue_S or
						Code.Beq or Code.Beq_S or
						Code.Bge or Code.Bge_S or Code.Bge_Un or Code.Bge_Un_S or
						Code.Bgt or Code.Bgt_S or Code.Bgt_Un or Code.Bgt_Un_S or
						Code.Ble or Code.Ble_S or Code.Ble_Un or Code.Ble_Un or
						Code.Blt or Code.Blt_S or Code.Blt_Un or Code.Blt_Un_S or
						Code.Bne_Un or Code.Bne_Un_S)
					{
						methodProcessor.InsertAfter(i, Instruction.Create(OpCodes.Call, throwHelper));
						if (!method.ReturnType.Is(typeof(void)))
							methodProcessor.InsertAfter(i + 1, Instruction.Create(OpCodes.Ret));
						break;
					}

					instructions.Add(method.Body.Instructions[i]);
					methodProcessor.RemoveAt(i);
				}

				for (int i = instructions.Count - 1; i >= 0; i--)
					throwProcessor.Append(instructions[i]);*/
			}

			method.Body.Optimize();
		}

		public void Dispose()
		{
			if (_successful)
			{
				_assembly.Write(new WriterParameters { WriteSymbols = _symbolsAvailable });

				if (_configuration.IlVerify)
				{
					string systemLib = "mscorlib";
					foreach (string reference in _references)
						if (File.Exists(Path.Combine(reference, "System.Private.CoreLib.dll")))
							systemLib = "System.Private.CoreLib";
					string arguments = $"ilverify \"{_assemblyPath}\" -s {systemLib} -r \"{string.Join("\" -r \"", _references)}\"";
					int exitCode = ProcessUtils.GetProcessOutput("dotnet", arguments, out string output, out string error);
					if (exitCode != 0)
						Log($"{error}\n{output}".Trim(), LogHandler.LogType.Error);
				}
			}

			_assembly.Dispose();
			_resolver.Dispose();
		}
	}
}
