namespace Machine.Specifications.Runner.Impl.Listener
{
    internal class TearDownRedirectOutputRunListener : RunListenerBase
    {
        private readonly RedirectOutputState state;

        public TearDownRedirectOutputRunListener(RedirectOutputState state)
        {
            this.state = state;
        }

        public override void OnAssemblyEnd(AssemblyInfo assembly)
        {
            state.TearDownAssembly(assembly);
        }

        public override void OnContextEnd(ContextInfo context)
        {
            state.TearDownContext(context);
        }

        public override void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            state.TearDownSpecification(specification);
        }
    }
}
