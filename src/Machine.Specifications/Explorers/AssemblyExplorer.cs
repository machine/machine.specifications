using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Machine.Specifications.Factories;
using Machine.Specifications.Model;
using Machine.Specifications.Sdk;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Explorers
{
    public class AssemblyExplorer
    {
        readonly ContextFactory _contextFactory;

        public AssemblyExplorer()
        {
            _contextFactory = new ContextFactory();
        }

        public IEnumerable<Context> FindContextsIn(Assembly assembly)
        {
            return EnumerateContextsIn(assembly).Select(CreateContextFrom);
        }

        public IEnumerable<Context> FindContextsIn(Assembly assembly, string targetNamespace)
        {
            return EnumerateContextsIn(assembly)
              .Where(x => x.Namespace == targetNamespace)
              .Select(CreateContextFrom);
        }

        public IEnumerable<ICleanupAfterEveryContextInAssembly> FindAssemblyWideContextCleanupsIn(Assembly assembly)
        {
            return assembly.GetExportedTypes()
              .Where(x => x.GetInterfaces().Contains(typeof(ICleanupAfterEveryContextInAssembly)))
              .Select(x => (ICleanupAfterEveryContextInAssembly)Activator.CreateInstance(x));
        }

        public IEnumerable<ISupplementSpecificationResults> FindSpecificationSupplementsIn(Assembly assembly)
        {
            return assembly.GetExportedTypes()
              .Where(x => x.GetInterfaces().Contains(typeof(ISupplementSpecificationResults)))
              .Select(x => (ISupplementSpecificationResults)Activator.CreateInstance(x));
        }

        public IEnumerable<IAssemblyContext> FindAssemblyContextsIn(Assembly assembly)
        {
            return assembly.GetExportedTypes()
              .Where(x => x.GetTypeInfo().IsClass && !x.GetTypeInfo().IsAbstract && x.GetInterfaces().Contains(typeof(IAssemblyContext)))
              .Select(x => (IAssemblyContext)Activator.CreateInstance(x));
        }

        Context CreateContextFrom(Type type)
        {
            object instance = Activator.CreateInstance(type);
            return _contextFactory.CreateContextFrom(instance);
        }

        Context CreateContextFrom(Type type, FieldInfo fieldInfo)
        {
            object instance = Activator.CreateInstance(type);
            return _contextFactory.CreateContextFrom(instance, fieldInfo);
        }

        static bool IsContext(Type type)
        {
            return HasSpecificationMembers(type) && !type.GetTypeInfo().HasAttribute(new BehaviorAttributeFullName());
        }

        static bool HasSpecificationMembers(Type type)
        {
            return !type.GetTypeInfo().IsAbstract && type.GetInstanceFieldsOfUsage(new AssertDelegateAttributeFullName(), new BehaviorDelegateAttributeFullName()).Any();
        }

        static IEnumerable<Type> EnumerateContextsIn(Assembly assembly)
        {
            return assembly
              .GetTypes()
              .Where(IsContext)
              .OrderBy(t => t.Namespace);
        }

        public Context FindContexts(Type type)
        {
            if (IsContext(type))
            {
                return CreateContextFrom(type);
            }

            return null;
        }

        public Context FindContexts(FieldInfo info)
        {
            Type type = info.DeclaringType;
            if (IsContext(type))
            {
                return CreateContextFrom(type, info);
            }

            return null;
        }
    }
}
