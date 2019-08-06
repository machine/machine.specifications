using System;

namespace Machine.Specifications
{
    namespace ExampleA
    {
        namespace ExampleB
        {
            public class InExampleB_1
            {
                It is_spec_1 = () => { };
            }

            public class InExampleB_2
            {
                It is_spec_1 = () => { };
            }
        }

        public class InExampleA_1
        {
            It is_spec_1 = () => { };
        }

        public class InExampleA_2
        {
            It is_spec_1 = () => { };
        }
    }

    namespace ExampleC
    {
        public class InExampleC_1
        {
            It is_spec_1 = () => { };
            It is_spec_2 = () => { };
        }

        public class InExampleC_2
        {
        }
    }

    public interface IFakeContext
    {
        void Reset();
    }

    public class ContextWithSpecificationExpectingThrowThatDoesnt : IFakeContext
    {
        public static bool it_invoked;
        static Exception exception;

        Because of = () =>
            exception = null;

        It should_throw_but_it_wont = () =>
            exception.ShouldNotBeNull();

        public void Reset()
        {
            it_invoked = false;
        }
    }

    public class ContextWithThrowingWhenAndPassingSpecification : IFakeContext
    {
        public static bool it_invoked;

        Because of = () =>
            throw new Exception();

        It should_fail = () =>
            it_invoked = true;

        public void Reset()
        {
            it_invoked = false;
        }
    }

    public class ContextWithEmptyWhen : IFakeContext
    {
        public static bool it_invoked;

        Because of;

        It should_do_stuff = () =>
            it_invoked = true;

        public void Reset()
        {
            it_invoked = false;
        }
    }

    public class ContextWithTwoWhens : IFakeContext
    {
        public static bool when_1_invoked;
        public static bool when_2_invoked;
        public static bool it_for_when_1_invoked;
        public static bool it_for_when_2_invoked;

        Because _1 = () =>
            when_1_invoked = true;

        It for_when_1 = () =>
            it_for_when_1_invoked = true;

        Because _2 = () =>
            when_2_invoked = true;

        It for_when_2 = () =>
            it_for_when_2_invoked = true;

        public void Reset()
        {
            when_1_invoked = false;
            when_2_invoked = false;
            it_for_when_1_invoked = false;
            it_for_when_2_invoked = false;
        }
    }

    public class ContextWithEmptySpecification : IFakeContext
    {
        public static bool when_invoked;

        Because of = () =>
            when_invoked = true;

        It should_do_stuff;

        public void Reset()
        {
            when_invoked = false;
        }
    }

    public class ContextWithThrowingSpecification : IFakeContext
    {
        public static bool when_invoked;
        public static bool it_invoked;
        public static Exception exception;

        Because it_happens = () =>
        {
            when_invoked = true;
            exception = Catch.Exception(() => throw new Exception());
        };

        It should_throw_an_exception = () =>
            it_invoked = true;

        public void Reset()
        {
            when_invoked = false;
            it_invoked = false;
        }
    }

    public class ContextWithSingleSpecification : IFakeContext
    {
        public static bool because_invoked;
        public static bool it_invoked;
        public static bool context_invoked;
        public static bool cleanup_invoked;

        Establish context = () =>
            context_invoked = true;

        Because of = () =>
            because_invoked = true;

        It is_a_specification = () =>
            it_invoked = true;

        Cleanup after = () =>
            cleanup_invoked = true;

        public void Reset()
        {
            because_invoked = false;
            it_invoked = false;
            context_invoked = false;
            cleanup_invoked = false;
        }
    }
}
