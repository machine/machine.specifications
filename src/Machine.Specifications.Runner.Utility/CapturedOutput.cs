using System;

namespace Machine.Specifications.Runner.Utility
{
    [Serializable]
    public class CapturedOutput
    {
        public CapturedOutput(string stdOut, string stdError, string debugTrace)
        {
            StdOut = stdOut;
            StdError = stdError;
            DebugTrace = debugTrace;
        }

        public string StdOut { get; }

        public string StdError { get; }

        public string DebugTrace { get; }
    }
}
