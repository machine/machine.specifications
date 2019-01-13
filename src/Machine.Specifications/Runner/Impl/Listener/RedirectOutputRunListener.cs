using Machine.Specifications.Runner.Impl.Listener.Redirection;

namespace Machine.Specifications.Runner.Impl.Listener
{
    internal class RedirectOutputState
    {
        OutputInterceptor _assemblyOutput;
        OutputInterceptor _contextOutput;
        OutputInterceptor _specificationOutput;

        public void SetUpAssembly()
        {
            _assemblyOutput = new OutputInterceptor();
        }

        public void TearDownAssembly(AssemblyInfo assembly)
        {
            _assemblyOutput.Dispose();
            assembly.CapturedOutput = _assemblyOutput.CapturedOutput;
        }

        public void SetUpContext()
        {
            _contextOutput = new OutputInterceptor();
        }

        public void TearDownContext(ContextInfo context)
        {
            _contextOutput.Dispose();
            context.CapturedOutput = _contextOutput.CapturedOutput;
        }

        public void SetUpSpecification()
        {
            _specificationOutput = new OutputInterceptor();
        }

        public void TearDownSpecification(SpecificationInfo specification)
        {
            _specificationOutput.Dispose();
            specification.CapturedOutput = _specificationOutput.CapturedOutput;
        }
    }

    internal class SetUpRedirectOutputRunListener : RunListenerBase
    {
        readonly RedirectOutputState _state;

        public SetUpRedirectOutputRunListener(RedirectOutputState state)
        {
            _state = state;
        }

        public override void OnAssemblyStart(AssemblyInfo assembly)
        {
            _state.SetUpAssembly();
        }

        public override void OnContextStart(ContextInfo context)
        {
            _state.SetUpContext();
        }

        public override void OnSpecificationStart(SpecificationInfo specification)
        {
            _state.SetUpSpecification();
        }
    }

    class TearDownRedirectOutputRunListener : RunListenerBase
    {
        readonly RedirectOutputState _state;

        public TearDownRedirectOutputRunListener(RedirectOutputState state)
        {
            _state = state;
        }

        public override void OnAssemblyEnd(AssemblyInfo assembly)
        {
            _state.TearDownAssembly(assembly);
        }

        public override void OnContextEnd(ContextInfo context)
        {
            _state.TearDownContext(context);
        }

        public override void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            _state.TearDownSpecification(specification);
        }
    }
}