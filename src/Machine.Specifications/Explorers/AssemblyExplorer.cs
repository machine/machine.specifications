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
        readonly ContextFactory _contextFactory;

        public AssemblyExplorer()
        {
            _contextFactory = new ContextFactory();
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
            return FindContextsIn(assembly, targetNamespace, options: null);
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
                .Where(IsContext);
        }
    }

    public static class FilteringExtensions
    {
        public static IEnumerable<Type> FilterBy(this IEnumerable<Type> types, RunOptions options)
        {
            if (options == null)
            {
                return types;
            }

            var filteredTypes = types;

            var restrictToTypes = new HashSet<string>(options.Filters, StringComparer.OrdinalIgnoreCase);

            if (restrictToTypes.Any())
            {
                filteredTypes = filteredTypes.Where(x => restrictToTypes.Contains(x.FullName));
            }

            var includeTags = new HashSet<Tag>(options.IncludeTags.Select(tag => new Tag(tag)));
            var excludeTags = new HashSet<Tag>(options.ExcludeTags.Select(tag => new Tag(tag)));

            if (includeTags.Any() || excludeTags.Any())
            {
                var extractor = new AttributeTagExtractor();

                var filteredTypesWithTags = filteredTypes.Select(type => (Type: type, Tags: extractor.ExtractTags(type)));

                if (includeTags.Any() )
                {
                    filteredTypesWithTags = filteredTypesWithTags.Where(x => x.Tags.Intersect(includeTags).Any());
                }

                if (excludeTags.Any() )
                {
                    filteredTypesWithTags = filteredTypesWithTags.Where(x => !x.Tags.Intersect(excludeTags).Any());
                }

                filteredTypes = filteredTypesWithTags.Select(x => x.Type);
            }

            return filteredTypes;
        }
    }
}
