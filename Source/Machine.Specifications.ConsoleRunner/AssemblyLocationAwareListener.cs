using System;
using System.IO;
using Machine.Specifications.Runner;

namespace Machine.Specifications.ConsoleRunner
{
  /// <summary>
  /// Sets the current directory to the directory containing running assembly, 
  /// thus allowing external files to be referenced by relative path within 
  /// specifications in the assembly. 
  /// </summary>
  public class AssemblyLocationAwareListener : ISpecificationRunListener
  {
    string originalDirectory;

    public void OnAssemblyStart(AssemblyInfo assembly)
    {
      this.originalDirectory = Environment.CurrentDirectory;
      Environment.CurrentDirectory = Path.GetDirectoryName(assembly.Location);
    }

    public void OnAssemblyEnd(AssemblyInfo assembly)
    {
      Environment.CurrentDirectory = this.originalDirectory;
    }

    public void OnRunStart()
    {
    }

    public void OnRunEnd()
    {
    }

    public void OnContextStart(ContextInfo context)
    {
    }

    public void OnContextEnd(ContextInfo context)
    {
    }

    public void OnSpecificationStart(SpecificationInfo specification)
    {
    }

    public void OnSpecificationEnd(SpecificationInfo specification, Result result)
    {
    }

    public void OnFatalError(ExceptionResult exception)
    {
    }
  }
}