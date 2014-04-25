using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Machine.Specifications.Runner.Impl.Listener.Redirection
{
    internal class OutputInterceptor : IDisposable
    {
        readonly TextWriter _combinedOutput;
        TextWriter _oldError;
        TraceListener[] _oldListeners;
        TextWriter _oldOut;

        public OutputInterceptor()
        {
            _combinedOutput = new StringWriter();

            CaptureStandardOut();
            CaptureStandardError();
            CaptureDebugTrace();
        }

        void CaptureStandardOut()
        {
            _oldOut = Console.Out;
            var stdOut = new ForwardingStringWriter(new[] { _oldOut, _combinedOutput });
            Console.SetOut(stdOut);
        }

        void CaptureStandardError()
        {
            _oldError = Console.Error;
            var stdError = new ForwardingStringWriter(new[] { _oldError, _combinedOutput });
            Console.SetError(stdError);
        }

        void CaptureDebugTrace()
        {
            _oldListeners = new TraceListener[Trace.Listeners.Count];
            Trace.Listeners.CopyTo(_oldListeners, 0);

            var trace = new ForwardingTraceListener(Combine(_oldListeners, _combinedOutput));
            Trace.Listeners.Clear();
            Trace.Listeners.Add(trace);
        }

        public string CapturedOutput
        {
            get { return _combinedOutput.ToString(); }
        }

        public void Dispose()
        {
            Console.SetOut(_oldOut);
            Console.SetError(_oldError);
            try
            {
                Trace.Listeners.Clear();
                Trace.Listeners.AddRange(_oldListeners);
            }
            catch (Exception)
            {
            }
        }

        static TraceListener[] Combine(IEnumerable<TraceListener> traceListeners, TextWriter textWriter)
        {
            return traceListeners
              .Concat(new TraceListener[] { new TextWriterTraceListener(textWriter) })
              .ToArray();
        }
    }
}