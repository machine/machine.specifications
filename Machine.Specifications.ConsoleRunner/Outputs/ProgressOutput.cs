using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.ConsoleRunner.Outputs
{
    class ProgressOutput : IOutput
    {
        readonly IConsole _console;

        public ProgressOutput(IConsole console)
        {
            _console = console;
        }

        public void RunStart()
        {
        }

        public void RunEnd()
        {
        }

        public void AssemblyStart(AssemblyInfo assembly)
        {
            EmptyLine();
            _console.WriteLine("Specs in " + assembly.Name + ":");
        }

        public void AssemblyEnd(AssemblyInfo assembly)
        {
            EmptyLine();
        }

        public void ContextStart(ContextInfo context)
        {
        }

        public void ContextEnd(ContextInfo context)
        {
        }

        public void SpecificationStart(SpecificationInfo specification)
        {
        }

        public void Passing(SpecificationInfo specification)
        {
            _console.Write(".");
        }

        public void NotImplemented(SpecificationInfo specification)
        {
            _console.Write("*");
        }

        public void Ignored(SpecificationInfo specification)
        {
            _console.Write("I");
        }

        public void Failed(SpecificationInfo specification, Result result)
        {
            _console.Write("F");
        }

        void EmptyLine()
        {
            _console.WriteLine("");
        }
    }
}