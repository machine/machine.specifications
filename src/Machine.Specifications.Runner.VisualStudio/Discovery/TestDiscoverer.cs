using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Machine.Specifications.Explorers;
using Machine.Specifications.Model;
using Machine.Specifications.Runner.VisualStudio.Helpers;
using Machine.Specifications.Runner.VisualStudio.Navigation;

namespace Machine.Specifications.Runner.VisualStudio.Discovery
{
    public class TestDiscoverer
#if NETFRAMEWORK
                                : MarshalByRefObject
#endif
    {
#if NETFRAMEWORK
        [System.Security.SecurityCritical]
        public override object InitializeLifetimeService()
        {
            return null;
        }
#endif
        private readonly PropertyInfo behaviorProperty = typeof(BehaviorSpecification).GetProperty("BehaviorFieldInfo");

        public IEnumerable<SpecTestCase> DiscoverTests(string assemblyPath)
        {
            var assemblyExplorer = new AssemblyExplorer();

            var assembly = AssemblyHelper.Load(assemblyPath);
            var contexts = assemblyExplorer.FindContextsIn(assembly);

            using (var session = new NavigationSession(assemblyPath))
            {
                return contexts.SelectMany(context => CreateTestCase(context, session)).ToList();
            }
        }

        private IEnumerable<SpecTestCase> CreateTestCase(Context context, NavigationSession session)
        {
            foreach (var spec in context.Specifications.ToList())
            {
                var testCase = new SpecTestCase
                {
                    ClassName = context.Type.Name,
                    ContextFullType = context.Type.FullName,
                    ContextDisplayName = GetContextDisplayName(context.Type),
                    SpecificationName = spec.FieldInfo.Name,
                    SpecificationDisplayName = spec.Name
                };

                string fieldDeclaringType;

                if (spec.FieldInfo.DeclaringType.GetTypeInfo().IsGenericType && !spec.FieldInfo.DeclaringType.GetTypeInfo().IsGenericTypeDefinition)
                    fieldDeclaringType = spec.FieldInfo.DeclaringType.GetGenericTypeDefinition().FullName;
                else
                    fieldDeclaringType = spec.FieldInfo.DeclaringType.FullName;

                var locationInfo = session.GetNavigationData(fieldDeclaringType, spec.FieldInfo.Name);

                if (locationInfo != null)
                {
                    testCase.CodeFilePath = locationInfo.CodeFile;
                    testCase.LineNumber = locationInfo.LineNumber;
                }

                if (spec is BehaviorSpecification behaviorSpec)
                    PopulateBehaviorField(testCase, behaviorSpec);

                if (context.Tags != null)
                    testCase.Tags = context.Tags.Select(tag => tag.Name).ToArray();

                if (context.Subject != null)
                    testCase.Subject = context.Subject.FullConcern;

                yield return testCase;
            }
        }

        private void PopulateBehaviorField(SpecTestCase testCase, BehaviorSpecification specification)
        {
            if (behaviorProperty?.GetValue(specification) is FieldInfo field)
            {
                testCase.BehaviorFieldName = field.Name;
                testCase.BehaviorFieldType = field.FieldType.GenericTypeArguments.FirstOrDefault()?.FullName;
            }
        }

        private string GetContextDisplayName(Type contextType)
        {
            var displayName = contextType.Name.Replace("_", " ");

            if (contextType.IsNested)
            {
                return GetContextDisplayName(contextType.DeclaringType) + " " + displayName;
            }

            return displayName;
        }
    }

}
