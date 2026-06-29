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
            [TestCaseProperties.FullyQualifiedName.Label] = TestCaseProperties.FullyQualifiedName,
            [TestCaseProperties.DisplayName.Id] = TestCaseProperties.DisplayName,
            [TestCaseProperties.DisplayName.Label] = TestCaseProperties.DisplayName
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

            if (filterExpression == null)
            {
                return testCases;
            }

            var filteredTests = testCases
                .Where(x => filterExpression.MatchTestCase(x, propertyName => ResolvePropertyValue(propertyName, x)));

            return filteredTests;
        }

        private object ResolvePropertyValue(string propertyName, TestCase testCase)
        {
            // Read well-known properties directly from TestCase — the TestObject property
            // bag may not contain them reliably across ObjectModel assembly versions
            if (string.Equals(propertyName, "FullyQualifiedName", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(propertyName, "TestCase.FullyQualifiedName", StringComparison.OrdinalIgnoreCase))
            {
                return testCase.FullyQualifiedName;
            }

            if (string.Equals(propertyName, "DisplayName", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(propertyName, "TestCase.DisplayName", StringComparison.OrdinalIgnoreCase))
            {
                return testCase.DisplayName;
            }

            // Trait-based properties (Tag, Subject, ClassName)
            var traits = testCase.Traits?
                .Where(x => string.Equals(x.Name, propertyName, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Value)
                .ToArray();

            if (traits?.Length == 1)
            {
                return traits[0];
            }

            if (traits?.Length > 1)
            {
                return traits;
            }

            return null;
        }
    }
}
