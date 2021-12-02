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
        private readonly AggregateRunListener listener;

        private readonly RunOptions options;

        private readonly IList<IAssemblyContext> executedAssemblyContexts = new List<IAssemblyContext>();

        private readonly AssemblyExplorer explorer = new AssemblyExplorer();

        private Action<Assembly> assemblyStart;

        private Action<Assembly> assemblyEnd;

        public AssemblyRunner(ISpecificationRunListener listener, RunOptions options)
        {
            var state = new RedirectOutputState();
            this.options = options;

            this.listener = new AggregateRunListener(new[]
            {
                new AssemblyLocationAwareListener(),
                new SetUpRedirectOutputRunListener(state),
                listener,
                new TearDownRedirectOutputRunListener(state),
            });

            assemblyStart = OnAssemblyStart;
            assemblyEnd = OnAssemblyEnd;
        }

        public void Run(Assembly assembly, IEnumerable<Context> contexts)
        {
            var hasExecutableSpecifications = false;

            try
            {
                var globalCleanups = explorer.FindAssemblyWideContextCleanupsIn(assembly).ToList();
                var specificationSupplements = explorer.FindSpecificationSupplementsIn(assembly).ToList();

                foreach (var context in contexts)
                {
                    if (!hasExecutableSpecifications)
                    {
                        assemblyStart(assembly);
                        hasExecutableSpecifications = true;
                    }

                    RunContext(context, globalCleanups, specificationSupplements);
                }
            }
            catch (Exception err)
            {
                listener.OnFatalError(new ExceptionResult(err));
            }
            finally
            {
                if (hasExecutableSpecifications)
                {
                    assemblyEnd(assembly);
                }
            }
        }

        private void OnAssemblyStart(Assembly assembly)
        {
            try
            {
                listener.OnAssemblyStart(assembly.GetInfo());

                var assemblyContexts = explorer.FindAssemblyContextsIn(assembly);

                assemblyContexts.Each(assemblyContext =>
                {
                    assemblyContext.OnAssemblyStart();
                    executedAssemblyContexts.Add(assemblyContext);
                });
            }
            catch (Exception err)
            {
                listener.OnFatalError(new ExceptionResult(err));
            }
        }

        private void OnAssemblyEnd(Assembly assembly)
        {
            try
            {
                listener.OnAssemblyEnd(assembly.GetInfo());
                executedAssemblyContexts
                    .Reverse()
                    .Each(assemblyContext => assemblyContext.OnAssemblyComplete());
            }
            catch (Exception err)
            {
                listener.OnFatalError(new ExceptionResult(err));
            }
        }

        public void StartExplicitRunScope(Assembly assembly)
        {
            assemblyStart = x => { };
            assemblyEnd = x => { };

            OnAssemblyStart(assembly);
        }

        public void EndExplicitRunScope(Assembly assembly)
        {
            OnAssemblyEnd(assembly);
        }

        private void RunContext(
            Context context,
            IEnumerable<ICleanupAfterEveryContextInAssembly> globalCleanups,
            IEnumerable<ISupplementSpecificationResults> supplements)
        {
            var runner = ContextRunnerFactory.GetContextRunnerFor(context);
            runner.Run(context, listener, options, globalCleanups, supplements);
        }
    }
}
