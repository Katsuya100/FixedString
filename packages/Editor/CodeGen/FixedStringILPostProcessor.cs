using Katuusagi.ILPostProcessorCommon.Editor;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity.CompilationPipeline.Common.ILPostProcessing;

namespace Katuusagi.FixedString.Editor
{
    internal class FixedStringILPostProcessor : ILPostProcessor
    {
        private ConstTableGenerator _constTable  = null;
        private MethodInfo EqualityMethod = typeof(FixedString16Bytes).GetMethod("op_Equality", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public, null, new Type[] { typeof(FixedString16Bytes).MakeByRefType(), typeof(FixedString16Bytes).MakeByRefType() }, null);
        private MethodInfo InequalityMethod = typeof(FixedString16Bytes).GetMethod("op_Inequality", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public, null, new Type[] { typeof(FixedString16Bytes).MakeByRefType(), typeof(FixedString16Bytes).MakeByRefType() }, null);
        private MethodReference EqualityMethodRef;
        private MethodReference InequalityMethodRef;

        public override ILPostProcessor GetInstance() => this;

        public override bool WillProcess(ICompiledAssembly compiledAssembly)
        {
            return true;
        }

        public override ILPostProcessResult Process(ICompiledAssembly compiledAssembly)
        {
            if (!WillProcess(compiledAssembly))
            {
                return null;
            }


            try
            {
                ILPPUtils.InitLog<FixedStringILPostProcessor>(compiledAssembly);
                using (var assembly = ILPPUtils.LoadAssemblyDefinition(compiledAssembly))
                {
                    EqualityMethodRef = assembly.MainModule.ImportReference(EqualityMethod);
                    InequalityMethodRef = assembly.MainModule.ImportReference(InequalityMethod);

                    using (_constTable = new ConstTableGenerator(assembly.MainModule, "Katuusagi.FixedString.Generated", "$$ConstTable"))
                    {
                        foreach (var type in assembly.Modules.SelectMany(v => v.Types).GetAllTypes())
                        {
                            if (!type.HasMethods)
                            {
                                continue;
                            }

                            var clonedMethods = type.Methods.ToArray();
                            foreach (var method in clonedMethods)
                            {
                                var body = method.Body;
                                if (body == null)
                                {
                                    continue;
                                }

                                bool isChanged = false;
                                var ilProcessor = body.GetILProcessor();
                                var instructions = body.Instructions;
                                for (var i = 0; i < instructions.Count; ++i)
                                {
                                    var instruction = instructions[i];
                                    int diff = 0;
                                    isChanged = OpImplicitProcess(ilProcessor, method, instruction, ref diff) || isChanged;
                                    OpEqualityProcess(method, instruction);
                                    OpInequalityProcess(method, instruction);
                                    i += diff;
                                }

                                if (!isChanged)
                                {
                                    continue;
                                }

                                ILPPUtils.ResolveInstructionOpCode(instructions);
                            }
                        }
                    }

                    var pe  = new MemoryStream();
                    var pdb = new MemoryStream();
                    var writeParameter = new WriterParameters()
                    {
                        SymbolWriterProvider = new PortablePdbWriterProvider(),
                        SymbolStream = pdb,
                        WriteSymbols = true,
                    };

                    assembly.Write(pe, writeParameter);
                    return new ILPostProcessResult(new InMemoryAssembly(pe.ToArray(), pdb.ToArray()), ILPPUtils.Logger.Messages);
                }
            }
            catch (Exception e)
            {
                ILPPUtils.LogException(e);
            }

            return new ILPostProcessResult(null, ILPPUtils.Logger.Messages);
        }

        private bool OpImplicitProcess(ILProcessor ilProcessor, MethodDefinition method, Instruction instruction, ref int instructionDiff)
        {
            if (instruction.OpCode != OpCodes.Call ||
                instruction.Operand.ToString() != "Katuusagi.FixedString.FixedString16Bytes Katuusagi.FixedString.FixedString16Bytes::op_Implicit(System.String)")
            {
                return false;
            }

            var argInstruction = instruction.Previous;
            if (!ILPPUtils.TryGetConstValue<string>(ref argInstruction, out var str))
            {
                return false;
            }

            if (Encoding.UTF8.GetByteCount(str) > FixedString16Bytes.Size)
            {
                ILPPUtils.LogError("FIXEDSTRING0001", "FixedString failed.", "Only strings of 16 bytes or less can be assigned to \"FixedString16Byte\".", method, instruction);
                return false;
            }

            var loadValue = _constTable.LoadValue(new FixedString16Bytes(str));
            instruction.OpCode = loadValue.OpCode;
            instruction.Operand = loadValue.Operand;

            // à¯êîÇçÌèú
            --instructionDiff;
            ILPPUtils.ReplaceTarget(ilProcessor, argInstruction, loadValue);
            argInstruction = argInstruction.Next;
            ilProcessor.Remove(argInstruction.Previous);
            return true;
        }

