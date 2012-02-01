using System;
using System.Threading;
using System.Threading.Tasks;

namespace Machine.Specifications.Tools4.Specs
{
    internal class TaskSpecificationClass
    {
        const int WaitTime = 300;

        public Task<string> DoItWithAGenericTask(string foo)
        {
            return Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(WaitTime);
                    if (foo == "ex")
                    {
                        throw new InvalidOperationException("ex is not allowed");
                    }
                    return foo + "Async";
                });
        }

        public Task DoItWithATask(string foo)
        {
            return Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(WaitTime);
                    if (foo == "ex")
                    {
                        throw new InvalidProgramException("ex is not allowed");
                    }
                });
        }


        public Task DoItWithWithTwoTask(string foo)
        {
            return Task.Factory.StartNew(() => Task.WaitAll(DoItWithAGenericTask(foo), DoItWithATask(foo)));
        }

    }

    [Subject(typeof (TaskSpecificationExtensions))]
    public class When_a_task_of_t_is_executed
    {
        static TaskSpecificationClass test;

        Establish context = () => { test = new TaskSpecificationClass(); };

        Because of = () => result = test.DoItWithAGenericTask("foo").Await();

        It should_have_the_right_result = () => result.ShouldEqual("fooAsync");

        static string result;
    }

    [Subject(typeof(TaskSpecificationExtensions))]
    public class When_a_task_of_t_is_executed_and_throws_a_exception
    {
        static TaskSpecificationClass test;

        Establish context = () => { test = new TaskSpecificationClass(); };

        Because of = () => result = Catch.Exception(() => test.DoItWithAGenericTask("ex").Await());

        It should_catch_the_thrown_exception = () => result.ShouldBeOfType<InvalidOperationException>();

        static Exception result;
    }

    [Subject(typeof(TaskSpecificationExtensions))]
    public class When_a_task_is_executed_and_no_exception_is_thrown
    {
        static TaskSpecificationClass test;

        Establish context = () => { test = new TaskSpecificationClass(); };

        Because of = () => result = Catch.Exception(() => test.DoItWithATask("foo").Await());

        It should_not_has_any_exception_as_result = () => result.ShouldBeNull();

        static Exception result;
    }

    [Subject(typeof(TaskSpecificationExtensions))]
    public class When_a_task_is_executed_and_throws_a_exception
    {
        static TaskSpecificationClass test;

        Establish context = () => { test = new TaskSpecificationClass(); };

        Because of = () => result = Catch.Exception(() => test.DoItWithATask("ex").Await());

        It should_catch_the_thrown_exception = () => result.ShouldBeOfType<InvalidProgramException>();

        static Exception result;
    }

    [Subject(typeof(TaskSpecificationExtensions))]
    public class When_a_task_with_both_task_are_executed_with_an_exception
    {
        static TaskSpecificationClass test;

        Establish context = () => { test = new TaskSpecificationClass(); };

        Because of = () => result = Catch.Exception(() => test.DoItWithWithTwoTask("ex").Await());

        It should_catch_the_thrown_exception = () => result.ShouldBeOfType<AggregateException>();

        It should_two_innerexceptions = () => ((AggregateException) result).InnerExceptions.Count.ShouldEqual(2);

        static Exception result;
    }

}