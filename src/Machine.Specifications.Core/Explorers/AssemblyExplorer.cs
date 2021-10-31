using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Machine.Specifications.Factories;
using Machine.Specifications.Model;
using Machine.Specifications.Runner;
using Machine.Specifications.Sdk;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Explorers
{
    public class AssemblyExplorer
    {
        private readonly ContextFactory contextFactory;

        public AssemblyExplorer()
        {
            contextFactory = new ContextFactory();
        }

        public Context FindContexts(Type type, RunOptions options = null)
        {
            var types = new[] {type};

            return types
                .Where(IsContext)
                .FilterBy(options)
                .Select(CreateContextFrom)
                .FirstOrDefault();
        }

        public Context FindContexts(FieldInfo info, RunOptions options = null)
        {
            var types = new[] {info.DeclaringType};

            return types
                .Where(IsContext)
                .FilterBy(options)
                .Select(t => CreateContextFrom(t, info))
                .FirstOrDefault();
        }

        public IEnumerable<Context> FindContextsIn(Assembly assembly)
        {
            return FindContextsIn(assembly, options: null);
        }

        public IEnumerable<Context> FindContextsIn(Assembly assembly, RunOptions options)
        {
            return EnumerateContextsIn(assembly)
                .FilterBy(options)
                .OrderBy(t => t.Namespace)
                .Select(CreateContextFrom);
        }

        public IEnumerable<Context> FindContextsIn(Assembly assembly, string targetNamespace)
        {
            return FindContextsIn(assembly, targetNamespace, null);
        }

        public IEnumerable<Context> FindContextsIn(Assembly assembly, string targetNamespace, RunOptions options)
        {
            return EnumerateContextsIn(assembly)
                .Where(x => x.Namespace == targetNamespace)
                .FilterBy(options)
                .Select(CreateContextFrom);
        }

        public IEnumerable<ICleanupAfterEveryContextInAssembly> FindAssemblyWideContextCleanupsIn(Assembly assembly)
        {
            return assembly.GetExportedTypes()
                .Where(x => x.GetInterfaces().Contains(typeof(ICleanupAfterEveryContextInAssembly)))
                .Select(x => (ICleanupAfterEveryContextInAssembly) Activator.CreateInstance(x));
        }

        public IEnumerable<ISupplementSpecificationResults> FindSpecificationSupplementsIn(Assembly assembly)
        {
            return assembly.GetExportedTypes()
                .Where(x => x.GetInterfaces().Contains(typeof(ISupplementSpecificationResults)))
                .Select(x => (ISupplementSpecificationResults) Activator.CreateInstance(x));
        }

        public IEnumerable<IAssemblyContext> FindAssemblyContextsIn(Assembly assembly)
        {
            return assembly.GetExportedTypes()
                .Where(x => x.GetTypeInfo().IsClass && !x.GetTypeInfo().IsAbstract && x.GetInterfaces().Contains(typeof(IAssemblyContext)))
                .Select(x => (IAssemblyContext) Activator.CreateInstance(x));
        }

        private Context CreateContextFrom(Type type)
        {
            var instance = Activator.CreateInstance(type);

            return contextFactory.CreateContextFrom(instance);
        }

        private Context CreateContextFrom(Type type, FieldInfo fieldInfo)
        {
            var instance = Activator.CreateInstance(type);

            return contextFactory.CreateContextFrom(instance, fieldInfo);
        }

        private static bool IsContext(Type type)
        {
            return HasSpecificationMembers(type) &&
                   !type.GetTypeInfo().HasAttribute(new BehaviorAttributeFullName());
        }

        private static bool HasSpecificationMembers(Type type)
        {
            return !type.GetTypeInfo().IsAbstract &&
                   type.GetInstanceFieldsOfUsage(new AssertDelegateAttributeFullName(), new BehaviorDelegateAttributeFullName()).Any();
        }

        private static IEnumerable<Type> EnumerateContextsIn(Assembly assembly)
        {
            return assembly
                .GetTypes()
                .Where(IsContext);
        }
    }
}
