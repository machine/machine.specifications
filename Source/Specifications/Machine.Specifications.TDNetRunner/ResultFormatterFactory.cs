using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications.Model;

namespace Machine.Specifications.TDNetRunner
{
  public class ResultFormatterFactory
  {
    private Dictionary<Result, IResultFormatter> _formatters = new Dictionary<Result, IResultFormatter>();
    public ResultFormatterFactory()
    {
      _formatters[Result.Passed] = new PassedResultFormatter();
      _formatters[Result.Failed] = new FailedResultFormatter();
      _formatters[Result.Unknown] = new UnknownResultFormatter();
    }

    public IResultFormatter GetResultFormatterFor(SpecificationVerificationResult verificationResult)
    {
      if (_formatters.ContainsKey(verificationResult.Result))
      {
        return _formatters[verificationResult.Result];
      }

      throw new Exception("Unknown Verification Result! " + verificationResult.Result);
    }
  }
}
