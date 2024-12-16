using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;

namespace Machine.Specifications.Runner.VisualStudio.Reflection
{
    public class SymbolReader
    {
        private readonly MetadataReader reader;

        public SymbolReader(string assembly)
        {
            var symbols = Path.ChangeExtension(assembly, "pdb");

            if (File.Exists(symbols))
            {
                reader = MetadataReaderProvider
                    .FromPortablePdbStream(File.OpenRead(symbols))
                    .GetMetadataReader();
            }
        }

        public IEnumerable<SequencePointData> ReadSequencePoints(MethodDefinitionHandle method)
        {
            if (reader == null)
            {
                return Enumerable.Empty<SequencePointData>();
            }

            return reader
                .GetMethodDebugInformation(method)
                .GetSequencePoints()
                .Select(x =>
                {
                    var document = reader.GetDocument(x.Document);
                    var fileName = reader.GetString(document.Name);

                    return new SequencePointData(fileName, x.StartLine, x.EndLine, x.Offset, x.IsHidden);
                });
        }
    }
}
