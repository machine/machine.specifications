namespace Machine.Specifications.ConsoleRunner.Specs
{
    public class ExampleAndFailingSpecs : CompiledSpecs
    {
        protected static string example_path;
        protected static string failing_path;

        Establish context = () =>
        {
            example_path = compiler.Compile(ExampleSpecs.Code);
            failing_path = compiler.Compile(FailingSpecs.Code);
        };
    }
}
