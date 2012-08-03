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
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;

namespace Gallio.Loader.Isolation
{
    /// <summary>
    /// The Gallio.Loader.Isolation namespace provides support for using
    /// Gallio runtime services in an isolated AppDomain of the current proces.
    /// </summary>
    [CompilerGenerated]
    class NamespaceDoc
    {
    }

    /// <summary>
    /// Provides a mechanism for wrapping exceptions as <see cref="ServerException"/>s
    /// before they cross a remoting boundary to avoid trying to serialize exception
    /// types that may not exist remotely.
    /// </summary>
    internal static class ServerExceptionUtils
    {
        /// <summary>
        /// Wraps an exception as a <see cref="ServerException" />.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <returns>The wrapped exception.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="ex"/> is null.</exception>
        public static ServerException Wrap(Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException("ex");

            ServerException serverException = ex as ServerException;
            if (serverException != null)
                return serverException;

            return new ServerException(ex.ToString());
        }
    }

    /// <summary>
    /// An isolated environment provides access to an instance of the Gallio loader
    /// inside an isolated AppDomain.  The AppDomain will be unloaded when the
    /// isolated environment is disposed.
    /// </summary>
    /// <seealso cref="IsolatedEnvironmentManager"/>
    internal interface IIsolatedEnvironment : IDisposable
    {
        /// <summary>
        /// Gets the AppDomain of the isolated environment.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if the isolated environment has been disposed.</exception>
        AppDomain AppDomain { get; }

        /// <summary>
        /// Gets a proxy for the loader within the isolated environment.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if the isolated environment has been disposed.</exception>
        ILoader Loader { get; }
    }

    /// <summary>
    /// The isolated environment manager creates AppDomains within the current
    /// process with an appropriate application base directory and assembly resolver
    /// for interacting with Gallio runtime services.
    /// </summary>
    internal static class IsolatedEnvironmentManager
    {
        /// <summary>
        /// Creates an isolated environment for Gallio in a new AppDomain of this process.
        /// The AppDomain will be unloaded when the isolated environment is disposed.
        /// Uses the <see cref="DefaultRuntimeLocator"/>.
        /// </summary>
        /// <returns>The isolated environment.</returns>
        /// <exception cref="LoaderException">Thrown if the environment could not be created.</exception>
        public static IIsolatedEnvironment CreateIsolatedEnvironment()
        {
            return CreateIsolatedEnvironment(new DefaultRuntimeLocator());
        }

        /// <summary>
        /// Creates an isolated environment for Gallio in a new AppDomain of this process.
        /// The AppDomain will be unloaded when the isolated environment is disposed.
        /// </summary>
        /// <param name="runtimeLocator">The runtime locator.</param>
        /// <returns>The isolated environment.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="runtimeLocator"/> is null.</exception>
        /// <exception cref="LoaderException">Thrown if the environment could not be created.</exception>
        public static IIsolatedEnvironment CreateIsolatedEnvironment(RuntimeLocator runtimeLocator)
        {
            if (runtimeLocator == null)
                throw new ArgumentNullException("runtimeLocator");

            string runtimePath = runtimeLocator.GetValidatedRuntimePathOrThrow();

            AppDomainSetup appDomainSetup = new AppDomainSetup();
            appDomainSetup.ApplicationName = "Gallio";
            appDomainSetup.ApplicationBase = runtimePath;
            Evidence evidence = AppDomain.CurrentDomain.Evidence;
            PermissionSet defaultPermissionSet = new PermissionSet(PermissionState.Unrestricted);
            StrongName[] fullTrustAssemblies = new StrongName[0];
            AppDomain appDomain = AppDomain.CreateDomain(appDomainSetup.ApplicationName, evidence,
                appDomainSetup, defaultPermissionSet, fullTrustAssemblies);

            return InternalCreateIsolatedEnvironment(appDomain, runtimePath);
        }

        /// <summary>
        /// Creates an isolated environment for Gallio in an existing AppDomain of this process.
        /// The AppDomain will be unloaded when the isolated environment is disposed.
        /// </summary>
        /// <param name="appDomain">The AppDomain in which to create the isolated environment.</param>
        /// <param name="runtimeLocator">The runtime locator.</param>
        /// <returns>The isolated environment.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="appDomain"/>
        /// or <paramref name="runtimeLocator"/> is null.</exception>
        /// <exception cref="LoaderException">Thrown if the environment could not be created.</exception>
        public static IIsolatedEnvironment CreateIsolatedEnvironmentInExistingAppDomain(
            AppDomain appDomain, RuntimeLocator runtimeLocator)
        {
            if (appDomain == null)
                throw new ArgumentNullException("appDomain");
            if (runtimeLocator == null)
                throw new ArgumentNullException("runtimeLocator");

            string runtimePath = runtimeLocator.GetValidatedRuntimePathOrThrow();
            return InternalCreateIsolatedEnvironment(appDomain, runtimePath);
        }

