namespace Machine.Specifications.Runner.Impl.Listener
{
    internal class SetUpRedirectOutputRunListener : RunListenerBase
    {
        private readonly RedirectOutputState state;

        public SetUpRedirectOutputRunListener(RedirectOutputState state)
        {
            this.state = state;
        }

        public override void OnAssemblyStart(AssemblyInfo assembly)
        {
            state.SetUpAssembly();
        }

        public override void OnContextStart(ContextInfo context)
        {
            state.SetUpContext();
        }

        public override void OnSpecificationStart(SpecificationInfo specification)
        {
            state.SetUpSpecification();
        }
    }
}
