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
      //no-op
    }

    public void OnRunEnd()
    {
      //no-op
    }

    public void OnContextStart(Context context)
    {
      //no-op
    }

    public void OnContextEnd(Context context)
    {
      //no-op
    }

    public void OnSpecificationStart(Specification specification)
    {
      _console.WriteLine(specification.Name);
    }

    public void OnSpecificationEnd(Specification specification, SpecificationVerificationResult result)
    {
      if (!result.Passed)
      {
        FailureOccured = true;
        _console.WriteLine(Resources.FailingSpecificationError, specification.Name, _assemblyName);
      }
    }
  }
}