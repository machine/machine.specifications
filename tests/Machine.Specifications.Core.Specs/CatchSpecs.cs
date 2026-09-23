using System;
using System.Threading.Tasks;

namespace Machine.Specifications.Specs
{
    [Subject(typeof(Catch))]
    class when_calling_catch_exception_with_an_action
    {
        [Subject(typeof(Catch))]
        class with_a_throwing_action
        {
            static ArgumentException an_exception;

            static Exception result;

            Establish context = () =>
                an_exception = new ArgumentException();

            Because of = () =>
                result = Catch.Exception(() => throw an_exception);

            It should_return_the_same_exception = () =>
                result.ShouldBeTheSameAs(an_exception);
        }

        [Subject(typeof(Catch))]
        class with_a_non_throwing_action
        {
            static string action_side_effect;

            static Exception result;

            Because of = () =>
                result = Catch.Exception(() => action_side_effect = "hi");

            It should_access_the_propety = () =>
                action_side_effect.ShouldEqual("hi");

            It should_return_null = () =>
                result.ShouldBeNull();
        }
    }

    [Subject(typeof(Catch))]
    class when_calling_catch_exception_with_a_func
    {
        static ArgumentException an_exception = new ArgumentException();

        static string ThrowingProperty => throw an_exception;

        static string NonThrowingProperty => "hi";

        [Subject(typeof(Catch))]
        class with_a_throwing_func
        {
            static Exception result;

            Because of = () =>
                result = Catch.Exception(() => ThrowingProperty);

            It should_return_the_same_exception = () =>
                result.ShouldBeTheSameAs(an_exception);
        }

        [Subject(typeof(Catch))]
        class with_a_non_throwing_func
        {
            static Exception result;

            static string property_value;

            Because of = () =>
                result = Catch.Exception(() => property_value = NonThrowingProperty);

            It should_access_the_propety = () =>
                property_value.ShouldEqual("hi");

            It should_return_null = () =>
                result.ShouldBeNull();
        }
    }

    [Subject(typeof(Catch))]
    class when_calling_catch_with_async_methods
    {
        static Exception exception;

        [Subject(typeof(Catch))]
        class with_a_non_throwing_action
        {
            static Task Test() => Task.Run(() => { });

            Because of = async () =>
                exception = await Catch.ExceptionAsync(Test);

            It should_return_null = () =>
                exception.ShouldBeNull();
        }

        [Subject(typeof(Catch))]
        class with_a_throwing_action
        {
            static Task Test() => Task.Run(() => throw new ArgumentNullException());

            Because of = async () =>
                exception = await Catch.ExceptionAsync(Test);

            It should_return_exception = () =>
                exception.ShouldBeOfExactType<ArgumentNullException>();
        }

        [Subject(typeof(Catch))]
        class calling_wrong_catch_method
        {
            static Task Test() => Task.Run(() => throw new ArgumentNullException());

            Because of = () =>
                exception = Catch.Exception(() => Catch.Exception(Test));

            It should_return_exception = () =>
                exception.ShouldBeOfExactType<InvalidOperationException>();

            It should_contain_message = () =>
                exception.Message.ShouldEqual("You must use Catch.ExceptionAsync for async methods");
        }

        [Subject(typeof(Catch))]
        class using_value_task
        {
            static ValueTask Test() => throw new ArgumentNullException();

            Because of = async () =>
                exception = await Catch.ExceptionAsync(Test);

            It should_return_exception = () =>
                exception.ShouldBeOfExactType<ArgumentNullException>();
        }

        [Subject(typeof(Catch))]
        class using_value_task_that_works
        {
            static string result;

            static ValueTask Test()
            {
                result = "done";

                return new ValueTask(Task.Run(() => { }));
            }

            Because of = async () =>
                exception = await Catch.ExceptionAsync(Test);

            It should_complete = () =>
                result.ShouldEqual("done");
        }

        [Subject(typeof(Catch))]
        class using_generic_value_task
        {
            static ValueTask<int> Test() => throw new ArgumentNullException();

            Because of = async () =>
                exception = await Catch.ExceptionAsync(Test);

            It should_return_exception = () =>
                exception.ShouldBeOfExactType<ArgumentNullException>();
        }

        [Subject(typeof(Catch))]
        class using_generic_value_task_that_works
        {
            static string result;

            static ValueTask<int> Test()
            {
                result = "done";

                return new ValueTask<int>(4);
            }

            Because of = async () =>
                exception = await Catch.ExceptionAsync(Test);

            It should_complete = () =>
                result.ShouldEqual("done");
        }
    }
}
