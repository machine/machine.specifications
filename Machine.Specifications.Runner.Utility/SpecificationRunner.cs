using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;

namespace Machine.Specifications.Runner.Utility
{
    /// <summary>
    /// Dynamically gets the mspec assembly, and handsover tests to the mspec sdk by using reflection calls
    /// </summary>
    public class SpecificationRunner : ISpecificationRunner
    {
        public void RunAssemblies(IEnumerable<AssemblyPath> assemblyPaths, ISpecificationRunListener listener, RunOptions options)
        {
            foreach (AssemblyPath assemblyPath in assemblyPaths)
            {
                AppDomain appDomain = null;
                try
                {
                    string specAssemblyPath = Path.GetFullPath(assemblyPath);

                    appDomain = CreateAppDomain(specAssemblyPath, true);

                    AssemblyName mspecAssemblyName = GetMspecAssemblyName(specAssemblyPath);

                    var runspecs = appDomain.CreateInstanceAndUnwrap(
                        mspecAssemblyName.FullName,
                        "Machine.Specifications.Sdk.RunSpecs",
                        false,
                        0,
                        null,
                        new object[] { listener, options.ToXml(), specAssemblyPath },
                        null,
                        null,
                        null);
                }
                catch (TargetInvocationException ex)
                {
                    UnloadAppDomain(appDomain);

                    RethrowWithNoStackTraceLoss(ex.InnerException);
                }
                catch (Exception)
                {
                    UnloadAppDomain(appDomain);

                    throw;
                }

                UnloadAppDomain(appDomain);
            }
        }

        private static AssemblyName GetMspecAssemblyName(string specAssemblyPath)
        {
            string mspecAssemblyFileName = Path.Combine(Path.GetDirectoryName(specAssemblyPath), "Machine.Specifications.dll");

            if (!File.Exists(mspecAssemblyFileName))
            {
                throw new ArgumentException("Could not find file: " + mspecAssemblyFileName);
            }

            var mspecAssemblyName = AssemblyName.GetAssemblyName(mspecAssemblyFileName);
            return mspecAssemblyName;
        }

        private static void UnloadAppDomain(AppDomain appDomain)
        {
            if (appDomain == null)
            {
                return;
            }

            var cachePath = appDomain.SetupInformation.CachePath;

            AppDomain.Unload(appDomain);

            if (Directory.Exists(cachePath))
            {
                Directory.Delete(cachePath, true);
            }
        }

        private static AppDomain CreateAppDomain(string specAssemblyPath, bool shadowCopy)
        {
            var setup = new AppDomainSetup
            {
                ApplicationBase = Path.GetDirectoryName(specAssemblyPath),
                ApplicationName = Guid.NewGuid().ToString()
            };

            if (shadowCopy)
            {
                setup.ShadowCopyFiles = "true";
                setup.ShadowCopyDirectories = setup.ApplicationBase;
                setup.CachePath = Path.Combine(Path.GetTempPath(), setup.ApplicationName);
            }

            setup.ConfigurationFile = GetConfigFile(specAssemblyPath);

            return AppDomain.CreateDomain(setup.ApplicationName, null, setup, new PermissionSet(PermissionState.Unrestricted));
        }

        private static string GetConfigFile(string specAssemblyPath)
        {
            var configFile = specAssemblyPath + ".config";

            if (File.Exists(configFile))
            {
                return configFile;
            }

            return null;
        }

        private static void RethrowWithNoStackTraceLoss(Exception ex)
        {
            FieldInfo remoteStackTraceString = typeof(Exception).GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic);
            remoteStackTraceString.SetValue(ex, ex.StackTrace + Environment.NewLine);
            throw ex;
        }

    }
}