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
        private string originalDirectory;

        public override void OnAssemblyStart(AssemblyInfo assembly)
        {
            originalDirectory = Directory.GetCurrentDirectory();

            if (assembly.Location != null)
            {
                var path = Path.GetDirectoryName(assembly.Location);

                if (path != null)
                {
                    Directory.SetCurrentDirectory(path);
                }
            }
        }

        public override void OnAssemblyEnd(AssemblyInfo assembly)
        {
            Directory.SetCurrentDirectory(originalDirectory);
        }
    }
}
