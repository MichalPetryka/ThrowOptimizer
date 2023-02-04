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

			foreach (MethodDefinition method in type.Methods.ToArray())
				ProcessMethod(method);
		}

		private void ProcessMethod(MethodDefinition method)
		{
			if (!method.HasBody)
				return;

			if (method.Body.Instructions.All(instruction => instruction.OpCode.Code != Code.Throw))
				return;

			List<(int segmentStart, int segmentEnd)> starts = ListPool<(int, int)>.Rent(first: (0, method.Body.Instructions.Count - 1));

			for (int i = 0; i < method.Body.Instructions.Count; i++)
			{
				Instruction instruction = method.Body.Instructions[i];

				switch (instruction.OpCode.Code)
				{
					//todo: try catch finally
					case Code.Br or Code.Br_S:
						starts.Add((i + 1, method.Body.Instructions.IndexOf((Instruction)instruction.Operand)));
						break;
					case Code.Brfalse or Code.Brfalse_S or Code.Brtrue or Code.Brtrue_S or Code.Beq or Code.Beq_S or Code.Bge or Code.Bge_S or Code.Bge_Un or Code.Bge_Un_S or Code.Bgt or Code.Bgt_S or Code.Bgt_Un or Code.Bgt_Un_S or Code.Ble or Code.Ble_S or Code.Ble_Un or Code.Ble_Un or Code.Blt or Code.Blt_S or Code.Blt_Un or Code.Blt_Un_S or Code.Bne_Un or Code.Bne_Un_S:
						starts.Add((i + 1, method.Body.Instructions.IndexOf((Instruction)instruction.Operand)));
						break;
					case Code.Switch:
						foreach (Instruction target in (IEnumerable<Instruction>)instruction.Operand)
							starts.Add((i + 1, method.Body.Instructions.IndexOf(target)));
						break;
				}
			}

			if (starts.Count < 2)
				return;

			starts.Sort((left, right) =>
				left.segmentStart == right.segmentStart ? left.segmentEnd.CompareTo(right.segmentEnd) : left.segmentStart.CompareTo(right.segmentStart));

			Instruction[] body = method.Body.Instructions.ToArray();

			foreach (int throwIndex in body.Select(instruction => instruction.OpCode.Code).ToArray().IndexOfAll(Code.Throw).Reverse())
			{
				int start = 0;

				for (int i = starts.Count - 1; i >= 0; i--)
				{
					(int segmentStart, int segmentEnd) = starts[i];

					if (segmentStart >= segmentEnd)
					{
						return;
					}

					if (segmentEnd == throwIndex)
						continue;

					if (segmentStart < throwIndex && segmentEnd < throwIndex &&
						!new ArraySegment<Instruction>(body, segmentStart, segmentEnd - segmentStart)
						.Any(c => c.OpCode.Code is Code.Throw or Code.Ret))
					{
						continue;
					}

					if (segmentEnd < throwIndex)
					{
						start = segmentEnd;
						break;
					}

					if (segmentStart < throwIndex)
					{
						start = segmentStart;
						break;
					}
				}

				if (start == 0)
					continue;

				ArraySegment<Instruction> instructions = new(body, start, throwIndex - start + 1);
				MethodDefinition throwHelper = GenerateThrowHelper(method, instructions, throwIndex);
				method.DeclaringType.Methods.Add(throwHelper);
				RewriteCallsite(method.Body, instructions, throwHelper);
			}

			ListPool<(int, int)>.Return(starts);
			method.Body.Optimize();
		}

		private MethodDefinition GenerateThrowHelper(MethodDefinition method, ArraySegment<Instruction> instructions, int id)
		{
			MethodDefinition throwHelper = new($"ThrowGeneratedException{method.Name}{id}", MethodAttributes.Static | MethodAttributes.Private, method.ReturnType)
			{
				NoInlining = _configuration.NoInline,
				Body =
				{
					InitLocals = true
				}
			};
			ILProcessor processor = throwHelper.Body.GetILProcessor();
			int paramCounter = 0;

			static Instruction GetLdArg(ref int counter)
			{
				return counter++ switch
				{
					0 => Instruction.Create(OpCodes.Ldarg_0),
					1 => Instruction.Create(OpCodes.Ldarg_1),
					2 => Instruction.Create(OpCodes.Ldarg_2),
					3 => Instruction.Create(OpCodes.Ldarg_3),
					<= byte.MaxValue => Instruction.Create(OpCodes.Ldarg_S, (byte)counter),
					<= ushort.MaxValue => Instruction.Create(OpCodes.Ldarg, (ushort)counter),
					_ => throw new Exception("Seriously?")
				};
			}
			static int ToId(Instruction instruction)
			{
				return instruction.OpCode.Code switch
				{
					Code.Ldarg_0 or Code.Ldloc_0 => 0,
					Code.Ldarg_1 or Code.Ldloc_1 => 1,
					Code.Ldarg_2 or Code.Ldloc_2 => 2,
					Code.Ldarg_3 or Code.Ldloc_3 => 3,
					Code.Ldarg_S or Code.Ldloc_S or Code.Ldarga_S or Code.Ldloca_S => (byte)instruction.Operand,
					Code.Ldarg or Code.Ldloc or Code.Ldarga or Code.Ldloca => (ushort)instruction.Operand,
					_ => throw new Exception("Seriously?")
				};
			}
			foreach (Instruction instruction in instructions)
			{
				switch (instruction.OpCode.Code)
				{
					case Code.Ldarg or Code.Ldarg_0 or Code.Ldarg_1 or Code.Ldarg_2 or Code.Ldarg_3 or Code.Ldarg_S:
						throwHelper.Parameters.Add(new ParameterDefinition(method.Parameters[ToId(instruction)].ParameterType));
						processor.Append(GetLdArg(ref paramCounter));
						break;
					case Code.Ldloc or Code.Ldloc_0 or Code.Ldloc_1 or Code.Ldloc_2 or Code.Ldloc_3 or Code.Ldloc_S:
						throwHelper.Parameters.Add(new ParameterDefinition(method.Body.Variables[ToId(instruction)].VariableType));
						processor.Append(GetLdArg(ref paramCounter));
						break;
					case Code.Ldarga or Code.Ldarga_S:
						throwHelper.Parameters.Add(new ParameterDefinition(method.Parameters[ToId(instruction)].ParameterType.MakeByReferenceType()));
						processor.Append(GetLdArg(ref paramCounter));
						break;
					case Code.Ldloca or Code.Ldloca_S:
						throwHelper.Parameters.Add(new ParameterDefinition(method.Body.Variables[ToId(instruction)].VariableType.MakeByReferenceType()));
						processor.Append(GetLdArg(ref paramCounter));
						break;
					default:
						processor.Append(instruction);
						break;
				}
			}

			return throwHelper;
		}

		private void RewriteCallsite(MethodBody body, ArraySegment<Instruction> instructions, MethodDefinition method)
		{
			ILProcessor processor = body.GetILProcessor();
			Instruction[] instr = instructions.Reverse().ToArray();

			for (int i = 0; i < instr.Length; i++)
			{
				Instruction instruction = instr[i];
				if (i == 0)
				{
					processor.InsertAfter(instruction, Instruction.Create(OpCodes.Ret));
					processor.InsertAfter(instruction, Instruction.Create(OpCodes.Call, method));
				}

				switch (instruction.OpCode.Code)
				{
					case Code.Ldarg or Code.Ldarg_0 or Code.Ldarg_1 or Code.Ldarg_2 or Code.Ldarg_3 or Code.Ldarg_S:
						break;
					case Code.Ldloc or Code.Ldloc_0 or Code.Ldloc_1 or Code.Ldloc_2 or Code.Ldloc_3 or Code.Ldloc_S:
						break;
					case Code.Ldarga or Code.Ldarga_S:
						break;
					case Code.Ldloca or Code.Ldloca_S:
						break;
					default:
						processor.Remove(instruction);
						break;
				}
			}
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
