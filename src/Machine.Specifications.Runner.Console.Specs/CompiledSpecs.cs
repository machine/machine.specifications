namespace Machine.Specifications.ConsoleRunner.Specs
{
    public abstract class CompiledSpecs : ConsoleRunnerSpecs
    {
        protected static CompileContext compiler;

        Establish context = () =>
            compiler = new CompileContext();

        Cleanup after = () =>
            compiler.Dispose();
    }
}
