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
            return compiledAssembly.References.Any(v => v.EndsWith("Katuusagi.FixedString.dll"));
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
                                    isChanged = OpEqualityProcess(method, instruction) || isChanged;
                                    isChanged = OpInequalityProcess(method, instruction) || isChanged;
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
                instruction.TryGetPushArgumentInstruction(0, out argInstruction);
            }
            else if (operandString == "System.Boolean Katuusagi.FixedString.FixedString16Bytes::op_Equality(Katuusagi.FixedString.FixedString16Bytes&,System.String)")
            {
                instruction.TryGetPushArgumentInstruction(1, out argInstruction);
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
                instruction.TryGetPushArgumentInstruction(0, out argInstruction);
            }
            else if (operandString == "System.Boolean Katuusagi.FixedString.FixedString16Bytes::op_Inequality(Katuusagi.FixedString.FixedString16Bytes&,System.String)")
            {
                instruction.TryGetPushArgumentInstruction(1, out argInstruction);
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
    }
}
