using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Machine.Specifications.Explorers;
using Machine.Specifications.Model;
using Machine.Specifications.Runner.Impl.Listener;

namespace Machine.Specifications.Runner.Impl
{
    internal class AssemblyRunner
    {
        readonly AggregateRunListener _listener;
        readonly RunOptions _options;
        Action<Assembly> _assemblyStart;
        Action<Assembly> _assemblyEnd;

        public AssemblyRunner(ISpecificationRunListener listener, RunOptions options)
        {
            var state = new RedirectOutputState();
            _listener = new AggregateRunListener(new[]
                                           {
                                             new AssemblyLocationAwareListener(),
                                             new SetUpRedirectOutputRunListener(state),
                                             listener,
                                             new TearDownRedirectOutputRunListener(state),
                                           });
            _options = options;

            _assemblyStart = OnAssemblyStart;
            _assemblyEnd = OnAssemblyEnd;
        }

        public void Run(Assembly assembly, IEnumerable<Context> contexts)
        {
            var hasExecutableSpecifications = false;
            AssemblyContextRunListener assemblyContextRunListener = new AssemblyContextRunListener(assembly);

            try
            {
                _listener.Add(assemblyContextRunListener);
                hasExecutableSpecifications = contexts.Any(x => x.HasExecutableSpecifications);

                var explorer = new AssemblyExplorer();
                var globalCleanups = explorer.FindAssemblyWideContextCleanupsIn(assembly).ToList();
                var specificationSupplements = explorer.FindSpecificationSupplementsIn(assembly).ToList();

                if (hasExecutableSpecifications)
                {
                    _assemblyStart(assembly);
                }

                foreach (var context in contexts)
                {
                    RunContext(context, globalCleanups, specificationSupplements);
                }
            }
            catch (Exception err)
            {
                _listener.OnFatalError(new ExceptionResult(err));
            }
            finally
            {
                try
                {
                    if (hasExecutableSpecifications)
                    {
                        _assemblyEnd(assembly);
                    }
                }
                finally
                {
                    _listener.Remove(assemblyContextRunListener);
                }

            }
        }

        void OnAssemblyStart(Assembly assembly)
        {
            try
            {
                _listener.OnAssemblyStart(assembly.GetInfo());
            }
            catch (Exception err)
            {
                _listener.OnFatalError(new ExceptionResult(err));
            }
        }

        void OnAssemblyEnd(Assembly assembly)
        {
            try
            {
                _listener.OnAssemblyEnd(assembly.GetInfo());
            }
            catch (Exception err)
            {
                _listener.OnFatalError(new ExceptionResult(err));
            }
        }

        public void StartExplicitRunScope(Assembly assembly)
        {
            _assemblyStart = x => { };
            _assemblyEnd = x => { };

            OnAssemblyStart(assembly);
        }

        public void EndExplicitRunScope(Assembly assembly)
        {
            OnAssemblyEnd(assembly);
        }

        void RunContext(Context context,
                        IEnumerable<ICleanupAfterEveryContextInAssembly> globalCleanups,
                        IEnumerable<ISupplementSpecificationResults> supplements)
        {
            var runner = ContextRunnerFactory.GetContextRunnerFor(context);
            runner.Run(context, _listener, _options, globalCleanups, supplements);
        }
    }
}