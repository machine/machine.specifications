using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications.Model;

namespace Machine.Specifications.Runner.Impl
{
  public class SpecificationRunner
  {
    readonly ISpecificationRunListener _listener;
    readonly RunOptions _options;

    public SpecificationRunner(ISpecificationRunListener listener, RunOptions options)
    {
      _listener = listener;
      _options = options;
    }

    public Result Run(Specification specification)
    {
      _listener.OnSpecificationStart(specification.GetInfo());
      var result = specification.Verify();
      _listener.OnSpecificationEnd(specification.GetInfo(), result);

      return result;
    }
  }
}
