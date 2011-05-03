using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Machine.Specifications.Runner;

using NAnt.Core;

namespace Machine.Specifications.NAntTask
{
    class NAntConsole : IConsole
    {
        readonly Task _task;

        public NAntConsole(Task task)
        {
            _task = task;
        }

        public void Write(string line)
        {
            _task.Log(Level.Info, line);
        }

        public void Write(string line, params object[] parameters)
        {
            _task.Log(Level.Info, line, parameters);
        }

        public void WriteLine(string line)
        {
            _task.Log(Level.Info, line);
        }

        public void WriteLine(string line, params object[] parameters)
        {
            _task.Log(Level.Info, line, parameters);
        }
    }
}
