using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace Machine.Specifications.Runner.VisualStudio.Reflection
{
    public class AssemblyData : IDisposable
    {
        private readonly PEReader reader;

        private readonly MetadataReader metadata;

        private readonly SymbolReader symbolReader;

        private readonly object sync = new object();

        private ReadOnlyCollection<TypeData> types;

        private AssemblyData(string assembly)
        {
            reader = new PEReader(File.OpenRead(assembly));
            metadata = reader.GetMetadataReader();
            symbolReader = new SymbolReader(assembly);
        }

        public static AssemblyData Read(string assembly)
        {
            return new AssemblyData(assembly);
        }

        public IReadOnlyCollection<TypeData> Types
        {
            get
            {
                if (types != null)
                {
                    return types;
                }

                lock (sync)
                {
                    types = ReadTypes().AsReadOnly();
                }

                return types;
            }
        }

        public void Dispose()
        {
            reader.Dispose();
        }

        private List<TypeData> ReadTypes()
        {
            var values = new List<TypeData>();

            foreach (var typeHandle in metadata.TypeDefinitions)
            {
                ReadType(values, typeHandle);
            }

            return values;
        }

        private void ReadType(List<TypeData> values, TypeDefinitionHandle typeHandle, string namespaceName = null)
        {
            var typeDefinition = metadata.GetTypeDefinition(typeHandle);

            var typeNamespace = string.IsNullOrEmpty(namespaceName)
                ? metadata.GetString(typeDefinition.Namespace)
                : namespaceName;

            var typeName = string.IsNullOrEmpty(namespaceName)
                ? $"{typeNamespace}.{metadata.GetString(typeDefinition.Name)}"
                : $"{typeNamespace}+{metadata.GetString(typeDefinition.Name)}";

            values.Add(new TypeData(typeName, reader, metadata, symbolReader, typeDefinition));

            foreach (var nestedTypeHandle in typeDefinition.GetNestedTypes())
            {
                ReadType(values, nestedTypeHandle, typeName);
            }
        }
    }
}
