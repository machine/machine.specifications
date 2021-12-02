using Machine.Specifications.Runner.Impl.Listener.Redirection;

namespace Machine.Specifications.Runner.Impl.Listener
{
    internal class RedirectOutputState
    {
        private OutputInterceptor assemblyOutput;

        private OutputInterceptor contextOutput;

        private OutputInterceptor specificationOutput;

        public void SetUpAssembly()
        {
            assemblyOutput = new OutputInterceptor();
        }

        public void TearDownAssembly(AssemblyInfo assembly)
        {
            assemblyOutput.Dispose();

            assembly.CapturedOutput = assemblyOutput.CapturedOutput;
        }

        public void SetUpContext()
        {
            contextOutput = new OutputInterceptor();
        }

        public void TearDownContext(ContextInfo context)
        {
            contextOutput.Dispose();

            context.CapturedOutput = contextOutput.CapturedOutput;
        }

        public void SetUpSpecification()
        {
            specificationOutput = new OutputInterceptor();
        }

        public void TearDownSpecification(SpecificationInfo specification)
        {
            specificationOutput.Dispose();

            specification.CapturedOutput = specificationOutput.CapturedOutput;
        }
    }
}
