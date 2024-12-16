using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications.Model;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Machine.Specifications.Runner.VisualStudio.Execution
{
    public class SpecificationFilterProvider : ISpecificationFilterProvider
    {
        private static readonly TestProperty TagProperty =
            TestProperty.Register(nameof(Tag), nameof(Tag), typeof(string), typeof(TestCase));

        private static readonly TestProperty SubjectProperty =
            TestProperty.Register(nameof(Subject), nameof(Subject), typeof(string), typeof(TestCase));

        private readonly Dictionary<string, TestProperty> testCaseProperties = new Dictionary<string, TestProperty>(StringComparer.OrdinalIgnoreCase)
        {
            [TestCaseProperties.FullyQualifiedName.Id] = TestCaseProperties.FullyQualifiedName,
            [TestCaseProperties.DisplayName.Id] = TestCaseProperties.DisplayName
        };

        private readonly Dictionary<string, TestProperty> traitProperties = new Dictionary<string, TestProperty>(StringComparer.OrdinalIgnoreCase)
        {
            [TagProperty.Id] = TagProperty,
            [SubjectProperty.Id] = SubjectProperty
        };

        private readonly string[] supportedProperties;

        public SpecificationFilterProvider()
        {
            supportedProperties = testCaseProperties.Keys
                .Concat(traitProperties.Keys)
                .ToArray();
        }

        public IEnumerable<TestCase> FilteredTests(IEnumerable<TestCase> testCases, IRunContext runContext, IFrameworkHandle handle)
        {
            var filterExpression = runContext.GetTestCaseFilter(supportedProperties, propertyName =>
            {
                if (testCaseProperties.TryGetValue(propertyName, out var testProperty))
                {
                    return testProperty;
                }
                if (traitProperties.TryGetValue(propertyName, out var traitProperty))
                {
                    return traitProperty;
                }
                return null;
            });

            handle?.SendMessage(TestMessageLevel.Informational, $"Machine Specifications Visual Studio Test Adapter - Filter property set '{filterExpression?.TestCaseFilterValue}'");

            if (filterExpression == null)
            {
                return testCases;
            }

            var filteredTests = testCases
                .Where(x => filterExpression.MatchTestCase(x, propertyName => GetPropertyValue(propertyName, x)));

            return filteredTests;
        }

        private object GetPropertyValue(string propertyName, TestObject testCase)
        {
            if (testCaseProperties.TryGetValue(propertyName, out var testProperty))
            {
                if (testCase.Properties.Contains(testProperty))
                {
                    return testCase.GetPropertyValue(testProperty);
                }
            }

            if (traitProperties.TryGetValue(propertyName, out var traitProperty))
            {
                var val = TraitContains(testCase, traitProperty.Id);

                if (val.Length == 1)
                {
                    return val[0];
                }

                if (val.Length > 1)
                {
                    return val;
                }
            }

            return null;
        }

        private static string[] TraitContains(TestObject testCase, string traitName)
        {
            return testCase?.Traits?
                .Where(x => x.Name == traitName)
                .Select(x => x.Value)
                .ToArray();
        }
    }
}
