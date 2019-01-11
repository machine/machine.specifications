using System.Collections.Generic;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Reporting.Generation
{
    public class CollectReportingInformationRunListener : ISpecificationRunListener
    {
        AssemblyInfo _currentAssembly;
        ContextInfo _currentContext;
        readonly Dictionary<AssemblyInfo, List<ContextInfo>> _contextsByAssembly;
        readonly Dictionary<ContextInfo, List<SpecificationInfo>> _specificationsByContext;
        readonly Dictionary<SpecificationInfo, Result> _resultsBySpecification;

        public CollectReportingInformationRunListener()
        {
            _currentAssembly = null;
            _currentContext = null;
            _contextsByAssembly = new Dictionary<AssemblyInfo, List<ContextInfo>>();
            _specificationsByContext = new Dictionary<ContextInfo, List<SpecificationInfo>>();
            _resultsBySpecification = new Dictionary<SpecificationInfo, Result>();
        }

        public Dictionary<SpecificationInfo, Result> ResultsBySpecification
        {
            get { return _resultsBySpecification; }
        }

        public Dictionary<ContextInfo, List<SpecificationInfo>> SpecificationsByContext
        {
            get { return _specificationsByContext; }
        }

        public Dictionary<AssemblyInfo, List<ContextInfo>> ContextsByAssembly
        {
            get { return _contextsByAssembly; }
        }

        public void OnAssemblyStart(AssemblyInfo assembly)
        {
            _currentAssembly = assembly;
            _contextsByAssembly.Add(_currentAssembly, new List<ContextInfo>());
        }

        public void OnAssemblyEnd(AssemblyInfo assembly)
        {
            _currentAssembly.CapturedOutput = assembly.CapturedOutput;
        }

        public void OnRunStart()
        {
        }

        public virtual void OnRunEnd()
        {
        }

        public void OnContextStart(ContextInfo context)
        {
            _contextsByAssembly[_currentAssembly].Add(context);
            _currentContext = context;
            _specificationsByContext.Add(_currentContext, new List<SpecificationInfo>());
        }

        public void OnContextEnd(ContextInfo context)
        {
            _currentContext.CapturedOutput = context.CapturedOutput;
        }

        public void OnSpecificationStart(SpecificationInfo specificationInfo)
        {
        }

        public void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            _specificationsByContext[_currentContext].Add(specification);
            _resultsBySpecification.Add(specification, result);
        }

        public void OnFatalError(ExceptionResult exceptionResult)
        {
        }
    }
}


