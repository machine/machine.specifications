// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Globalization;
using System.Text;

namespace Machine.Specifications.Reporting.Integration
{

    /// <summary>
    /// Writes specially formatted service messages for TeamCity.
    /// These messages are interpreted by TeamCity to perform some task.
    /// See also: http://www.jetbrains.net/confluence/display/TCD3/Build+Script+Interaction+with+TeamCity
    /// </summary>
    public class TeamCityServiceMessageWriter
    {
        private readonly Action<string> writer;

        public TeamCityServiceMessageWriter(Action<string> writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            this.writer = writer;
        }

        public void WriteProgressMessage(string message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            WriteMessage(builder =>
            {
                builder.Append("progressMessage '");
                AppendEscapedString(builder, message);
                builder.Append('\'');
            });
        }

        public void WriteProgressStart(string message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            WriteMessage(builder =>
            {
                builder.Append("progressStart '");
                AppendEscapedString(builder, message);
                builder.Append('\'');
            });
        }

        public void WriteProgressFinish(string message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            WriteMessage(builder =>
            {
                builder.Append("progressFinish '");
                AppendEscapedString(builder, message);
                builder.Append('\'');
            });
        }

        public void WriteTestSuiteStarted(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            WriteMessage(builder =>
            {
                builder.Append("testSuiteStarted name='");
                AppendEscapedString(builder, name);
                builder.Append('\'');
            });
        }

        public void WriteTestSuiteFinished(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            WriteMessage(builder =>
            {
                builder.Append("testSuiteFinished name='");
                AppendEscapedString(builder, name);
                builder.Append('\'');
            });
        }

        public void WriteTestStarted(string name, bool captureStandardOutput)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            WriteMessage(builder =>
            {
                builder.Append("testStarted name='");
                AppendEscapedString(builder, name);
                builder.Append("' captureStandardOutput='");
                AppendEscapedString(builder, captureStandardOutput ? @"true" : @"false");
                builder.Append('\'');
            });
        }

        public void WriteTestFinished(string name, TimeSpan duration)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            WriteMessage(builder =>
            {
                builder.Append("testFinished name='");
                AppendEscapedString(builder, name);
                builder.Append("' duration='");
                builder.Append(((int)duration.TotalMilliseconds).ToString(CultureInfo.InvariantCulture));
                builder.Append('\'');
            });
        }

        public void WriteTestIgnored(string name, string message)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (message == null)
                throw new ArgumentNullException("message");

            WriteMessage(builder =>
            {
                builder.Append("testIgnored name='");
                AppendEscapedString(builder, name);
                builder.Append("' message='");
                AppendEscapedString(builder, message);
                builder.Append('\'');
            });
        }

        public void WriteTestStdOut(string name, string text)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (text == null)
                throw new ArgumentNullException("text");

            WriteMessage(builder =>
            {
                builder.Append("testStdOut name='");
                AppendEscapedString(builder, name);
                builder.Append("' out='");
                AppendEscapedString(builder, text);
                builder.Append('\'');
            });
        }

        public void WriteTestStdErr(string name, string text)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (text == null)
                throw new ArgumentNullException("text");

            WriteMessage(builder =>
            {
                builder.Append("testStdErr name='");
                AppendEscapedString(builder, name);
                builder.Append("' out='");
                AppendEscapedString(builder, text);
                builder.Append('\'');
            });
        }

        public void WriteTestFailed(string name, string message, string details)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (message == null)
                throw new ArgumentNullException("message");
            if (details == null)
                throw new ArgumentNullException("details");

            WriteMessage(builder =>
            {
                builder.Append("testFailed name='");
                AppendEscapedString(builder, name);
                builder.Append("' message='");
                AppendEscapedString(builder, message);
                builder.Append("' details='");
                AppendEscapedString(builder, details);
                builder.Append('\'');
            });
        }

        public void WriteTestFailedWithComparisonFailure(string name, string message, string details, string expected, string actual)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (message == null)
                throw new ArgumentNullException("message");
            if (details == null)
                throw new ArgumentNullException("details");
            if (expected == null)
                throw new ArgumentNullException("expected");
            if (actual == null)
                throw new ArgumentNullException("actual");

            WriteMessage(builder =>
            {
                builder.Append("testFailed name='");
                AppendEscapedString(builder, name);
                builder.Append("' type='comparisonFailure' message='");
                AppendEscapedString(builder, message);
                builder.Append("' details='");
                AppendEscapedString(builder, details);
                builder.Append("' expected='");
                AppendEscapedString(builder, expected);
                builder.Append("' actual='");
                AppendEscapedString(builder, actual);
                builder.Append('\'');
            });
        }

        public void WriteError(string message, string details)
        {
          WriteMessage(builder =>
          {
            builder.Append("message test='");
            AppendEscapedString(builder, message);
            builder.Append("' errorDetails='");
            AppendEscapedString(builder, details);
            builder.Append("' status='ERROR'");
          });
        }

        private void WriteMessage(Action<StringBuilder> formatter)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("##teamcity[");
            formatter(builder);
            builder.Append(']');
            writer(builder.ToString());
        }

        private static void AppendEscapedString(StringBuilder builder, string rawString)
        {
            foreach (char c in rawString)
            {
                if (c == '\n')
                    builder.Append("|n");
                else if (c == '\'')
                    builder.Append("|'");
                else if (c == '\r')
                    builder.Append("|r");
                else if (c == '|')
                    builder.Append("||");
                else if (c == ']')
                    builder.Append("|]");
                else
                    builder.Append(c);
            }
        }
    }
}
