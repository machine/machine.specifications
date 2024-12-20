using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Text;

namespace Machine.Specifications.Runner.VisualStudio.Reflection
{
    public class InstructionData
    {
        public InstructionData(ILOpCode opCode, string opCodeName, OperandType operandType, int offset, InstructionData previous, string name = null)
        {
            OpCode = opCode;
            OpCodeName = opCodeName;
            OperandType = operandType;
            Offset = offset;
            Previous = previous;
            Name = name;
        }

        public ILOpCode OpCode { get; }

        public string OpCodeName { get; }

        public OperandType OperandType { get; }

        public string Name { get; }

        public int Offset { get; }

        public InstructionData Previous { get; }

        public override string ToString()
        {
            var value = new StringBuilder();

            AppendLabel(value);

            value.Append(": ");
            value.Append(OpCode);

            if (!string.IsNullOrEmpty(Name))
            {
                value.Append(" ");

                if (OperandType == OperandType.InlineString)
                {
                    value.Append("\"");
                    value.Append(Name);
                    value.Append("\"");
                }
                else
                {
                    value.Append(Name);
                }
            }

            return value.ToString();
        }

        private void AppendLabel(StringBuilder value)
        {
            value.Append("IL_");
            value.Append(Offset.ToString("x4"));
        }
    }
}
