namespace Machine.Specifications.ConsoleRunner.Specs
{
    public class FailingSpecs : CompiledSpecs
    {
        public const string Code = @"
using System;
using Machine.Specifications;

namespace Example.Failing
{
    [SetupDelegate]
    public delegate void Given();

    [Subject(""Scott Bellware"")]
    public class at_any_given_moment
    {
        It will_fail = () => { throw new Exception(""hi scott, love you, miss you.""); };
    }

    [Tags(""example"")]
    public class context_with_multiple_establish_clauses
    {
        Establish foo = () => { };
        Establish bar = () => { };

        It should = () => { };
    }

    [Tags(""example"")]
    public class context_with_multiple_given_clauses
    {
        Given foo = () => { };
        Given bar = () => { };

        It should = () => { };
    }
}";

        protected static string path;

        Establish context = () =>
            path = compiler.Compile(Code);
    }
}
