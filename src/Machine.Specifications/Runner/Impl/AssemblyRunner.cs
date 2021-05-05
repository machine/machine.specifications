using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Machine.Specifications.Explorers;
using Machine.Specifications.Model;
using Machine.Specifications.Runner.Impl.Listener;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Runner.Impl
{
    internal class AssemblyRunner
    {
        readonly AggregateRunListener _listener;
        readonly RunOptions _options;
        Action<Assembly> _assemblyStart;
        Action<Assembly> _assemblyEnd;

        readonly IList<IAssemblyContext> _executedAssemblyContexts;
        readonly AssemblyExplorer _explorer;

        public AssemblyRunner(ISpecificationRunListener listener, RunOptions options)
        {
            RedirectOutputState state = new RedirectOutputState();
            _listener = new AggregateRunListener(new[]
                                           {
                                             new AssemblyLocationAwareListener(),
                                             new SetUpRedirectOutputRunListener(state),
                                             listener,
                                             new TearDownRedirectOutputRunListener(state),
                                           });
            _options = options;
            _explorer = new AssemblyExplorer();
            _executedAssemblyContexts = new List<IAssemblyContext>();

            _assemblyStart = OnAssemblyStart;
            _assemblyEnd = OnAssemblyEnd;
        }

        public void Run(Assembly assembly, IEnumerable<Context> contexts)
        {
            var hasExecutableSpecifications = false;

            try
            {
                var globalCleanups = _explorer.FindAssemblyWideContextCleanupsIn(assembly).ToList();
                var specificationSupplements = _explorer.FindSpecificationSupplementsIn(assembly).ToList();

                foreach (var context in contexts)
                {
                    if (!hasExecutableSpecifications)
                    {
                        _assemblyStart(assembly);
                        hasExecutableSpecifications = true;
                    }

                    RunContext(context, globalCleanups, specificationSupplements);
                }
            }
            catch (Exception err)
            {
                _listener.OnFatalError(new ExceptionResult(err));
            }
            finally
            {
                if (hasExecutableSpecifications)
                {
                    _assemblyEnd(assembly);
                }

            }
        }

        void OnAssemblyStart(Assembly assembly)
        {
            try
            {
                _listener.OnAssemblyStart(assembly.GetInfo());

                IEnumerable<IAssemblyContext> assemblyContexts = _explorer.FindAssemblyContextsIn(assembly);
                assemblyContexts.Each(assemblyContext =>
                {
                    assemblyContext.OnAssemblyStart();
                    _executedAssemblyContexts.Add(assemblyContext);
                });
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
                _executedAssemblyContexts
                    .Reverse()
                    .Each(assemblyContext => assemblyContext.OnAssemblyComplete());

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
