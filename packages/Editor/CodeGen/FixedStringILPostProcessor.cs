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

            ILPostProcessorUtils.Logger = new Logger();

            try
            {
                using (var assembly = ILPostProcessorUtils.LoadAssemblyDefinition(compiledAssembly))
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
                                    isChanged = OpImplicitProcess(ilProcessor, method, ref instruction, out var diff) || isChanged;
                                    isChanged = OpEqualityProcess(ilProcessor, method, ref instruction, out diff) || isChanged;
                                    isChanged = OpInequalityProcess(ilProcessor, method, ref instruction, out diff) || isChanged;
                                    i += diff;
                                }

                                if (!isChanged)
                                {
                                    continue;
                                }

                                ILPostProcessorUtils.ResolveInstructionOpCode(instructions);
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
                    return new ILPostProcessResult(new InMemoryAssembly(pe.ToArray(), pdb.ToArray()), ILPostProcessorUtils.Logger.Messages);
                }
            }
            catch (Exception e)
            {
                ILPostProcessorUtils.LogException(e);
            }

            return new ILPostProcessResult(null, ILPostProcessorUtils.Logger.Messages);
        }

        private bool OpImplicitProcess(ILProcessor ilProcessor, MethodDefinition method, ref Instruction instruction, out int instructionDiff)
        {
            instructionDiff = 0;

            if (instruction.OpCode != OpCodes.Call ||
                instruction.Operand.ToString() != "Katuusagi.FixedString.FixedString16Bytes Katuusagi.FixedString.FixedString16Bytes::op_Implicit(System.String)")
            {
                return false;
            }

            var argInstruction = instruction.Previous;
            if (!ILPostProcessorUtils.TryEmulateLiteral<string>(ref argInstruction, out var str))
            {
                return false;
            }

            if (Encoding.UTF8.GetByteCount(str) > FixedString16Bytes.Size)
            {
                ILPostProcessorUtils.LogError("Only strings of 16 bytes or less can be assigned to \"FixedString16Byte\".", method, instruction);
                return false;
            }

            var loadValue = _constTable.LoadValue(new FixedString16Bytes(str));
            ++instructionDiff;
            ilProcessor.InsertAfter(instruction, loadValue);

            while (argInstruction != instruction)
            {
                --instructionDiff;
                ILPostProcessorUtils.ReplaceTarget(ilProcessor, argInstruction, loadValue);
                argInstruction = argInstruction.Next;
                ilProcessor.Remove(argInstruction.Previous);
            }

            --instructionDiff;
            ILPostProcessorUtils.ReplaceTarget(ilProcessor, instruction, loadValue);
            ilProcessor.Remove(instruction);

            instruction = loadValue;
            return true;
        }

        private bool OpEqualityProcess(ILProcessor ilProcessor, MethodDefinition method, ref Instruction instruction, out int instructionDiff)
        {
            instructionDiff = 0;

            if (instruction.OpCode != OpCodes.Call ||
                instruction.Operand.ToString() != "System.Boolean Katuusagi.FixedString.FixedString16Bytes::op_Equality(Katuusagi.FixedString.FixedString16Bytes&,System.String)")
            {
                return false;
            }

            var argInstruction = instruction.Previous;
            if (!ILPostProcessorUtils.TryEmulateLiteral<string>(ref argInstruction, out var str))
            {
                return false;
            }

            if (Encoding.UTF8.GetByteCount(str) > FixedString16Bytes.Size)
            {
                ILPostProcessorUtils.LogError("Only strings of 16 bytes or less can be assigned to \"FixedString16Byte\".", method, instruction);
                return false;
            }

            var field = _constTable.GetStructField(new FixedString16Bytes(str));
            var loadValue = Instruction.Create(OpCodes.Ldsflda, field);

            ++instructionDiff;
            ilProcessor.InsertAfter(argInstruction, loadValue);

            ++instructionDiff;
            ilProcessor.InsertAfter(instruction, Instruction.Create(OpCodes.Call, EqualityMethodRef));

            --instructionDiff;
            ILPostProcessorUtils.ReplaceTarget(ilProcessor, argInstruction, loadValue);
            ilProcessor.Remove(argInstruction);

            --instructionDiff;
            ILPostProcessorUtils.ReplaceTarget(ilProcessor, instruction, loadValue);
            ilProcessor.Remove(instruction);

            instruction = loadValue;
            return true;
        }

        private bool OpInequalityProcess(ILProcessor ilProcessor, MethodDefinition method, ref Instruction instruction, out int instructionDiff)
        {
            instructionDiff = 0;

            if (instruction.OpCode != OpCodes.Call ||
                instruction.Operand.ToString() != "System.Boolean Katuusagi.FixedString.FixedString16Bytes::op_Inequality(Katuusagi.FixedString.FixedString16Bytes&,System.String)")
            {
                return false;
            }

            var argInstruction = instruction.Previous;
            if (!ILPostProcessorUtils.TryEmulateLiteral<string>(ref argInstruction, out var str))
            {
                return false;
            }

            if (Encoding.UTF8.GetByteCount(str) > FixedString16Bytes.Size)
            {
                ILPostProcessorUtils.LogError("Only strings of 16 bytes or less can be assigned to \"FixedString16Byte\".", method, instruction);
                return false;
            }

            var field = _constTable.GetStructField(new FixedString16Bytes(str));
            var loadValue = Instruction.Create(OpCodes.Ldsflda, field);

            ++instructionDiff;
            ilProcessor.InsertAfter(argInstruction, loadValue);

            ++instructionDiff;
            ilProcessor.InsertAfter(instruction, Instruction.Create(OpCodes.Call, InequalityMethodRef));

            --instructionDiff;
            ILPostProcessorUtils.ReplaceTarget(ilProcessor, argInstruction, loadValue);
            ilProcessor.Remove(argInstruction);

            --instructionDiff;
            ILPostProcessorUtils.ReplaceTarget(ilProcessor, instruction, loadValue);
            ilProcessor.Remove(instruction);

            instruction = loadValue;
            return true;
        }
    }
}
