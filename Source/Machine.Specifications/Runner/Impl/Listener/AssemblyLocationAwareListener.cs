using System;
using System.IO;

namespace Machine.Specifications.Runner.Impl.Listener
{
  /// <summary>
  /// Sets the current directory to the directory containing running assembly,
  /// thus allowing external files to be referenced by relative path within
  /// specifications in the assembly.
  /// </summary>
  class AssemblyLocationAwareListener : RunListenerBase
  {
    string _originalDirectory;

    public override void OnAssemblyStart(AssemblyInfo assembly)
    {
      _originalDirectory = Environment.CurrentDirectory;
      Environment.CurrentDirectory = Path.GetDirectoryName(assembly.Location);
    }

    public override void OnAssemblyEnd(AssemblyInfo assembly)
    {
      Environment.CurrentDirectory = _originalDirectory;
    }
  }
}