        private bool OpEqualityProcess(MethodDefinition method, Instruction instruction)
        {
            if (instruction.OpCode != OpCodes.Call)
            {
                return false;
            }

            Instruction argInstruction = null;
            var operandString = instruction.Operand.ToString();
            if (operandString == "System.Boolean Katuusagi.FixedString.FixedString16Bytes::op_Equality(System.String,Katuusagi.FixedString.FixedString16Bytes&)")
            {
                TryGetPushArgumentInstruction(instruction, 0, out argInstruction);
            }
            else if (operandString == "System.Boolean Katuusagi.FixedString.FixedString16Bytes::op_Equality(Katuusagi.FixedString.FixedString16Bytes&,System.String)")
            {
                TryGetPushArgumentInstruction(instruction, 1, out argInstruction);
            }

            if (argInstruction == null)
            {
                return false;
            }

            if (!ILPPUtils.TryGetConstValue<string>(ref argInstruction, out var str))
            {
                return false;
            }

            if (Encoding.UTF8.GetByteCount(str) > FixedString16Bytes.Size)
            {
                ILPPUtils.LogError("FIXEDSTRING0001", "FixedString failed.", "Only strings of 16 bytes or less can be assigned to \"FixedString16Byte\".", method, instruction);
                return false;
            }

            var field = _constTable.GetStructField(new FixedString16Bytes(str));
            argInstruction.OpCode = OpCodes.Ldsflda;
            argInstruction.Operand = field;

            instruction.OpCode = OpCodes.Call;
            instruction.Operand = EqualityMethodRef;
            return true;
        }

        private bool OpInequalityProcess(MethodDefinition method, Instruction instruction)
        {
            if (instruction.OpCode != OpCodes.Call)
            {
                return false;
            }

            Instruction argInstruction = null;
            var operandString = instruction.Operand.ToString();
            if (operandString == "System.Boolean Katuusagi.FixedString.FixedString16Bytes::op_Inequality(System.String,Katuusagi.FixedString.FixedString16Bytes&)")
            {
                TryGetPushArgumentInstruction(instruction, 0, out argInstruction);
            }
            else if (operandString == "System.Boolean Katuusagi.FixedString.FixedString16Bytes::op_Inequality(Katuusagi.FixedString.FixedString16Bytes&,System.String)")
            {
                TryGetPushArgumentInstruction(instruction, 1, out argInstruction);
            }

            if (argInstruction == null)
            {
                return false;
            }

            if (!ILPPUtils.TryGetConstValue<string>(ref argInstruction, out var str))
            {
                return false;
            }

            if (Encoding.UTF8.GetByteCount(str) > FixedString16Bytes.Size)
            {
                ILPPUtils.LogError("FIXEDSTRING0001", "FixedString failed.", "Only strings of 16 bytes or less can be assigned to \"FixedString16Byte\".", method, instruction);
                return false;
            }

            var field = _constTable.GetStructField(new FixedString16Bytes(str));
            argInstruction.OpCode = OpCodes.Ldsflda;
            argInstruction.Operand = field;

            instruction.OpCode = OpCodes.Call;
            instruction.Operand = InequalityMethodRef;
            return true;
        }

        private bool TryGetPushArgumentInstruction(Instruction call, int argNumber, out Instruction arg)
        {
            arg = null;
            if (call.OpCode != OpCodes.Call &&
                call.OpCode != OpCodes.Callvirt &&
                call.OpCode != OpCodes.Calli)
            {
                return false;
            }

            if (!(call.Operand is MethodReference method))
            {
                return false;
            }

            var parameterCount = method.Parameters.Count;
            var targetStackCount = parameterCount - argNumber;
            var stackCount = 0;
            var instruction = call.Previous;
            while (instruction != null)
            {
                var pushCount = GetPushCount(instruction.OpCode);
                stackCount += pushCount;
                if (stackCount >= targetStackCount)
                {
                    arg = instruction;
                    return true;
                }

                var popCount = GetPopCount(instruction.OpCode);
                if (popCount == -1)
                {
                    return false;
                }
                stackCount -= popCount;
                instruction = instruction.Previous;
            }

            return false;
        }

        private int GetPushCount(OpCode opCode)
        {
            switch (opCode.StackBehaviourPush)
            {
                case StackBehaviour.Push0:
                    return 0;
                case StackBehaviour.Push1:
                case StackBehaviour.Pushi:
                case StackBehaviour.Pushi8:
                case StackBehaviour.Pushr4:
                case StackBehaviour.Pushr8:
                case StackBehaviour.Pushref:
                case StackBehaviour.Varpush:
                    return 1;
                case StackBehaviour.Push1_push1:
                    return 2;
            }
            return 0;
        }

        private int GetPopCount(OpCode opCode)
        {
            switch (opCode.StackBehaviourPop)
            {
                case StackBehaviour.Pop0:
                    return 0;
                case StackBehaviour.Pop1:
                case StackBehaviour.Popi:
                case StackBehaviour.Popref:
                case StackBehaviour.Popref_pop1:
                case StackBehaviour.Varpop:
                    return 1;
                case StackBehaviour.Pop1_pop1:
                case StackBehaviour.Popi_pop1:
                case StackBehaviour.Popi_popi:
                case StackBehaviour.Popi_popi8:
                case StackBehaviour.Popi_popr4:
                case StackBehaviour.Popi_popr8:
                case StackBehaviour.Popref_popi:
                    return 2;
                case StackBehaviour.Popi_popi_popi:
                case StackBehaviour.Popref_popi_popi:
                case StackBehaviour.Popref_popi_popi8:
                case StackBehaviour.Popref_popi_popr4:
                case StackBehaviour.Popref_popi_popr8:
                case StackBehaviour.Popref_popi_popref:
                    return 3;
                case StackBehaviour.PopAll:
                    return -1;
            }

            return 0;
        }
    }
}
