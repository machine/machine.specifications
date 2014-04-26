using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Machine.Specifications.Runner.Utility
{
    /// <summary>
    /// Dynamically gets the mspec assembly, and handsover tests to the mspec sdk by using reflection calls
    /// </summary>
    public class VersionResilentSpecRunner : IVersionResilentSpecRunner
    {
        public void RunSpecAssemblies(IEnumerable<SpecAssemblyPath> specAssemblyPaths, ISpecificationRunListener listener, RunOptions options)
        {
            string mspecAssemblyName = GetMSpecAssemblyName(specAssemblyPaths.First());
            List<Assembly> testAssemblyList = specAssemblyPaths.Select(x => Assembly.Load(AssemblyName.GetAssemblyName(Path.GetFullPath(x)))).ToList();
            CreateObject(mspecAssemblyName, "Machine.Specifications.Sdk.RunSpecs", listener, options.ToXml(), testAssemblyList);
        }

        object CreateObject(string mspecAssemblyName, string typeName, params object[] args)
        {
            try
            {
                return Activator.CreateInstance(
                  mspecAssemblyName,
                  typeName,
                  false,
                  BindingFlags.Default,
                  null,
                  args,
                  null,
                  null,
                  null);
            }
            catch (TargetInvocationException ex)
            {
                RethrowWithNoStackTraceLoss(ex.InnerException);
                return null;
            }
        }

        private static void RethrowWithNoStackTraceLoss(Exception ex)
        {
            FieldInfo remoteStackTraceString = typeof(Exception).GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic);
            remoteStackTraceString.SetValue(ex, ex.StackTrace + Environment.NewLine);
            throw ex;
        }

        private static string GetMSpecAssemblyName(string assemblyFileName)
        {
            assemblyFileName = Path.GetFullPath(assemblyFileName);
            if (!File.Exists(assemblyFileName))
                throw new ArgumentException("Could not find file: " + assemblyFileName);

            AssemblyName mspecAssemblyName = null;
            try
            {
                var mspecFileName = Path.Combine(Path.GetDirectoryName(assemblyFileName), "Machine.Specifications.dll");

                if (!File.Exists(mspecFileName))
                    throw new ArgumentException("Could not find file: " + mspecFileName);

                mspecAssemblyName = AssemblyName.GetAssemblyName(mspecFileName);
            }
            catch (TargetInvocationException ex)
            {
                RethrowWithNoStackTraceLoss(ex.InnerException);
            }

            return mspecAssemblyName.FullName;
        }
    }
}