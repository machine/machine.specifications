using System.Collections.Generic;
using Machine.Specifications.Model;

namespace Machine.Specifications.Runner.Impl
{
    public interface IContextRunner
    {
        IEnumerable<Result> Run(
            Context context,
            ISpecificationRunListener listener,
            RunOptions options,
            IEnumerable<ICleanupAfterEveryContextInAssembly> globalCleanups,
            IEnumerable<ISupplementSpecificationResults> resultSupplementers);
    }
}
