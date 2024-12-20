using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace Machine.Specifications.Runner.VisualStudio.Reflection
{
    public class MethodData
    {
        private readonly PEReader reader;

        private readonly MetadataReader metadata;

        private readonly SymbolReader symbolReader;

        private readonly MethodDefinition definition;

        private readonly MethodDefinitionHandle handle;

        private readonly object sync = new object();

        private ReadOnlyCollection<InstructionData> instructions;

        private List<SequencePointData> sequencePoints;

        public MethodData(string name, PEReader reader, MetadataReader metadata, SymbolReader symbolReader, MethodDefinition definition, MethodDefinitionHandle handle)
        {
            this.reader = reader;
            this.metadata = metadata;
            this.symbolReader = symbolReader;
            this.definition = definition;
            this.handle = handle;

            Name = name;
        }

        public string Name { get; }

        public IReadOnlyCollection<InstructionData> Instructions
        {
            get
            {
                if (instructions != null)
                {
                    return instructions;
                }

                lock (sync)
                {
                    instructions = GetInstructions().AsReadOnly();
                }

                return instructions;
            }
        }

        public SequencePointData GetSequencePoint(InstructionData instruction)
        {
            if (sequencePoints == null)
            {
                lock (sync)
                {
                    sequencePoints = GetSequencePoints().ToList();
                }
            }

            return sequencePoints.FirstOrDefault(x => x.Offset == instruction.Offset);
        }

        public override string ToString()
        {
            return Name;
        }

        private List<InstructionData> GetInstructions()
        {
            var blob = reader
                .GetMethodBody(definition.RelativeVirtualAddress)
                .GetILReader();

            var codeReader = new CodeReader();

            return codeReader.GetInstructions(metadata, ref blob).ToList();
        }

        private IEnumerable<SequencePointData> GetSequencePoints()
        {
            return symbolReader.ReadSequencePoints(handle);
        }
    }
}
