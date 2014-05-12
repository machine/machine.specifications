namespace Machine.Specifications.Runner.Utility
{
    /// <summary>
    /// Dynamically gets the mspec assembly, and handsover tests to the mspec sdk by using reflection calls
    /// </summary>
    //public class SpecificationRunner : ISpecificationRunner
    //{
    //    public void RunAssemblies(IEnumerable<AssemblyPath> assemblyPaths, ISpecificationRunListener listener, RunOptions options)
    //    {
    //        if (options.SeperateAppDomain)
    //        {
    //            new SeperateAppDomain(assemblyPaths, listener, options);
    //        }
    //        else
    //        {
    //            new OneAppDomain(assemblyPaths, listener, options);
    //        }
    //    }

    //    private static AssemblyName GetMspecAssemblyName(string specAssemblyPath)
    //    {
    //        string mspecAssemblyFileName = Path.Combine(Path.GetDirectoryName(specAssemblyPath), "Machine.Specifications.dll");

    //        if (!File.Exists(mspecAssemblyFileName))
    //        {
    //            throw new ArgumentException("Could not find file: " + mspecAssemblyFileName);
    //        }

    //        var mspecAssemblyName = AssemblyName.GetAssemblyName(mspecAssemblyFileName);
    //        return mspecAssemblyName;
    //    }

    //    private static void UnloadAppDomain(AppDomain appDomain)
    //    {
    //        if (appDomain == null)
    //        {
    //            return;
    //        }

    //        var cachePath = appDomain.SetupInformation.CachePath;

    //        try
    //        {
    //            AppDomain.Unload(appDomain);

    //            if (cachePath != null)
    //            {
    //                Directory.Delete(cachePath, true);
    //            }
    //        }
    //        catch
    //        {
    //            // This is OK for cleanup
    //        }
    //    }

    //    private static AppDomain CreateAppDomain(string specAssemblyPath, string shadowCopyCachePath)
    //    {
    //        var setup = new AppDomainSetup
    //        {
    //            ApplicationBase = Path.GetDirectoryName(specAssemblyPath),
    //            ApplicationName = string.Format("Machine Specifications Runner for {0}", Path.GetFileName(specAssemblyPath))
    //        };

    //        if (!string.IsNullOrEmpty(shadowCopyCachePath))
    //        {
    //            setup.ShadowCopyFiles = "true";
    //            setup.ShadowCopyDirectories = setup.ApplicationBase;
    //            setup.CachePath = shadowCopyCachePath;
    //        }

    //        setup.ConfigurationFile = GetConfigFile(specAssemblyPath);

    //        return AppDomain.CreateDomain(setup.ApplicationName, null, setup, new PermissionSet(PermissionState.Unrestricted));
    //    }

    //    private static string GetConfigFile(string specAssemblyPath)
    //    {
    //        var configFile = specAssemblyPath + ".config";

    //        if (File.Exists(configFile))
    //        {
    //            return configFile;
    //        }

    //        return null;
    //    }

    //    private static void RethrowWithNoStackTraceLoss(Exception ex)
    //    {
    //        FieldInfo remoteStackTraceString = typeof(Exception).GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic);
    //        remoteStackTraceString.SetValue(ex, ex.StackTrace + Environment.NewLine);
    //        throw ex;
    //    }

    //    private class SeperateAppDomain
    //    {
    //        public SeperateAppDomain(IEnumerable<AssemblyPath> assemblyPaths, ISpecificationRunListener listener, RunOptions options)
    //        {
    //            foreach (AssemblyPath assemblyPath in assemblyPaths)
    //            {
    //                AppDomain appDomain = null;
    //                try
    //                {
    //                    string specAssemblyPath = Path.GetFullPath(assemblyPath);
    //                    IEnumerable<string> specAssemblyPaths = new[] { specAssemblyPath };
    //                    appDomain = CreateAppDomain(specAssemblyPath, options.ShadowCopyCachePath);

    //                    AssemblyName mspecAssemblyName = GetMspecAssemblyName(specAssemblyPath);

    //                    var runspecs = appDomain.CreateInstanceAndUnwrap(
    //                        mspecAssemblyName.FullName,
    //                        "Machine.Specifications.Sdk.RunSpecs",
    //                        false,
    //                        0,
    //                        null,
    //                        new object[] { listener, options.ToXml(), specAssemblyPaths },
    //                        null,
    //                        null,
    //                        null);
    //                }
    //                catch (TargetInvocationException ex)
    //                {
    //                    UnloadAppDomain(appDomain);

    //                    RethrowWithNoStackTraceLoss(ex.InnerException);
    //                }
    //                catch (Exception)
    //                {
    //                    UnloadAppDomain(appDomain);

    //                    throw;
    //                }

    //                UnloadAppDomain(appDomain);
    //            }
    //        }
    //    }

    //    private class OneAppDomain
    //    {
    //        public OneAppDomain(IEnumerable<AssemblyPath> assemblyPaths, ISpecificationRunListener listener, RunOptions options)
    //        {
    //            AppDomain appDomain = null;
    //            try
    //            {
    //                IEnumerable<string> specAssemblyPaths = assemblyPaths.Select(p => Path.GetFullPath(p)).ToList();
    //                string specAssemblyPath = Path.GetFullPath(specAssemblyPaths.First());

    //                appDomain = CreateAppDomain(specAssemblyPath, options.ShadowCopyCachePath);

    //                AssemblyName mspecAssemblyName = GetMspecAssemblyName(specAssemblyPath);

    //                var runspecs = appDomain.CreateInstanceAndUnwrap(
    //                    mspecAssemblyName.FullName,
    //                    "Machine.Specifications.Sdk.RunSpecs",
    //                    false,
    //                    0,
    //                    null,
    //                    new object[] { listener, options.ToXml(), specAssemblyPaths },
    //                    null,
    //                    null,
    //                    null);
    //            }
    //            catch (TargetInvocationException ex)
    //            {
    //                UnloadAppDomain(appDomain);

    //                RethrowWithNoStackTraceLoss(ex.InnerException);
    //            }
    //            catch (Exception)
    //            {
    //                UnloadAppDomain(appDomain);

    //                throw;
    //            }

    //            UnloadAppDomain(appDomain);
    //        }
    //    }
    //}
}