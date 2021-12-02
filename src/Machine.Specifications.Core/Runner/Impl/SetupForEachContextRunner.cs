using System.Collections.Generic;
using Machine.Specifications.Model;

namespace Machine.Specifications.Runner.Impl
{
    internal class SetupForEachContextRunner : IContextRunner
    {
        public IEnumerable<Result> Run(
            Context context,
            ISpecificationRunListener listener,
            RunOptions options,
            IEnumerable<ICleanupAfterEveryContextInAssembly> globalCleanups,
            IEnumerable<ISupplementSpecificationResults> resultSupplementers)
        {
            var results = new List<Result>();

            listener.OnContextStart(context.GetInfo());

            foreach (var specification in context.Specifications)
            {
                var result = Result.Pass();

                if (specification.IsExecutable)
                {
                    result = context.EstablishContext();
                }

                if (result.Passed)
                {
                    var runner = new SpecificationRunner(listener, resultSupplementers);
                    result = runner.Run(specification);
                }
                else
                {
                    results = FailSpecification(listener, specification, result);
                }

                if (specification.IsExecutable)
                {
                    var cleanupResult = context.Cleanup();

                    if (result.Passed && !cleanupResult.Passed)
                    {
                        result = cleanupResult;
                    }

                    foreach (var cleanup in globalCleanups)
                    {
                        cleanup.AfterContextCleanup();
                    }
                }

                results.Add(result);
            }

            listener.OnContextEnd(context.GetInfo());

            return results;
        }

        static List<Result> FailSpecification(ISpecificationRunListener listener, Specification specification, Result result)
        {
            listener.OnSpecificationStart(specification.GetInfo());
            listener.OnSpecificationEnd(specification.GetInfo(), result);

            return new List<Result> {result};
        }
    }
}
