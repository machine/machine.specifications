using System;
using System.IO;

namespace Machine.Specifications.Runner.Impl.Listener
{
    /// <summary>
    /// Sets the current directory to the directory containing running assembly,
    /// thus allowing external files to be referenced by relative path within
    /// specifications in the assembly.
    /// </summary>
    internal class AssemblyLocationAwareListener : RunListenerBase
    {
        string _originalDirectory;

        public override void OnAssemblyStart(AssemblyInfo assembly)
        {
            _originalDirectory = Directory.GetCurrentDirectory();
            if (assembly.Location != null)
            {
                Directory.SetCurrentDirectory(Path.GetDirectoryName(assembly.Location));
            }
        }

        public override void OnAssemblyEnd(AssemblyInfo assembly)
        {
            Directory.SetCurrentDirectory(_originalDirectory);
        }
    }
}