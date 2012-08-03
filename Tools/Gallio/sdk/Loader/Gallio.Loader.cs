// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Microsoft.Win32;

namespace Gallio.Loader
{
    /// <summary>
    /// The Gallio.Loader namespace contains types for loading the Gallio runtime
    /// into an application.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The loader is not distributed as a compiled assembly.  Instead it is intended
    /// to be included as a source file within the application itself.
    /// </para>
    /// <para>
    /// For documentation see: http://www.gallio.org/wiki/devel:gallio_loader
    /// </para>
    /// </remarks>
    [CompilerGenerated]
    class NamespaceDoc
    {
    }
    
    /// <summary>
    /// Helper utilities for implementing the Gallio loader.
    /// </summary>
    internal static class LoaderUtils
    {
        /// <summary>
        /// Gets the local path of an assembly.  (Not its shadow copied location.)
        /// </summary>
        /// <param name="assembly">The assembly, not null.</param>
        /// <returns>The assembly path.</returns>
        public static string GetAssemblyPath(Assembly assembly)
        {
            return new Uri(assembly.CodeBase).LocalPath;
        }
        
        /// <summary>
        /// Gets the directory path of the assembly that contains the loader.
        /// </summary>
        /// <returns>The directory path of the assembly that contains the loader.</returns>
        public static string GetLoaderDirectoryPath()
        {
            return Path.GetDirectoryName(GetAssemblyPath(typeof(LoaderUtils).Assembly));
        }
    }

    /// <summary>
    /// The exception that is thrown by the loader when an operation cannot be performed.
    /// </summary>
    [Serializable]
    internal class LoaderException : Exception
    {
        /// <summary>
        /// Creates an exception.
        /// </summary>
        public LoaderException()
        {
        }

        /// <summary>
        /// Creates an exception.
        /// </summary>
        /// <param name="message">The message.</param>
        public LoaderException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates an exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public LoaderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates an exception from serialization info.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected LoaderException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary>
    /// Specifies how to locate the Gallio runtime.
    /// </summary>
    internal abstract class RuntimeLocator
    {
        /// <summary>
        /// Gets the path of the runtime and validates it.  If not found or if a
        /// problem occurs, throws.
        /// </summary>
        /// <returns>The validated runtime path.</returns>
        /// <exception cref="LoaderException">Thrown if the runtime path could not be obtained or validated.</exception>
        public virtual string GetValidatedRuntimePathOrThrow()
        {
            try
            {
                string runtimePath = GetRuntimePath();
                if (runtimePath == null)
                {
                    throw new LoaderException(string.Format(
                        "Could not determine the Gallio runtime path.\n\n{0}",
                        GetFailureHint()));
                }

                if (! IsRuntimePathValid(runtimePath))
                {
                    throw new LoaderException(string.Format(
                        "The Gallio runtime path '{0}' appears to be invalid.\n\n{1}",
                        runtimePath, GetFailureHint()));
                }

                return runtimePath;
            }
            catch (LoaderException)
            {
                throw;
            }            
            catch (Exception ex)
            {
                throw new LoaderException(string.Format(
                    "An exception occurred while attempting to determine the Gallio runtime path.\n\n{0}",
                    GetFailureHint()), ex);
            }
        }

        /// <summary>
        /// Returns a hint to help the user figure out how to resolve an issue
        /// involving a bad runtime path.
        /// </summary>
        public abstract string GetFailureHint();

        /// <summary>
        /// Gets the path of the runtime.
        /// </summary>
        /// <returns>The runtime path, or null if not found.</returns>
        public abstract string GetRuntimePath();

        /// <summary>
        /// Returns true if the specified runtime path is valid.
        /// </summary>
        /// <param name="runtimePath">The runtime path, not null.</param>
        /// <returns>True if the path was valid.</returns>
        public virtual bool IsRuntimePathValid(string runtimePath)
        {
            return File.Exists(GetGallioAssemblyPath(runtimePath));
        }

        /// <summary>
        /// Gets the path of the Gallio assembly.
        /// </summary>
        /// <param name="runtimePath">The runtime path, not null.</param>
        /// <returns>The path of the Gallio assembly.</returns>
        public virtual string GetGallioAssemblyPath(string runtimePath)
        {
            return Path.Combine(runtimePath, "Gallio.dll");
        }
    }

