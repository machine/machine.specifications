using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Machine.Specifications.Model;
using Machine.Specifications.Runner;

namespace Machine.Specifications.Reporting
{
  public class CollectReportingInformationRunListener : ISpecificationRunListener
  {
    string _currentAssemblyName;
    Context _currentContext;
    Dictionary<string, List<Context>> _contextsByAssembly;
    Dictionary<Context, List<Specification>> _specificationsByContext;
    Dictionary<Specification, SpecificationVerificationResult> _resultsBySpecification;

    public CollectReportingInformationRunListener()
    {
      _currentAssemblyName = "";
      _currentContext = null;
      _contextsByAssembly = new Dictionary<string, List<Context>>();
      _specificationsByContext = new Dictionary<Context, List<Specification>>();
      _resultsBySpecification = new Dictionary<Specification, SpecificationVerificationResult>();
    }

    public Dictionary<Specification, SpecificationVerificationResult> ResultsBySpecification
    {
      get { return _resultsBySpecification; }
    }

    public Dictionary<Context, List<Specification>> SpecificationsByContext
    {
      get { return _specificationsByContext; }
    }

    public Dictionary<string, List<Context>> ContextsByAssembly
    {
      get { return _contextsByAssembly; }
    }

    public virtual void OnAssemblyStart(System.Reflection.Assembly assembly)
    {
      _currentAssemblyName = assembly.GetName().Name;
      _contextsByAssembly.Add(_currentAssemblyName, new List<Context>());
    }

    public virtual void OnAssemblyEnd(System.Reflection.Assembly assembly)
    {
    }

    public virtual void OnRunStart()
    {
    }

    public virtual void OnRunEnd()
    {
    }

    public virtual void OnContextStart(Machine.Specifications.Model.Context context)
    {
      _contextsByAssembly[_currentAssemblyName].Add(context);
      _currentContext = context;
      _specificationsByContext.Add(_currentContext, new List<Specification>());
    }

    public virtual void OnContextEnd(Machine.Specifications.Model.Context context)
    {
    }

    public virtual void OnSpecificationStart(Machine.Specifications.Model.Specification specification)
    {
    }

    public virtual void OnSpecificationEnd(Machine.Specifications.Model.Specification specification, Machine.Specifications.Model.SpecificationVerificationResult result)
    {
      _specificationsByContext[_currentContext].Add(specification);
      _resultsBySpecification.Add(specification,result);
    }
  }
}
