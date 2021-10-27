using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications.Model;

namespace Machine.Specifications.Runner.Impl
{
    internal class SpecificationRunner
    {
        readonly ISpecificationRunListener _listener;
        readonly RunOptions _options;
        readonly IEnumerable<ISupplementSpecificationResults> _resultSupplementers;

        public SpecificationRunner(ISpecificationRunListener listener, RunOptions options, IEnumerable<ISupplementSpecificationResults> resultSupplementers)
        {
            _listener = listener;
            _options = options;
            _resultSupplementers = resultSupplementers;
        }

        public Result Run(Specification specification)
        {
            _listener.OnSpecificationStart(specification.GetInfo());
            var result = specification.Verify();
            result = _resultSupplementers.Aggregate(result, (r, supplement) => supplement.SupplementResult(r));
            _listener.OnSpecificationEnd(specification.GetInfo(), result);

            return result;
        }
    }
}