    /// <summary>
    /// Locates the runtime path automatically using environment variables and the registry.
    /// </summary>
    internal class DefaultRuntimeLocator : RuntimeLocator
    {
        /// <inheritdoc />
        public override string GetFailureHint()
        {
            return "This usually means that this program or Gallio has not been installed correctly.\n"
                + "As a workaround, you can set the GALLIO_RUNTIME_PATH environment variable to force "
                + "the program to look for Gallio in a particular directory such as 'C:\\Gallio\\bin'.  "
                + "Alternately, you can set the GALLIO_RUNTIME_VERSION environment variable to force the "
                + "program to search for a particular version of Gallio in the registry, such as '3.2'.";
        }

        /// <summary>
        /// Gets the path of the runtime.
        /// </summary>
        /// <remarks>
        /// The default implementation tries <see cref="GetRuntimePathUsingEnvironment" />
        /// then <see cref="GetRuntimePathUsingRegistry" /> then
        /// <see cref="GetRuntimePathUsingApplicationBaseDirectoryOrAncestor"/>.
        /// </remarks>
        /// <returns>The runtime path, or null if not found.</returns>
        public override string GetRuntimePath()
        {
            return GetRuntimePathUsingEnvironment()
                ?? GetRuntimePathUsingRegistry()
                ?? GetRuntimePathUsingApplicationBaseDirectoryOrAncestor();
        }

        /// <summary>
        /// Gets the Gallio runtime path by examining the GALLIO_RUNTIME_PATH
        /// environment variable.
        /// </summary>
        /// <returns>The runtime path, or null if not found.</returns>
        protected virtual string GetRuntimePathUsingEnvironment()
        {
            return Environment.GetEnvironmentVariable("GALLIO_RUNTIME_PATH");
        }

        /// <summary>
        /// Gets the Gallio runtime path by searching the registry for the
        /// version returned by <see cref="GetRuntimeVersion"/>.
        /// </summary>
        /// <returns>The runtime path, or null if not found.</returns>
        protected virtual string GetRuntimePathUsingRegistry()
        {
            string runtimeVersion = GetRuntimeVersion();
            if (runtimeVersion == null)
                return null;

            foreach (string installationKey in new[]
            {
                @"HKEY_CURRENT_USER\Software\Gallio.org\Gallio\" + runtimeVersion,
                @"HKEY_CURRENT_USER\Software\Wow6432Node\Gallio.org\Gallio\" + runtimeVersion,
                @"HKEY_LOCAL_MACHINE\Software\Gallio.org\Gallio\" + runtimeVersion,
                @"HKEY_LOCAL_MACHINE\Software\Wow6432Node\Gallio.org\Gallio\" + runtimeVersion
            })
            {
                string installationFolder = Registry.GetValue(installationKey, @"InstallationFolder", null) as string;
                if (installationFolder != null)
                    return Path.Combine(installationFolder, "bin");
            }

            return null;
        }

        /// <summary>
        /// Gets the Gallio runtime path by searching the application base directory
        /// and its ancestors for Gallio.dll.
        /// </summary>
        /// <remarks>
        /// This fallback option only makes sense for applications that are themselves
        /// installed in the Gallio runtime path.  It is also useful for
        /// debugging since debug versions of Gallio contain extra special logic
        /// hardcoded in the runtime to tweak the runtime path assuming that the
        /// application resides in the Gallio source tree.
        /// </remarks>
        /// <returns>The runtime path, or null if not found.</returns>
        protected virtual string GetRuntimePathUsingApplicationBaseDirectoryOrAncestor()
        {
            string candidatePath = AppDomain.CurrentDomain.BaseDirectory;
            while (candidatePath != null)
            {
                if (IsRuntimePathValid(candidatePath))
                    return candidatePath;

                candidatePath = Path.GetDirectoryName(candidatePath);
            }
            return null;
        }

        /// <summary>
        /// Gets the version of Gallio to load.
        /// </summary>
        /// <remarks>
        /// The default implementation tries <see cref="GetRuntimeVersionUsingEnvironment" />
        /// then <see cref="GetRuntimeVersionUsingReferencedAssemblies" />.
        /// </remarks>
        /// <returns>The runtime version, or null if not found.</returns>
        protected virtual string GetRuntimeVersion()
        {
            return GetRuntimeVersionUsingEnvironment()
                ?? GetRuntimeVersionUsingReferencedAssemblies();
        }
        
