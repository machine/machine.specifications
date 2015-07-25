using System.Collections.Generic;
using System.Linq;

using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.ConsoleRunner.Outputs
{
    class FailedSpecificationsSummary
    {
        readonly IConsole _console;

        readonly Dictionary<ContextInfo, IList<FailedSpecification>> _failures =
            new Dictionary<ContextInfo, IList<FailedSpecification>>();

        readonly VerboseOutput _verbose;
        readonly IList<ExceptionResult> _fatalErrors = new List<ExceptionResult>();

        public FailedSpecificationsSummary(VerboseOutput verbose, IConsole console)
        {
            _verbose = verbose;
            _console = console;
        }

        public void WriteSummary()
        {
            if (!_failures.Any() && !_fatalErrors.Any())
            {
                return;
            }

            WriteFailures();
            WriteFatalErrors();
        }

        void WriteFailures()
        {
            if (!_failures.Any())
            {
                return;
            }

            _console.WriteLine("");
            _console.WriteLine("Failures:");

            _failures
                .OrderBy(context => context.Key.FullName)
                .ToList()
                .ForEach(context =>
                {
                    _verbose.ContextStart(context.Key);
                    context.Value.ToList().ForEach(spec =>
                    {
                        _verbose.SpecificationStart(spec.Specification);
                        _verbose.Failed(spec.Specification, spec.Result);
                    });
                });
        }

        void WriteFatalErrors()
        {
            if (!_fatalErrors.Any())
            {
                return;
            }

            _console.WriteLine("");
            _console.WriteLine("Fatal Errors:");

            _fatalErrors
                .ToList()
                .ForEach(x => _verbose.FatalError(x));

        }

        public void RecordFailure(ContextInfo context, SpecificationInfo specification, Result result)
        {
            if (!_failures.ContainsKey(context))
            {
                _failures.Add(context, new List<FailedSpecification>());
            }

            var entry = _failures[context];
            entry.Add(new FailedSpecification { Specification = specification, Result = result });
        }

        public void RecordFatalError(ExceptionResult exception)
        {
            _fatalErrors.Add(exception);
        }
    }
}