        private static IIsolatedEnvironment InternalCreateIsolatedEnvironment(AppDomain appDomain, string runtimePath)
        {
            Type initializerType = typeof(IsolatedInitializer);
            Assembly initializerAssembly = initializerType.Assembly;

            IsolatedInitializer initializer;
            try
            {
                try
                {
                    initializer = (IsolatedInitializer)
                        appDomain.CreateInstanceAndUnwrap(initializerAssembly.FullName, initializerType.FullName);
                }
                catch (Exception)
                {
                    string initializerAssemblyPath = LoaderUtils.GetAssemblyPath(initializerAssembly);
                    initializer = (IsolatedInitializer)
                        appDomain.CreateInstanceFromAndUnwrap(initializerAssemblyPath, initializerType.FullName);
                }
            }
            catch (Exception ex)
            {
                throw new LoaderException("Failed to load the loader into the remote AppDomain.", ex);
            }

            var environment = new IsolatedEnvironment(appDomain, runtimePath, initializer);
            environment.Initialize();
            return environment;
        }

        private sealed class IsolatedEnvironment : IIsolatedEnvironment, ILoader
        {
            private AppDomain appDomain;
            private IsolatedInitializer initializer;
            private string runtimePath;

            public IsolatedEnvironment(AppDomain appDomain, string runtimePath, IsolatedInitializer initializer)
            {
                this.appDomain = appDomain;
                this.runtimePath = runtimePath;
                this.initializer = initializer;
            }

            public AppDomain AppDomain
            {
                get
                {
                    ThrowIfDisposed();
                    return appDomain;
                }
            }

            public ILoader Loader
            {
                get
                {
                    ThrowIfDisposed();
                    return this;
                }
            }

            public string RuntimePath
            {
                get
                {
                    ThrowIfDisposed();
                    return runtimePath;
                }
            }

            public void Dispose()
            {
                runtimePath = null;
                initializer = null;

                if (appDomain != null)
                {
                    AppDomain.Unload(appDomain);
                    appDomain = null;
                }
            }

            private void ThrowIfDisposed()
            {
                if (initializer == null)
                    throw new ObjectDisposedException("The isolated environment has been disposed.");
            }

            public void Initialize()
            {
                try
                {
                    initializer.Initialize(runtimePath);
                }
                catch (Exception ex)
                {
                    throw UnwrapException(ex);
                }
            }

            public void SetupRuntime()
            {
                try
                {
                    initializer.SetupRuntime();
                }
                catch (Exception ex)
                {
                    throw UnwrapException(ex);
                }
            }

            public void AddHintDirectory(string path)
            {
                try
                {
                    initializer.AddHintDirectory(path);
                }
                catch (Exception ex)
                {
                    throw UnwrapException(ex);
                }
            }

            public T Resolve<T>()
            {
                throw new NotSupportedException("Not supported for isolated environments.");
            }

            public object Resolve(Type serviceType)
            {
                throw new NotSupportedException("Not supported for isolated environments.");
            }

            private Exception UnwrapException(Exception ex)
            {
                if (ex is ServerException)
                    throw new LoaderException(ex.Message);
                return new LoaderException("Failed to perform operation in isolated environment.", ex);
            }
        }

        private sealed class IsolatedInitializer : MarshalByRefObject
        {
            public void Initialize(string runtimePath)
            {
                try
                {
                    LoaderManager.Initialize(new ExplicitRuntimeLocator(runtimePath));
                }
                catch (Exception ex)
                {
                    throw ServerExceptionUtils.Wrap(ex);
                }
            }

            public void SetupRuntime()
            {
                try
                {
                    LoaderManager.Loader.SetupRuntime();
                }
                catch (Exception ex)
                {
                    throw ServerExceptionUtils.Wrap(ex);
                }
            }

            public void AddHintDirectory(string path)
            {
                try
                {
                    LoaderManager.Loader.AddHintDirectory(path);
                }
                catch (Exception ex)
                {
                    throw ServerExceptionUtils.Wrap(ex);
                }
            }

            public override object InitializeLifetimeService()
            {
                return null;
            }
        }
    }
}