        /// <summary>
        /// Gets the version of Gallio to load by examining the GALLIO_RUNTIME_VERSION
        /// environment variable.
        /// </summary>
        /// <returns>The runtime version, or null if not found.</returns>
        protected virtual string GetRuntimeVersionUsingEnvironment()
        {
            return Environment.GetEnvironmentVariable("GALLIO_RUNTIME_VERSION");
        }

        /// <summary>
        /// Gets the version of Gallio to load by searching the referenced assemblies
        /// for an assembly name for which <see cref="IsGallioAssemblyName"/> returns true
        /// then returns the runtime version that was discovered.
        /// </summary>
        /// <returns>The runtime version, or null if not found.</returns>
        protected virtual string GetRuntimeVersionUsingReferencedAssemblies()
        {
            foreach (AssemblyName assemblyName in GetReferencedAssemblies())
            {
                if (IsGallioAssemblyName(assemblyName))
                    return string.Format(CultureInfo.InvariantCulture, "{0}.{1}",
                        assemblyName.Version.Major, assemblyName.Version.Minor);
            }

            return null;
        }

        /// <summary>
        /// Returns true if the assembly name is a Gallio assembly.
        /// </summary>
        /// <remarks>
        /// The default implementation returns true for any assembly called
        /// "Gallio" or whose name starts with "Gallio.".
        /// </remarks>
        /// <param name="assemblyName">The assembly name to check, not null.</param>
        /// <returns>True if the assembly is a Gallio assembly.</returns>
        protected virtual bool IsGallioAssemblyName(AssemblyName assemblyName)
        {
            return assemblyName.Name == "Gallio"
                || assemblyName.Name.StartsWith("Gallio.");
        }

        /// <summary>
        /// Gets the referenced assemblies to search for a Gallio reference in order
        /// to automatically determine the version of Gallio to load.
        /// </summary>
        /// <remarks>
        /// The default implementation returns the name of the assemblies referenced by the
        /// currently executing assembly and the name of the executing assembly itself.
        /// </remarks>
        /// <returns>The referenced assemblies.</returns>
        protected virtual IEnumerable<AssemblyName> GetReferencedAssemblies()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            foreach (AssemblyName assemblyName in executingAssembly.GetReferencedAssemblies())
                yield return assemblyName;
            yield return executingAssembly.GetName();
        }
    }

    /// <summary>
    /// A runtime locator that uses an explicitly specified runtime path.
    /// </summary>
    internal class ExplicitRuntimeLocator : RuntimeLocator
    {
        private readonly string runtimePath;

        /// <summary>
        /// Creates the explicit runtime locator.
        /// </summary>
        /// <param name="runtimePath">The runtime path, or null if not found.</param>
        public ExplicitRuntimeLocator(string runtimePath)
        {
            this.runtimePath = runtimePath;
        }

        public override string GetFailureHint()
        {
            return "";
        }

        public override string GetRuntimePath()
        {
            return runtimePath;
        }
    }

    /// <summary>
    /// The loader provides access to Gallio runtime services from an application
    /// that might not be installed in Gallio's runtime path.
    /// </summary>
    /// <seealso cref="LoaderManager"/>
    internal interface ILoader
    {
        /// <summary>
        /// Gets the Gallio runtime path used by the loader.
        /// </summary>
        /// <exception cref="LoaderException">Thrown if the operation could not be performed.</exception>
        string RuntimePath { get; }

        /// <summary>
        /// Sets up the runtime with a default runtime setup using the loader's
        /// runtime path and a null logger.  Does nothing if the runtime has
        /// already been initialized.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If you need more control over this behavior, call RuntimeBootstrap
        /// yourself.
        /// </para>
        /// </remarks>
        /// <exception cref="LoaderException">Thrown if the operation could not be performed.</exception>
        void SetupRuntime();

        /// <summary>
        /// Adds a hint directory to the assembly resolver.
        /// </summary>
        /// <param name="path">The path of the hint directory to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> is null.</exception>
        /// <exception cref="LoaderException">Thrown if the operation could not be performed.</exception>
        void AddHintDirectory(string path);

        /// <summary>
        /// Resolves a runtime service.
        /// </summary>
        /// <typeparam name="T">The type of service to resolve.</typeparam>
        /// <returns>The resolved service.</returns>
        /// <exception cref="LoaderException">Thrown if the operation could not be performed.</exception>
        T Resolve<T>();

