namespace Machine.Specifications.ConsoleRunner.Specs
{
    public class FailAndSuccessSpecs : CompiledSpecs
    {
        const string FailCode = @"
using System;
using Machine.Specifications;

namespace Example.Issue157_Fail
{
    public class when_a_spec_fails
    {
        It will_fail = () => { throw new Exception(); };
    }
}";

        const string SucessCode = @"
using Machine.Specifications;

namespace Example.Issue157_Success
{
    public class when_a_spec_succeeds
    {
        It will_succeed = () => { };
    }
}";

        protected static string fail_path;
        protected static string success_path;

        Establish context = () =>
        {
            fail_path = compiler.Compile(FailCode);
            success_path = compiler.Compile(SucessCode);
        };
    }
}
