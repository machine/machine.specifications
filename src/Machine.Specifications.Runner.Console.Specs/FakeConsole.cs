using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.ConsoleRunner.Specs
{
    public class FakeConsole : IConsole
    {
        readonly StringBuilder _console = new StringBuilder();

        public IEnumerable<string> Lines
        {
            get { return _console.ToString().Split(new[] {Environment.NewLine}, StringSplitOptions.None).ToList(); }
        }

        public string Output
        {
            get { return _console.ToString(); }
        }

        public void Write(string line)
        {
            _console.Append(line);
        }

        public void Write(string line, params object[] parameters)
        {
            _console.AppendFormat(line, parameters);
        }

        public void WriteLine(string line)
        {
            _console.AppendLine(line);
        }

        public void WriteLine(string line, params object[] parameters)
        {
            _console.AppendLine(string.Format(line, parameters));
        }
    }
}
