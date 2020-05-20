using System.Collections.Generic;

namespace Machine.Specifications.Runner.Impl
{
    public class TestContextListener : RunListenerBase
    {
        List<ITestContext> _testContext;

        public void SetTestContexts(List<ITestContext> testContexts)
        {
            _testContext = testContexts;
        }

        public override void OnAssemblyStart(AssemblyInfo assembly)
        {
            if (_testContext == null) return;
            foreach (var testContext in _testContext)
            {
                testContext.OnAssemblyStart(assembly);
            }
        }

        public override void OnAssemblyEnd(AssemblyInfo assembly)
        {
            if (_testContext == null) return;
            foreach (var testContext in _testContext)
            {
                testContext.OnAssemblyEnd(assembly);
            }
        }

        public override void OnRunStart()
        {
            if (_testContext == null) return;
            foreach (var testContext in _testContext)
            {
                testContext.OnRunStart();
            }
        }

        public override void OnRunEnd()
        {
            if (_testContext == null) return;
            foreach (var testContext in _testContext)
            {
                testContext.OnRunEnd();
            }
        }

        public override void OnContextStart(ContextInfo context)
        {
            foreach (var testContext in _testContext)
            {
                testContext.OnContextStart(context);
            }
        }

        public override void OnContextEnd(ContextInfo context)
        {
            if (_testContext == null) return;
            foreach (var testContext in _testContext)
            {
                testContext.OnContextEnd(context);
            }
        }

        public override void OnSpecificationStart(SpecificationInfo specification)
        {
            if (_testContext == null) return;
            foreach (var testContext in _testContext)
            {
                testContext.OnSpecificationStart(specification);
            }
        }

        public override void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            if (_testContext == null) return;
            foreach (var testContext in _testContext)
            {
                testContext.OnSpecificationEnd(specification, result);
            }
        }

        public override void OnFatalError(ExceptionResult exception)
        {
            if (_testContext == null) return;
            foreach (var testContext in _testContext)
            {
                try
                {
                    testContext.OnFatalError(exception);
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}
