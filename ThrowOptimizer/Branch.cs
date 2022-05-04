using System;
using System.Collections.Generic;
using System.Text;
using Mono.Cecil.Cil;
using ThrowOptimizer.Utils;

namespace ThrowOptimizer
{
	internal sealed class Branch
	{
		public readonly ArraySegment<Instruction> CommonStart;
		public readonly ArraySegment<Instruction> CommonEnd;
		public readonly ArraySegment<Instruction>[] Paths;

		public Branch(Instruction[] instructions, int start)
		{
			/*for (int i = start; i < instructions.Length; i++)
			{
				Instruction instruction = instructions[i];

				switch (instruction.OpCode.Code)
				{
					case Code.Brfalse or Code.Brfalse_S or
						Code.Brtrue or Code.Brtrue_S or
						Code.Beq or Code.Beq_S or
						Code.Bge or Code.Bge_S or Code.Bge_Un or Code.Bge_Un_S or
						Code.Bgt or Code.Bgt_S or Code.Bgt_Un or Code.Bgt_Un_S or
						Code.Ble or Code.Ble_S or Code.Ble_Un or Code.Ble_Un or
						Code.Blt or Code.Blt_S or Code.Blt_Un or Code.Blt_Un_S or
						Code.Bne_Un or Code.Bne_Un_S:
						int end = ((Instruction)instruction.Operand).Offset - instruction.Offset;
						Common = instructions.Slice(0, i);
						Paths = new[] { instructions.Slice(i, end - i), instructions.Slice(end) };
						return;
					case Code.Br or Code.Br_S:
						int end = ((Instruction)instruction.Operand).Offset - instruction.Offset;
						Common = instructions.Slice(0, i);
						Paths = new[] { instructions.Slice(i, end - i), instructions.Slice(end) };
						return;
					case Code.Switch:
						Instruction[] targets 
						return;
				}
			}
			Common = instructions;
			Paths = null;*/
		}
	}
}
