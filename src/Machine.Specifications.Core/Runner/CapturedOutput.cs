using System;

namespace Machine.Specifications.Runner
{
#if !NET6_0_OR_GREATER
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
