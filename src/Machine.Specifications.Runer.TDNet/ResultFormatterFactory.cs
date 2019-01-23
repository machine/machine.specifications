using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.TDNetRunner
{
    public class ResultFormatterFactory
    {
        private Dictionary<Status, IResultFormatter> _formatters = new Dictionary<Status, IResultFormatter>();
        public ResultFormatterFactory()
        {
            _formatters[Status.Passing] = new PassedResultFormatter();
            _formatters[Status.Failing] = new FailedResultFormatter();
            _formatters[Status.NotImplemented] = new NotImplementedResultFormatter();
            _formatters[Status.Ignored] = new IgnoredResultFormatter();
        }

        public IResultFormatter GetResultFormatterFor(Result verificationResult)
        {
            if (_formatters.ContainsKey(verificationResult.Status))
            {
                return _formatters[verificationResult.Status];
            }

            throw new Exception("Unknown Verification Result! " + verificationResult.Status);
        }
    }
}
