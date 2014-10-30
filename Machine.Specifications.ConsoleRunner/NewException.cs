using System;
using System.Runtime.Serialization;
using Machine.Specifications.ConsoleRunner.Properties;

namespace Machine.Specifications.ConsoleRunner
{
    [Serializable]
    public class ConsoleRunnerException : Exception
    {
        public ConsoleRunnerException(string message)
            : base(message)
        {
        }

        private ConsoleRunnerException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected ConsoleRunnerException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context)
        {
        }
    }

    public static class NewException
    {
        public static Exception MissingAssembly(string assemblyName)
        {
            return new ConsoleRunnerException(String.Format(Resources.MissingAssemblyError, assemblyName));
        }
    }
}