        /// <summary>
        /// Resolves a runtime service.
        /// </summary>
        /// <param name="serviceType">The type of service to resolve.</param>
        /// <returns>The resolved service.</returns>
        /// <exception cref="LoaderException">Thrown if the operation could not be performed.</exception>
        object Resolve(Type serviceType);
    }


    /// <summary>
    /// The loader manager is responsible for initializing a loader within the current process.
    /// At most one loader can exist in a process.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Gallio loader is used in situations where applications that are not installed
    /// in the Gallio runtime path need to load Gallio assemblies.  We can't just copy
    /// Gallio assemblies into the application base directory because it creates
    /// assembly loading ambiguities.  In particular, it is possible for multiple copies
    /// to be loaded in the same process simultaneously in different load contexts
    /// (Load / LoadFrom / LoadFile).  When multiple copies of the same assembly are loaded
    /// their types are considered distinct and they cannot reliably exchange information
    /// with other components (like plugins).
    /// </para>
    /// <para>
    /// For example, assembly load context problems were observed when two different Visual
    /// Studio add-ins installed in different locations were loaded at the same time.
    /// The Gallio loader avoids this problem by ensuring that there is only one copy of
    /// Gallio installed on the machine.  Applications installed outside of the Gallio
    /// runtime path are required to use a custom assembly resolver to load Gallio assemblies.
    /// </para>
    /// <para>
    /// Before the loader is initialized, it might not be possible to load
    /// certain Gallio types unless the appropriate assemblies have been
    /// copied into the application base directory.  (In fact, that's the
    /// problem that the loader is designed to solve.)
    /// </para>
    /// <para>
    /// Consequently it is very important that the application refrain
    /// from causing any Gallio types to be loaded.  This bears repeating!
    /// Don't try to use any Gallio functions or declare any fields of
    /// Gallio types until the loader is initialized.  Otherwise a runtime
    /// exception will probably be thrown.  Ideally you should initialize
    /// the loader in your program's main entry point before doing anything
    /// else.
    /// </para>
    /// <para>
    /// When the loader is initialized, it will install a custom assembly
    /// resolver to ensure that assemblies in the Gallio runtime path can
    /// be resolved.  In particular, this makes it possible to initialize
    /// the Gallio runtime extensibility framework.
    /// </para>
    /// </remarks>
    internal static class LoaderManager
    {
        private static ILoader loader;

        /// <summary>
        /// Gets the current loader, or null if it has not been initialized.
        /// </summary>
        public static ILoader Loader
        {
            get { return loader; }
        }

        /// <summary>
        /// If the loader has not been initialized, initializes it using the default
        /// runtime locator and sets up the runtime.
        /// </summary>
        public static void InitializeAndSetupRuntimeIfNeeded()
        {
            if (loader == null)
            {
                Initialize();
                loader.SetupRuntime();
            }
        }

        /// <summary>
        /// Initializes the loader using the <see cref="DefaultRuntimeLocator"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the loader has already been initialized.</exception>
        /// <exception cref="LoaderException">Thrown if the loader could not be initialized.</exception>
        public static void Initialize()
        {
            Initialize(new DefaultRuntimeLocator());
        }

        /// <summary>
        /// Initializes the loader.
        /// </summary>
        /// <param name="locator">The runtime locator.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="locator"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the loader has already been initialized.</exception>
        /// <exception cref="LoaderException">Thrown if the loader could not be initialized.</exception>
        public static void Initialize(RuntimeLocator locator)
        {
            if (locator == null)
                throw new ArgumentNullException("locator");
            if (loader != null)
                throw new InvalidOperationException("The loader has already been initialized.");

            string runtimePath = locator.GetValidatedRuntimePathOrThrow();
            string gallioAssemblyPath = locator.GetGallioAssemblyPath(runtimePath);

            LoaderImpl newLoader = new LoaderImpl(runtimePath, gallioAssemblyPath);
            newLoader.Initialize();
            loader = newLoader;
        }

        private class LoaderImpl : ILoader
        {
            private const string BootstrapTypeFullName = "Gallio.Runtime.Loader.GallioLoaderBootstrap";
            private const string BootstrapInstallAssemblyLoaderMethodName = "InstallAssemblyLoader";
            private const string BootstrapSetupRuntimeMethodName = "SetupRuntime";
            private const string BootstrapAddHintDirectoryMethodName = "AddHintDirectory";
            private const string BootstrapResolveMethodName = "Resolve";

            private readonly string runtimePath;
            private readonly string gallioAssemblyPath;

