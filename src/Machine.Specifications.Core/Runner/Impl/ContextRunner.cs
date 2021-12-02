using System.Collections.Generic;
using Machine.Specifications.Model;
using System.Linq;

namespace Machine.Specifications.Runner.Impl
{
    internal class SetupOnceContextRunner : IContextRunner
    {
        public IEnumerable<Result> Run(
            Context context,
            ISpecificationRunListener listener,
            RunOptions options,
            IEnumerable<ICleanupAfterEveryContextInAssembly> globalCleanups,
            IEnumerable<ISupplementSpecificationResults> resultSupplementers)
        {
            listener.OnContextStart(context.GetInfo());

            var result = Result.Pass();

            if (context.HasExecutableSpecifications)
            {
                result = context.EstablishContext();
            }

            var results = result.Passed
                ? RunSpecifications(context, listener, options, resultSupplementers)
                : FailSpecifications(context, listener, result, resultSupplementers);

            if (context.HasExecutableSpecifications)
            {
                var cleanupResult = context.Cleanup();

                if (!cleanupResult.Passed)
                {
                    listener.OnFatalError(cleanupResult.Exception);
                }

                foreach (var cleanup in globalCleanups)
                {
                    cleanup.AfterContextCleanup();
                }
            }

            listener.OnContextEnd(context.GetInfo());

            return results;
        }

        private static IEnumerable<Result> RunSpecifications(
            Context context,
            ISpecificationRunListener listener,
            RunOptions options,
            IEnumerable<ISupplementSpecificationResults> resultSupplementers)
        {
            var results = new List<Result>();

            foreach (var specification in context.Specifications)
            {
                var runner = new SpecificationRunner(listener, resultSupplementers);
                var result = runner.Run(specification);

                results.Add(result);
            }

            return results;
        }

        private static IEnumerable<Result> FailSpecifications(
            Context context,
            ISpecificationRunListener listener,
            Result result,
            IEnumerable<ISupplementSpecificationResults> resultSupplementers)
        {
            result = resultSupplementers.Aggregate(result, (r, supplement) => supplement.SupplementResult(r));

            var results = new List<Result>();

            foreach (var specification in context.Specifications)
            {
                listener.OnSpecificationStart(specification.GetInfo());
                listener.OnSpecificationEnd(specification.GetInfo(), result);

                results.Add(result);
            }

            return results;
        }
    }
}
