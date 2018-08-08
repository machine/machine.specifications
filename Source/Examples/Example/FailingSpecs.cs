using System;
using Machine.Specifications;

namespace Example
{
    [Subject("example")]
    public class when_context_with_passing_and_failing_clauses
    {
        It should_fail = () => throw new Exception();

        It should_pass = () => { };
    }
}