using System;
using System.IO;
using System.Reflection;

namespace Machine.Specifications.Runner.VisualStudio.Helpers
{
    internal static class AssemblyHelper
    {
        public static Assembly Load(string path)
        {
            try
            {
#if NETCOREAPP
                return Assembly.Load(new AssemblyName(Path.GetFileNameWithoutExtension(path)));
#else
                return Assembly.LoadFile(path);
#endif
            } catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
