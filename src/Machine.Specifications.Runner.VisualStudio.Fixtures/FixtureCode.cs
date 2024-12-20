using System;
using System.Collections.Generic;
using Machine.Specifications;

namespace SampleSpecs
{
    [Behaviors]
    public class SampleBehavior
    {
        It sample_behavior_test = () =>
            true.ShouldBeTrue();
    }

    class BehaviorSampleSpec
    {
        Because of = () => { };

        Behaves_like<SampleBehavior> some_behavior;
    }

    class CleanupSpec
    {
        static int cleanup_count;

        Because of = () => { };

        Cleanup after = () =>
            cleanup_count++;

        It should_not_increment_cleanup = () =>
            cleanup_count.ShouldEqual(0);

        It should_have_no_cleanups = () =>
            cleanup_count.ShouldEqual(0);
    }

    [AssertDelegate]
    public delegate void They();

    [ActDelegate]
    public delegate void WhenDoing();

    class CustomActAssertDelegateSpec
    {
        static string a;
        static string b;

        static int resultA;
        static int resultB;

        Establish context = () =>
        {
            a = "foo";
            b = "foo";
        };

        WhenDoing of = () =>
        {
            resultA = a.GetHashCode();
            resultB = b.GetHashCode();
        };

        They should_have_the_same_hash_code = () => resultA.ShouldEqual(resultB);
    }

    class Parent
    {
        class NestedSpec
        {
            It should_remember_that_true_is_true = () =>
                true.ShouldBeTrue();
        }
    }

    class StandardSpec
    {
        Because of = () => { };

        It should_pass = () =>
            1.ShouldEqual(1);

        [Ignore("reason")]
        It should_be_ignored = () => { };

        It should_fail = () =>
            1.ShouldEqual(2);

        It unhandled_exception = () =>
        {
            throw new NotImplementedException();
        };

        It not_implemented;
    }

    class When_something
    {
        Because of = () => { };

        It should_pass = () =>
            1.ShouldEqual(1);
    }
}
