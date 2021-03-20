using System.Collections.Generic;

namespace Machine.Specifications.Runner.Impl
{
    public class TestContextListener : RunListenerBase
    {
        List<ITestContext> _testContext = new List<ITestContext>();

        public void SetTestContexts(List<ITestContext> testContexts)
        {
            _testContext = testContexts ?? new List<ITestContext>();
        }

        public override void OnAssemblyStart(AssemblyInfo assembly)
        {
            foreach (var testContext in _testContext)
            {
                testContext.OnAssemblyStart(assembly);
            }
        }

        public override void OnAssemblyEnd(AssemblyInfo assembly)
        {
            foreach (var testContext in _testContext)
            {
                testContext.OnAssemblyEnd(assembly);
            }
        }

        public override void OnRunStart()
        {
            foreach (var testContext in _testContext)
            {
                testContext.OnRunStart();
            }
        }

        public override void OnRunEnd()
        {
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
            foreach (var testContext in _testContext)
            {
                testContext.OnContextEnd(context);
            }
        }

        public override void OnSpecificationStart(SpecificationInfo specification)
        {
            foreach (var testContext in _testContext)
            {
                testContext.OnSpecificationStart(specification);
            }
        }

        public override void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            foreach (var testContext in _testContext)
            {
                testContext.OnSpecificationEnd(specification, result);
            }
        }

        public override void OnFatalError(ExceptionResult exception)
        {
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