            private Assembly gallioAssembly;
            private Type bootstrapType;

            public LoaderImpl(string runtimePath, string gallioAssemblyPath)
            {
                this.runtimePath = runtimePath;
                this.gallioAssemblyPath = gallioAssemblyPath;
            }

            public string RuntimePath
            {
                get { return runtimePath; }
            }

            public void Initialize()
            {
                LoadGallioAssembly();
                LoadBootstrapType();
                InstallAssemblyLoader();
            }

            public void SetupRuntime()
            {
                try
                {
                    MethodInfo method = GetBootstrapMethod(BootstrapSetupRuntimeMethodName);
                    method.Invoke(null, new object[] { runtimePath });
                }
                catch (TargetInvocationException ex)
                {
                    throw new LoaderException("Failed to setup the runtime.", ex.InnerException);
                }
                catch (Exception ex)
                {
                    throw new LoaderException("Failed to setup the runtime.", ex);
                }
            }

            public void AddHintDirectory(string path)
            {
                if (path == null)
                    throw new ArgumentNullException("path");

                try
                {
                    MethodInfo method = GetBootstrapMethod(BootstrapAddHintDirectoryMethodName);
                    method.Invoke(null, new object[] { path });
                }
                catch (TargetInvocationException ex)
                {
                    throw new LoaderException("Failed to add hint directory.", ex.InnerException);
                }
                catch (Exception ex)
                {
                    throw new LoaderException("Failed to add hint directory.", ex);
                }
            }

            public T Resolve<T>()
            {
                return (T)Resolve(typeof(T));
            }

            public object Resolve(Type serviceType)
            {
                if (serviceType == null)
                    throw new ArgumentNullException("serviceType");

                try
                {
                    MethodInfo method = GetBootstrapMethod(BootstrapResolveMethodName);
                    return method.Invoke(null, new object[] { serviceType });
                }
                catch (TargetInvocationException ex)
                {
                    throw new LoaderException("Failed to resolve service.", ex.InnerException);
                }
                catch (Exception ex)
                {
                    throw new LoaderException("Failed to resolve service.", ex);
                }
            }

            private void LoadGallioAssembly()
            {
                try
                {
                    try
                    {
                        // Try to resolve the Gallio assembly if we already know where it is.
                        // But we need to sanity check that it is the right version otherwise we will
                        // encounter conflicts.
                        gallioAssembly = Assembly.Load("Gallio");
                        if (gallioAssembly.Location == null
                            || Path.GetFullPath(gallioAssembly.Location) != Path.GetFullPath(gallioAssemblyPath))
                        {
                            AssemblyName expectedAssemblyName = AssemblyName.GetAssemblyName(gallioAssemblyPath);
                            if (gallioAssembly.FullName != expectedAssemblyName.FullName)
                                throw new LoaderException(string.Format(
                                    "Failed to load the expected version of the Gallio assembly.  Actually loaded '{0}' but expected '{1}'.",
                                    gallioAssembly.FullName, expectedAssemblyName.FullName));
                        }
                    }
                    catch (FileNotFoundException)
                    {
                        gallioAssembly = Assembly.LoadFrom(gallioAssemblyPath);
                    }
                }
                catch (LoaderException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new LoaderException(string.Format(
                        "Failed to load the Gallio assembly from '{0}'.",
                        gallioAssemblyPath), ex);
                }
            }

            private void LoadBootstrapType()
            {
                try
                {
                    bootstrapType = gallioAssembly.GetType(BootstrapTypeFullName, true);
                }
                catch (Exception ex)
                {
                    throw new LoaderException("Failed to load the bootstrap type.", ex);
                }
            }

            private void InstallAssemblyLoader()
            {
                try
                {
                    MethodInfo method = GetBootstrapMethod(BootstrapInstallAssemblyLoaderMethodName);
                    method.Invoke(null, new object[] { runtimePath });
                }
                catch (TargetInvocationException ex)
                {
                    throw new LoaderException("Failed to install the assembly loader.", ex.InnerException);
                }
                catch (Exception ex)
                {
                    throw new LoaderException("Failed to install the assembly resolver.", ex);
                }
            }

            private MethodInfo GetBootstrapMethod(string methodName)
            {
                MethodInfo method = bootstrapType.GetMethod(methodName);
                if (method == null)
                    throw new LoaderException(String.Format("Could not resolve method '{0}' on bootstrap type.", methodName));
                return method;
            }
        }
    }
}
