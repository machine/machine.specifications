using System;

namespace Machine.Specifications.Specs
{
    [Subject(typeof(Catch))]
    public class when_calling_Catch_Exception_with_an_Action
    {
        [Subject(typeof(Catch))]
        public class with_a_throwing_Action
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
        public class with_a_non_throwing_Action
        {
            static string action_side_effect;
            static Exception result;

            Because of = () =>
                result = Catch.Exception(() => action_side_effect = "hi");

            It should_access_the_propety = () =>
                action_side_effect.ShouldEqual("hi");

            It should_return_null =
                () => result.ShouldBeNull();
        }
    }

    [Subject(typeof(Catch))]
    public class when_calling_Catch_Exception_with_a_Func
    {
        class Dummy
        {
            public static readonly ArgumentException AnException = new ArgumentException();

            public static string ThrowingProperty => throw AnException;

            public static string NonThrowingProperty => "hi";
        }

        [Subject(typeof(Catch))]
        public class with_a_throwing_Func
        {
            static Exception result;

            Because of = () =>
                result = Catch.Exception(() => Dummy.ThrowingProperty);

            It should_return_the_same_exception = () =>
                result.ShouldBeTheSameAs(Dummy.AnException);
        }

        [Subject(typeof(Catch))]
        public class with_a_non_throwing_Func
        {
            static Exception result;
            static string property_value;

            Because of = () =>
                result = Catch.Exception(() => property_value = Dummy.NonThrowingProperty);

            It should_access_the_propety = () =>
                property_value.ShouldEqual("hi");

            It should_return_null = () =>
                result.ShouldBeNull();
        }
    }

    [Subject(typeof(Catch))]
    public class when_calling_Catch_Only_with_an_Action
    {
        [Subject(typeof(Catch))]
        public class with_a_throwing_Action_which_matches_exception_to_be_caught
        {
            static ArgumentException an_exception;
            static Exception result;

            Establish context = () =>
                an_exception = new ArgumentException();

            Because of = () =>
                result = Catch.Only<ArgumentException>(() => throw an_exception);

            It should_return_the_same_exception = () =>
                result.ShouldBeTheSameAs(an_exception);
        }

        [Subject(typeof(Catch))]
        public class with_a_throwing_Action_which_doesnt_match_exception_to_be_caught
        {
            static ArgumentException an_exception;
            static Exception result;

            Establish context = () =>
                an_exception = new ArgumentException();

            Because of = () =>
                result = Catch.Exception(() => Catch.Only<InvalidOperationException>(() => throw an_exception));

            It should_return_the_same_exception = () =>
                result.ShouldBeTheSameAs(an_exception);
        }

        [Subject(typeof(Catch))]
        public class with_a_non_throwing_Action
        {
            static string action_side_effect;
            static Exception result;

            Because of = () =>
                result = Catch.Only<ArgumentException>(() => action_side_effect = "hi");

            It should_access_the_propety = () =>
                action_side_effect.ShouldEqual("hi");

            It should_return_null = () =>
                result.ShouldBeNull();
        }
    }
}
