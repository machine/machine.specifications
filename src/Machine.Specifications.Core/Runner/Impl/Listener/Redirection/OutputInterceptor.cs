using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Machine.Specifications.Runner.Impl.Listener.Redirection
{
    internal class OutputInterceptor : IDisposable
    {
        private readonly TextWriter combinedOutput = new SafeStringWriter();

        private TextWriter oldError;

        private TraceListener[] oldListeners;

        private TextWriter oldOut;

        public OutputInterceptor()
        {
            CaptureStandardOut();
            CaptureStandardError();
            CaptureDebugTrace();
        }

        public string CapturedOutput => combinedOutput.ToString();

        private void CaptureStandardOut()
        {
            oldOut = Console.Out;

            var stdOut = new ForwardingStringWriter(new[] { oldOut, combinedOutput });

            Console.SetOut(stdOut);
        }

        private void CaptureStandardError()
        {
            oldError = Console.Error;

            var stdError = new ForwardingStringWriter(new[] { oldError, combinedOutput });

            Console.SetError(stdError);
        }

        private void CaptureDebugTrace()
        {
            oldListeners = new TraceListener[Trace.Listeners.Count];

            Trace.Listeners.CopyTo(oldListeners, 0);

            var trace = new ForwardingTraceListener(Combine(oldListeners, combinedOutput));

            Trace.Listeners.Clear();
            Trace.Listeners.Add(trace);
        }

        public void Dispose()
        {
            Console.SetOut(oldOut);
            Console.SetError(oldError);

            try
            {
                Trace.Listeners.Clear();
                Trace.Listeners.AddRange(oldListeners);
            }
            catch (Exception)
            {
                // suppress
            }
        }

        private static TraceListener[] Combine(IEnumerable<TraceListener> traceListeners, TextWriter textWriter)
        {
            return traceListeners
              .Concat(new TraceListener[] { new TextWriterTraceListener(textWriter) })
              .ToArray();
        }
    }
}
