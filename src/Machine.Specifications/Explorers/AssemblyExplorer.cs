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

        public Context FindContexts(Type type)
        {
            return FindContexts(type, null);
        }

        public Context FindContexts(Type type, RunOptions options)
        {
            var filterExpression = CreateTypeFilterExpression(options);

            return filterExpression(type) && IsContext(type) ? CreateContextFrom(type) : null;
        }

        public Context FindContexts(FieldInfo info)
        {
            return FindContexts(info, null);
        }

        public Context FindContexts(FieldInfo info, RunOptions options)
        {
            var filterExpression = CreateTypeFilterExpression(options);

            Type type = info.DeclaringType;
            if (filterExpression(type) && IsContext(type))
            {
                return CreateContextFrom(type, info);
            }

            return null;
        }

        public IEnumerable<Context> FindContextsIn(Assembly assembly)
        {
            return FindContextsIn(assembly, options: null);
        }

        public IEnumerable<Context> FindContextsIn(Assembly assembly, RunOptions options)
        {
            return EnumerateContextsIn(assembly, options).Select(CreateContextFrom);
        }

        public IEnumerable<Context> FindContextsIn(Assembly assembly, string targetNamespace)
        {
            return FindContextsIn(assembly, targetNamespace, options: null);
        }

        public IEnumerable<Context> FindContextsIn(Assembly assembly, string targetNamespace, RunOptions options)
        {
            return EnumerateContextsIn(assembly, options)
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

        static IEnumerable<Type> EnumerateContextsIn(Assembly assembly, RunOptions options)
        {
            var typeFilterExpression = CreateTypeFilterExpression(options);

            return assembly
              .GetTypes()
              .Where(typeFilterExpression)
              .Where(IsContext)
              .OrderBy(t => t.Namespace);
        }

        static Func<Type, bool> CreateTypeFilterExpression(RunOptions options)
        {
            if (options == null)
            {
                return type => true;
            }

            var extractor = new AttributeTagExtractor();

            var restrictToTypes = options.Filters.ToArray();
            var includeTags = options.IncludeTags.Select(tag => new Tag(tag)).ToArray();
            var excludeTags = options.ExcludeTags.Select(tag => new Tag(tag)).ToArray();

            var typeFilterExpression = restrictToTypes.Any() ?
                (Func<Type, bool>) (x => restrictToTypes.Any(filter => StringComparer.OrdinalIgnoreCase.Equals(filter, x.FullName))) :
                _ => true;

            var includeTypeFilterExpression = includeTags.Any() ?
                (Func<IEnumerable<Tag>, bool>) (tags => tags.Intersect(includeTags).Any()) :
                _ => true;

            var excludeTypeFilterExpression = excludeTags.Any() ?
                (Func<IEnumerable<Tag>, bool>) (tags => !tags.Intersect(excludeTags).Any()) :
                _ => true;

            return type =>
            {
                var tags = extractor.ExtractTags(type).ToArray();
                return typeFilterExpression(type) && includeTypeFilterExpression(tags) && excludeTypeFilterExpression(tags);
            };
        }
    }
}
