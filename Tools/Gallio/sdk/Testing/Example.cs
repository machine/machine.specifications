// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Common.Reflection;
using Gallio.Framework;
using Gallio.Framework.Data;
using Gallio.Framework.Pattern;
using MbUnit.Framework;

namespace Gallio.Testing.Acceptance
{
    // The following example shows how to create custom row-like and column-like data source attributes.
    // The test fixture below also shows how to use [RunSample] and BaseTestWithSampleRunner to efficiently test the custom data source attributes.

    // A custom row-like data source that provides N rows of pairs {int, string}.
    [AttributeUsage(PatternAttributeTargets.DataContext, AllowMultiple = false, Inherited = true)]
    public class FooRowAttribute : DataAttribute
    {
        private readonly int count;

        public FooRowAttribute(int count)
        {
            this.count = count;
        }

        protected override void PopulateDataSource(IPatternScope scope, DataSource dataSource, ICodeElementInfo codeElement)
        {
            for (int i = 1; i <= count; i++)
            {
                var row = new object[] { i, "Hello from #" + i };
                dataSource.AddDataSet(new ItemSequenceDataSet(new IDataItem[] { new ListDataItem<object>(row, GetMetadata(), false) }, row.Length));
            }
        }
    }

    // A custom column-like data source that provides a column of letters ('A', 'B', 'C', etc.)
    [AttributeUsage(PatternAttributeTargets.DataContext, AllowMultiple = false, Inherited = true)]
    public class FooColumnAttribute : DataAttribute
    {
        private readonly int count;

        public FooColumnAttribute(int count)
        {
            this.count = count;
        }

        protected override void PopulateDataSource(IPatternScope scope, DataSource dataSource, ICodeElementInfo codeElement)
        {
            object[] values = Enumerable.Range(1, count).Select<int, object>(i => Convert.ToChar((Convert.ToInt32('A') + i - 1)).ToString()).ToArray();
            dataSource.AddDataSet(new ValueSequenceDataSet(values, GetMetadata(), false));
        }
    }

    [TestFixture]
    [RunSample(typeof(Sample))]
    public class RunSampleAttributeTest : BaseTestWithSampleRunner
    {
        [Test]
        public void FooAttribute()
        {
            // Runs the explicit sample fixture through the sample test runner,
            // Retrieves the resulting test step runs and their log stream,
            // and verifies that the log streams have the expected contents.
            var runs = GetTestStepRuns(typeof(Sample), "Test");
            Assert.AreElementsEqualIgnoringOrder(new[] { // Rows and columns are never guaranteed to run in a specific order!
                String.Empty, // = Parent node
                "1: 'Hello from #1', 'A'",
                "2: 'Hello from #2', 'A'",
                "3: 'Hello from #3', 'A'",
                "1: 'Hello from #1', 'B'",
                "2: 'Hello from #2', 'B'",
                "3: 'Hello from #3', 'B'",
                "1: 'Hello from #1', 'C'",
                "2: 'Hello from #2', 'C'",
                "3: 'Hello from #3', 'C'",
                "1: 'Hello from #1', 'D'",
                "2: 'Hello from #2', 'D'",
                "3: 'Hello from #3', 'D'",
            }, runs.Select(GetLog));
        }

        [TestFixture, Explicit]
        public class Sample
        {
            [Test, FooRow(3)] // Using the custom data source [FooRow] and [FooColumn] to generate rows and columns of test parameters.
            public void Test(int i, string text, [FooColumn(4)] string letter)
            {
                // Prints the values provided by our custom data sources into the test log, so we can assert them later.
                TestLog.Write("{0}: '{1}', '{2}'", i, text, letter);
            }
        }
    }
}
