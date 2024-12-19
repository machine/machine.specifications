using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace Machine.Specifications.Runner.VisualStudio.Reflection
{
    public class TypeData
    {
        private readonly PEReader reader;

        private readonly MetadataReader metadata;

        private readonly SymbolReader symbolReader;

        private readonly TypeDefinition definition;

        private readonly object sync = new object();

        private ReadOnlyCollection<MethodData> methods;

        public TypeData(string typeName, PEReader reader, MetadataReader metadata, SymbolReader symbolReader, TypeDefinition definition)
        {
            this.reader = reader;
            this.metadata = metadata;
            this.symbolReader = symbolReader;
            this.definition = definition;

            TypeName = typeName;
        }

        public string TypeName { get; }

        public IReadOnlyCollection<MethodData> Constructors
        {
            get
            {
                if (methods != null)
                {
                    return methods;
                }

                lock (sync)
                {
                    methods = GetConstructors().AsReadOnly();
                }

                return methods;
            }
        }

        public override string ToString()
        {
            return TypeName;
        }

        private List<MethodData> GetConstructors()
        {
            var values = new List<MethodData>();

            foreach (var methodHandle in definition.GetMethods())
            {
                var methodDefinition = metadata.GetMethodDefinition(methodHandle);
                var parameters = methodDefinition.GetParameters();

                var methodName = metadata.GetString(methodDefinition.Name);

                if (IsConstructor(methodDefinition, methodName) && parameters.Count == 0)
                {
                    values.Add(new MethodData(methodName, reader, metadata, symbolReader, methodDefinition, methodHandle));
                }
            }

            return values;
        }

        private bool IsConstructor(MethodDefinition method, string name)
        {
            return method.Attributes.HasFlag(MethodAttributes.RTSpecialName) &&
                   method.Attributes.HasFlag(MethodAttributes.SpecialName) &&
                   name == ".ctor";
        }
    }
}
