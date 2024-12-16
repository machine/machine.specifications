using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Machine.Specifications.Runner.VisualStudio.Reflection
{
    public class CodeReader
    {
        private static readonly OperandType[] OperandTypes = Enumerable.Repeat((OperandType) 0xff, 0x11f).ToArray();

        private static readonly string[] OperandNames = new string[0x11f];

        static CodeReader()
        {
            foreach (var field in typeof(OpCodes).GetFields())
            {
                var opCode = (OpCode) field.GetValue(null);
                var index = (ushort) (((opCode.Value & 0x200) >> 1) | opCode.Value & 0xff);

                OperandTypes[index] = opCode.OperandType;
                OperandNames[index] = opCode.Name;
            }
        }

        public IEnumerable<InstructionData> GetInstructions(MetadataReader reader, ref BlobReader blob)
        {
            var instructions = new List<InstructionData>();

            InstructionData previous = null;

            while (blob.RemainingBytes > 0)
            {
                var offset = blob.Offset;

                var opCode = ReadOpCode(ref blob);
                var opCodeName = GetDisplayName(opCode);
                var operandType = GetOperandType(opCode);

                var name = operandType != OperandType.InlineNone
                    ? ReadOperand(reader, ref blob, operandType)
                    : null;

                previous = new InstructionData(opCode, opCodeName, operandType, offset, previous, name);

                instructions.Add(previous);
            }

            return instructions;
        }

        private string ReadOperand(MetadataReader reader, ref BlobReader blob, OperandType operandType)
        {
            var name = string.Empty;

            switch (operandType)
            {
                case OperandType.InlineI8:
                case OperandType.InlineR:
                    blob.Offset += 8;
                    break;

                case OperandType.InlineBrTarget:
                case OperandType.InlineI:
                case OperandType.InlineSig:
                case OperandType.InlineString:
                case OperandType.InlineTok:
                case OperandType.InlineType:
                case OperandType.ShortInlineR:
                    blob.Offset += 4;
                    break;

                case OperandType.InlineField:
                case OperandType.InlineMethod:
                    var handle = MetadataTokens.EntityHandle(blob.ReadInt32());

                    name = LookupToken(reader, handle);
                    break;

                case OperandType.InlineSwitch:
                    var length = blob.ReadInt32();
                    blob.Offset += length * 4;
                    break;

                case OperandType.InlineVar:
                    blob.Offset += 2;
                    break;

                case OperandType.ShortInlineVar:
                case OperandType.ShortInlineBrTarget:
                case OperandType.ShortInlineI:
                    blob.Offset++;
                    break;
            }

            return name;
        }

        private string LookupToken(MetadataReader reader, EntityHandle handle)
        {
            if (handle.Kind == HandleKind.FieldDefinition)
            {
                var field = reader.GetFieldDefinition((FieldDefinitionHandle) handle);

                return reader.GetString(field.Name);
            }

            if (handle.Kind == HandleKind.MethodDefinition)
            {
                var method = reader.GetMethodDefinition((MethodDefinitionHandle) handle);

                return reader.GetString(method.Name);
            }

            return string.Empty;
        }

        private ILOpCode ReadOpCode(ref BlobReader blob)
        {
            var opCodeByte = blob.ReadByte();

            var value = opCodeByte == 0xfe
                ? 0xfe00 + blob.ReadByte()
                : opCodeByte;

            return (ILOpCode) value;
        }

        private OperandType GetOperandType(ILOpCode opCode)
        {
            var index = (ushort) ((((int) opCode & 0x200) >> 1) | ((int) opCode & 0xff));

            if (index >= OperandTypes.Length)
            {
                return (OperandType) 0xff;
            }

            return OperandTypes[index];
        }

        private string GetDisplayName(ILOpCode opCode)
        {
            var index = (ushort) ((((int) opCode & 0x200) >> 1) | ((int) opCode & 0xff));

            if (index >= OperandNames.Length)
            {
                return string.Empty;
            }

            return OperandNames[index];
        }
    }
}
