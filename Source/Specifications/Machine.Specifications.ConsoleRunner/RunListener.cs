using System.Reflection;

using Machine.Specifications.ConsoleRunner.Properties;
using Machine.Specifications.Model;
using Machine.Specifications.Runner;

namespace Machine.Specifications.ConsoleRunner
{
  public class RunListener : ISpecificationRunListener
  {
    readonly IConsole _console;
    string _assemblyName;
    int _contextCount;
    int _specificationCount;

    public bool FailureOccured
    {
      get; private set;
    }

    public RunListener(IConsole console)
    {
      _console = console;
    }

    public void OnAssemblyStart(Assembly assembly)
    {
      _assemblyName = assembly.GetName().Name;
    }

    public void OnAssemblyEnd(Assembly assembly)
    {
    }

    public void OnRunStart()
    {
      _contextCount = 0;
      _specificationCount = 0;
    }

    public void OnRunEnd()
    {
      _console.WriteLine("Contexts: {0} Specifications: {1}", _contextCount, _specificationCount);
    }

    public void OnContextStart(Context context)
    {
      _console.WriteLine(context.FullName);
    }

    public void OnContextEnd(Context context)
    {
      _contextCount += 1;
    }

    public void OnSpecificationStart(Specification specification)
    {
      _console.WriteLine(specification.Name);
    }

    public void OnSpecificationEnd(Specification specification, SpecificationVerificationResult result)
    {
      _specificationCount += 1;
      if (!result.Passed)
      {
        FailureOccured = true;
        _console.WriteLine(Resources.FailingSpecificationError, specification.Name, _assemblyName);
      }
    }
  }
}