using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Machine.Specifications.Model;
using Machine.Specifications.Runner;

namespace Machine.Specifications.Reporting
{
  public class ReportingRunListener : ISpecificationRunListener
  {
    string _currentAssemblyName;
    string _currentContextName;
    Dictionary<string, List<Context>> _contextsByAssembly;
    Dictionary<string, List<Specification>> _specificationsByContext;
    Dictionary<Specification, SpecificationVerificationResult> _resultsBySpecification;


    public ReportingRunListener()
    {
      _currentAssemblyName = "";
      _currentContextName = "";
      _contextsByAssembly = new Dictionary<string, List<Context>>();
      _specificationsByContext = new Dictionary<string, List<Specification>>();
      _resultsBySpecification = new Dictionary<Specification, SpecificationVerificationResult>();
    }

    public void OnAssemblyStart(System.Reflection.Assembly assembly)
    {
      _currentAssemblyName = assembly.GetName().FullName;
      _contextsByAssembly.Add(_currentAssemblyName, new List<Context>());
    }

    public void OnAssemblyEnd(System.Reflection.Assembly assembly)
    {
    }

    public void OnRunStart()
    {
    }

    public void OnRunEnd()
    {
    }

    public void OnContextStart(Machine.Specifications.Model.Context context)
    {
      _contextsByAssembly[_currentAssemblyName].Add(context);
      _currentContextName = context.FullName;
      _specificationsByContext.Add(_currentContextName, new List<Specification>());
    }

    public void OnContextEnd(Machine.Specifications.Model.Context context)
    {
    }

    public void OnSpecificationStart(Machine.Specifications.Model.Specification specification)
    {
    }

    public void OnSpecificationEnd(Machine.Specifications.Model.Specification specification, Machine.Specifications.Model.SpecificationVerificationResult result)
    {
      _specificationsByContext[_currentContextName].Add(specification);
      _resultsBySpecification.Add(specification,result);
    }
  }
}
