namespace Machine.Specifications.ConsoleRunner.Specs
{
    public class ExternalFileSpecs : CompiledSpecs
    {
        const string Code = @"
using System.Configuration;
using System.IO;
using Machine.Specifications;

namespace Example.UsingExternalFile
{
    [Subject(""External resources usage"")]
    public class when_using_test_assembly_configuration_file
    {
        It should_be_able_to_read_application_settings = () =>
            ConfigurationManager.AppSettings[""key""].ShouldEqual(""value"");
    }

    [Subject(""External resources usage"")]
    public class when_using_file_copied_to_assembly_output_directory
    {
        It should_be_able_to_locate_it_by_relative_path = () =>
            File.Exists(""TestData.txt"").ShouldBeTrue();
    }
}";

        protected static string path;

        Establish context = () =>
            path = compiler.Compile(Code, "System.dll", "System.Configuration.dll");
    }
}
