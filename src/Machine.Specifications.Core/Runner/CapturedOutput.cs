using System;

namespace Machine.Specifications.Runner
{
#if !NETSTANDARD
    [Serializable]
#endif
    public class CapturedOutput
    {
        public CapturedOutput(string stdOut, string stdError, string debugTrace)
        {
            StdOut = stdOut;
            StdError = stdError;
            DebugTrace = debugTrace;
        }

        public string StdOut { get; set; }

        public string StdError { get; set; }

        public string DebugTrace { get; set; }
    }
}
