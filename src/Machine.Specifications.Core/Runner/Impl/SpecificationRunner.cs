using System.Collections.Generic;
using System.Linq;
using Machine.Specifications.Model;

namespace Machine.Specifications.Runner.Impl
{
    internal class SpecificationRunner
    {
        private readonly ISpecificationRunListener listener;

        private readonly IEnumerable<ISupplementSpecificationResults> resultSupplementers;

        public SpecificationRunner(ISpecificationRunListener listener, IEnumerable<ISupplementSpecificationResults> resultSupplementers)
        {
            this.listener = listener;
            this.resultSupplementers = resultSupplementers;
        }

        public Result Run(Specification specification)
        {
            listener.OnSpecificationStart(specification.GetInfo());

            var result = specification.Verify();
            result = resultSupplementers.Aggregate(result, (r, supplement) => supplement.SupplementResult(r));

            listener.OnSpecificationEnd(specification.GetInfo(), result);

            return result;
        }
    }
}
