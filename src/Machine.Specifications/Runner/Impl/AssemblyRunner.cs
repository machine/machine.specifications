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
        readonly TestContextListener _testContext;
        readonly List<ITestContext> _testContexts;

        public AssemblyRunner(ISpecificationRunListener listener, RunOptions options)
        {
            var state = new RedirectOutputState();
            _testContext = new TestContextListener();
            _listener = new AggregateRunListener(new[]
                                           {
                                             new AssemblyLocationAwareListener(),
                                             new TestContextListener(),
                                             new SetUpRedirectOutputRunListener(state),
                                             listener,
                                             new TearDownRedirectOutputRunListener(state),
                                           });
            _options = options;
            _explorer = new AssemblyExplorer();
            _executedAssemblyContexts = new List<IAssemblyContext>();
            _testContexts = new List<ITestContext>();

            _assemblyStart = OnAssemblyStart;
            _assemblyEnd = OnAssemblyEnd;
        }

        public void Run(Assembly assembly, IEnumerable<Context> contexts)
        {
            var hasExecutableSpecifications = false;

            try
            {
                var contextsList = contexts.ToList();
                hasExecutableSpecifications = contextsList.Any(x => x.HasExecutableSpecifications);

                var globalCleanups = _explorer.FindAssemblyWideContextCleanupsIn(assembly).ToList();
                var specificationSupplements = _explorer.FindSpecificationSupplementsIn(assembly).ToList();
                _testContext.SetTestContexts(_testContexts);
                if (hasExecutableSpecifications)
                {
                    _assemblyStart(assembly);
                }

                foreach (var context in contextsList)
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

                var assemblyContexts = _explorer.FindAssemblyContextsIn(assembly);
                assemblyContexts.Each(assemblyContext =>
                {
                    assemblyContext.OnAssemblyStart();
                    _executedAssemblyContexts.Add(assemblyContext);
                });

                var testContexts = _explorer.FindTestContextsIn(assembly);
                testContexts.Each(testContext =>
                {
                    _testContexts.Add(testContext);
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
