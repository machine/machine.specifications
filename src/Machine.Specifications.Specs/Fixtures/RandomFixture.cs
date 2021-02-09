namespace Machine.Specifications.Specs.Fixtures
{
    public static class RandomFixture
    {
        public static string Code = @"
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Example.Random
{
    public class TestAssemblyContext : IAssemblyContext
    {
        public static bool OnAssemblyStartRun;
        public static bool OnAssemblyCompleteRun;

        public void OnAssemblyStart()
        {
            OnAssemblyStartRun = true;
        }

        public void OnAssemblyComplete()
        {
            OnAssemblyCompleteRun = true;
        }
    }

    public static class StaticContainer
    {
        static readonly bool Foo;

        static StaticContainer()
        {
            Foo = true;
        }

        public class when_a_context_is_nested_inside_a_static_class
        {
            It should_be_run = () =>
                Foo.ShouldBeTrue();
        }
    }

    public class NonStaticContainer
    {
        static readonly bool Bar;

        static NonStaticContainer()
        {
            Bar = true;
        }

        public static class StaticContainer
        {
            static readonly bool Foo;

            static StaticContainer()
            {
                Foo = true;
            }

            public class when_a_context_is_nested_inside_a_static_class_that_is_nested_inside_a_class
            {
                It should_be_run = () =>
                {
                    Foo.ShouldBeTrue();
                    Bar.ShouldBeTrue();
                };
            }
        }
    }

    public static class tag
    {
        public const string example = ""example"";
        public const string some_other_tag = ""some other tag"";
        public const string one_more_tag = ""one more tag"";
    }

    [SetupForEachSpecification, Tags(tag.example)]
    public class context_with_multiple_specifications_and_setup_for_each
    {
        public static int establish_run_count;
        public static int because_clause_run_count;

        Establish context = () =>
            establish_run_count++;

        Because of = () =>
            because_clause_run_count++;

        It spec1 = () => { };

        It spec2 = () => { };
    }

    [Tags(tag.example, ""foobar"")]
    public class context_with_multiple_specifications
    {
        public static int establish_run_count;
        public static int because_clause_run_count;

        Establish context = () =>
            establish_run_count++;

        Because of = () =>
            because_clause_run_count++;

        It spec1 = () => { };

        It spec2 = () => { };
    }

    [Tags(tag.example, tag.example)]
    [Tags(tag.example)]
    public class context_with_duplicate_tags
    {
        It bla_bla = () => { };
    }

    [Tags(tag.example, tag.some_other_tag, tag.one_more_tag)]
    public class context_with_tags
    {
        It bla_bla = () => { };
    }

    public class context_with_unimplemented_specs
    {
        It should_be_unimplemented;
    }

    [Ignore(""example reason"")]
    public class context_with_ignore : context_with_no_specs
    {
        public static bool ignored_spec_ran;

        It should_be_ignored = () =>
            ignored_spec_ran = true;
    }

    public class context_with_ignore_on_one_spec : context_with_no_specs
    {
        public static bool ignored_spec_ran;

        [Ignore(""example reason"")]
        It should_be_ignored = () =>
            ignored_spec_ran = true;
    }

    [Ignore(""example reason"")]
    public class context_with_ignore_and_reason : context_with_no_specs
    {
        public static bool ignored_spec_ran;

        It should_be_ignored = () =>
            ignored_spec_ran = true;
    }

    public class context_with_ignore_and_reason_on_one_spec : context_with_no_specs
    {
        public static bool ignored_spec_ran;

        [Ignore(""example reason"")]
        It should_be_ignored = () =>
            ignored_spec_ran = true;
    }

    [Tags(tag.example)]
    public class context_with_no_specs
    {
        public static bool context_established;
        public static bool cleanup_occurred;

        Establish context = () =>
            context_established = true;

        Cleanup after_each = () =>
            cleanup_occurred = true;
    }

    [Subject(typeof(int), ""Some description"")]
    [Tags(tag.example)]
    public class context_with_subject
    {
    }

    public class context_with_parent_with_subject : context_with_subject
    {
    }

    [Subject(typeof(int), ""Parent description"")]
    public class parent_context
    {
        It should_be_able_to_assert_something = () =>
            true.ShouldBeTrue();

        public class nested_context
        {
            It should_be_able_to_assert_something_else = () =>
                false.ShouldBeFalse();
        }

        public class nested_context_inheriting_another_concern : context_with_subject
        {
            It should_be_able_to_assert_something_else = () =>
                false.ShouldBeFalse();
        }

        [Subject(typeof(int), ""Nested description"")]
        public class nested_context_inheriting_and_owning_a_concern : context_with_subject
        {
            It should_be_able_to_assert_something_else = () =>
                false.ShouldBeFalse();
        }
    }

    public class parent_context_without_concern
    {
        It should_be_able_to_assert_something = () =>
            true.ShouldBeTrue();

        public class nested_context
        {
            It should_be_able_to_assert_something_else = () =>
                false.ShouldBeFalse();
        }
    }

    public class parent_context_that_has_its_own_because_block
    {
        Because of = () => { };

        public class nested_context_that_has_a_because_block_which
        {
            Because of = () => { };
        }
    }

    [Tags(tag.example)]
    public class context_with_failing_specs
    {
        It should = () =>
        {
            throw new InvalidOperationException(""something went wrong"");
        };
    }

    [Tags(tag.example)]
    public class context_with_failing_establish
    {
        Establish context = () =>
        {
            throw new InvalidOperationException(""something went wrong"");
        };

        It should = () => { };
    }

    [Tags(tag.example)]
    public class context_with_failing_because
    {
        Because of = () =>
        {
            throw new InvalidOperationException(""something went wrong"");
        };

        It should = () => { };
    }

    [Tags(tag.example)]
    public class context_with_failing_cleanup
    {
        public static readonly Exception ExceptionThrownByCleanup = new InvalidOperationException(""something went wrong"");

        It should = () => { };

        Cleanup after = () =>
        {
            throw ExceptionThrownByCleanup;
        };
    }

    [Tags(tag.example)]
    public class context_with_console_output
    {
        Establish context = () =>
            System.Console.Out.WriteLine(""Console.Out message in establish"");

        Because of = () =>
            System.Console.Out.WriteLine(""Console.Out message in because"");

        Cleanup after = () =>
            System.Console.Out.WriteLine(""Console.Out message in cleanup"");

        It should_log_messages = () =>
            System.Console.Out.WriteLine(""Console.Out message in spec"");

        It should_log_messages_also_for_the_nth_spec = () =>
            System.Console.Out.WriteLine(""Console.Out message in nth spec"");
    }

    [Tags(tag.example)]
    public class context_with_console_error_output
    {
        Establish context = () =>
            System.Console.Error.WriteLine(""Console.Error message in establish"");

        Because of = () =>
            System.Console.Error.WriteLine(""Console.Error message in because"");

        Cleanup after = () =>
            System.Console.Error.WriteLine(""Console.Error message in cleanup"");

        It should_log_messages = () =>
            System.Console.Error.WriteLine(""Console.Error message in spec"");

        It should_log_messages_also_for_the_nth_spec = () =>
            System.Console.Error.WriteLine(""Console.Error message in nth spec"");
    }

    [Tags(tag.example)]
    public class context_with_debug_output
    {
        Establish context = () =>
            System.Console.WriteLine(""Debug.WriteLine message in establish"");

        Because of = () =>
            System.Console.WriteLine(""Debug.WriteLine message in because"");

        Cleanup after = () =>
            System.Console.WriteLine(""Debug.WriteLine message in cleanup"");

        It should_log_messages = () =>
            System.Console.WriteLine(""Debug.WriteLine message in spec"");

        It should_log_messages_also_for_the_nth_spec = () =>
            System.Console.WriteLine(""Debug.WriteLine message in nth spec"");
    }

    [SetupForEachSpecification, Tags(tag.example)]
    public class context_with_console_output_and_setup_for_each
    {
        Establish context = () =>
            System.Console.Out.WriteLine(""Console.Out message in establish"");

        Because of = () =>
            System.Console.Out.WriteLine(""Console.Out message in because"");

        Cleanup after = () =>
            System.Console.Out.WriteLine(""Console.Out message in cleanup"");

        It should_log_messages = () =>
            System.Console.Out.WriteLine(""Console.Out message in spec"");

        It should_log_messages_also_for_the_nth_spec = () =>
            System.Console.Out.WriteLine(""Console.Out message in nth spec"");
    }

    [Tags(tag.example)]
    public class context_with_inner_exception
    {
        It should_throw = () =>
        {
            try
            {
                throw new Exception(""INNER123"");
            }
            catch (Exception err)
            {
                throw new TargetInvocationException(err);
            }
        };
    }

    public class Container
    {
        [Tags(tag.example)]
        public class nested_context
        {
            It should_be_run = () => { };
        }
    }

    [Tags(tag.example)]
    public class context_with_public_It_field
    {
        public It should;
    }

    [Tags(tag.example)]
    public class context_with_protected_It_field
    {
        protected It should;
    }

    [Tags(tag.example)]
    public class context_with_internal_It_field
    {
        internal It should;
    }

    [Tags(tag.example)]
    public class context_with_public_Behaves_like_field
    {
        public Behaves_like<Behaviors> behavior;
    }

    [Tags(tag.example)]
    public class context_with_nonprivate_framework_fields
    {
        public Establish establish;
        public Because because;
        public Cleanup cleanup;

        internal Behaves_like<Behaviors> behavior;
        protected It specification;
        It private_specification;
    }

    [Tags(tag.example)]
    public class generic_container<Type1>
    {
        public class nested_context
        {
        }

        public class nested_generic_context<Type2>
        {
            public class nested_nested_non_generic
            {
            }

            public class nested_nested_generic<Type3>
            {
            }
        }
    }

    [Tags(tag.example, ""behavior usage"")]
    public class context_with_behaviors
    {
        public static bool local_spec_ran;

        It should_run = () => local_spec_ran = true;

        Behaves_like<Behaviors> behavior;
    }

    [Tags(tag.example, ""behavior usage"")]
    public class second_context_with_behaviors
    {
        Behaves_like<Behaviors> behavior;
    }

    [Tags(tag.example)]
    public class context_with_behaviors_where_the_behavior_field_is_ignored
    {
        public static bool local_spec_ran;

        It should_run = () =>
            local_spec_ran = true;

        [Ignore(""example reason"")]
        Behaves_like<Behaviors> behavior;
    }

    [Tags(tag.example)]
    public class context_with_behaviors_where_the_behavior_is_ignored
    {
        public static bool local_spec_ran;

        It should_run = () =>
            local_spec_ran = true;

        Behaves_like<IgnoredBehaviors> behavior;
    }

    [Tags(tag.example)]
    public class context_with_behaviors_where_the_behavior_specs_are_ignored
    {
        public static bool local_spec_ran;

        It should_run = () =>
            local_spec_ran = true;

        Behaves_like<BehaviorsWithIgnoredSpec> behavior;
    }

    [Tags(tag.example)]
    public class context_with_nested_behaviors
    {
        public static bool local_spec_ran;

        It should_run = () =>
            local_spec_ran = true;

        Behaves_like<BehaviorsWithNestedBehavior> behavior_with_nested_behavior;
    }

    [Tags(tag.example)]
    public class context_with_behaviors_without_behaviors_attribute
    {
        public static bool local_spec_ran;

        It should_run = () =>
            local_spec_ran = true;

        Behaves_like<BehaviorsWithoutBehaviorsAttribute> behavior_without_behavior_attribute;
    }

    [Tags(tag.example)]
    public class context_with_behaviors_with_establish
    {
        public static bool local_spec_ran;

        It should_run = () =>
            local_spec_ran = true;

        Behaves_like<BehaviorsWithEstablish> behavior_with_establish;
    }

    [Tags(tag.example)]
    public class context_with_behaviors_with_because
    {
        public static bool local_spec_ran;

        It should_run = () =>
            local_spec_ran = true;

        Behaves_like<BehaviorsWithBecause> behavior_with_because;
    }

    [Tags(tag.example)]
    public class context_missing_protected_fields_that_are_in_behaviors
    {
        public static bool local_spec_ran;

        It should_not_run = () =>
            local_spec_ran = true;

        Behaves_like<BehaviorsWithProtectedFields> behavior_with_protected_fields;
    }

    [Tags(tag.example)]
    public class context_with_protected_fields_having_different_types_than_in_behaviors
    {
        public static bool local_spec_ran;
        protected static bool field_that_should_be_copied_over_from_context;

        It should_not_run = () =>
            local_spec_ran = true;

        Behaves_like<BehaviorsWithProtectedFields> behavior_with_protected_fields;
    }

    [Behaviors]
    public class Behaviors
    {
        public static bool behavior_spec_ran;

        It should_run_if_behavior_is_not_ignored = () =>
            behavior_spec_ran = true;
    }

    [Ignore(""example reason"")]
    [Behaviors]
    public class IgnoredBehaviors
    {
        public static bool behavior_spec_ran;

        It should_not_run = () =>
            behavior_spec_ran = true;
    }

    [Behaviors]
    public class BehaviorsWithIgnoredSpec
    {
        public static bool behavior_spec_ran;

        [Ignore(""example reason"")]
        It should_not_run = () =>
            behavior_spec_ran = true;
    }

    [Behaviors]
    public class BehaviorsWithNestedBehavior
    {
        Behaves_like<object> disallowed_nested_behavior;
    }

    public class BehaviorsWithoutBehaviorsAttribute
    {
        public static bool behavior_spec_ran;

        It should_not_run = () =>
            behavior_spec_ran = true;
    }

    [Behaviors]
    public class BehaviorsWithEstablish
    {
        Establish context;
    }

    [Behaviors]
    public class BehaviorsWithBecause
    {
        Because of;
    }

    [Behaviors]
    public class BehaviorsWithProtectedFields
    {
        public static bool behavior_spec_ran;
        protected static int field_that_should_be_copied_over_from_context;

        It should_not_run = () =>
            behavior_spec_ran = true;
    }

    [Subject(""Context that inherits"")]
    [Tags(tag.example)]
    public abstract class context_that_inherits
    {
        public static int base_establish_run_count;

        Establish context = () =>
            base_establish_run_count++;

        protected It should_be_inherited_but_not_executed = () => { };

        It should_not_be_executed = () => { };
    }

    public class context_with_inherited_specifications : context_that_inherits
    {
        public static int because_clause_run_count;
        public static int establish_run_count;

        Establish context = () =>
            establish_run_count++;

        Because of = () =>
            because_clause_run_count++;

        It spec1 = () => { };

        It spec2 = () => { };
    }

    [SetupForEachSpecification]
    public class context_with_inherited_specifications_and_setup_for_each : context_that_inherits
    {
        public static int because_clause_run_count;
        public static int establish_run_count;

        Establish context = () =>
            establish_run_count++;

        Because of = () =>
            because_clause_run_count++;

        It spec1 = () => { };

        It spec2 = () => { };
    }

    [Behaviors]
    public class SampleBehaviors
    {
        It behavior1 = () => { };
    }

    public class context_with_specs_and_behaviors
    {
        Establish context = () => { };

        Because of = () => { };

        Behaves_like<SampleBehaviors> behaviors;

        It spec1 = () => { };

        It spec2 = () => { };
    }

    public class TestCleanupAfterEveryContext : ICleanupAfterEveryContextInAssembly
    {
        public static bool AfterContextCleanupRun;
        public static int AfterContextCleanupRunCount;

        public void AfterContextCleanup()
        {
            ++AfterContextCleanupRunCount;
            AfterContextCleanupRun = true;
        }

        public static void Reset()
        {
            AfterContextCleanupRun = false;
            AfterContextCleanupRunCount = 0;
        }
    }

    [Tags(""foobar"")]
    public class TaggedCleanup : ICleanupAfterEveryContextInAssembly
    {
        public static bool AfterContextCleanupRun;
        public static int AfterContextCleanupRunCount;

        public void AfterContextCleanup()
        {
            ++AfterContextCleanupRunCount;
            AfterContextCleanupRun = true;
        }

        public static void Reset()
        {
            AfterContextCleanupRun = false;
            AfterContextCleanupRunCount = 0;
        }
    }

    public class UntaggedCleanup : ICleanupAfterEveryContextInAssembly
    {
        public static bool AfterContextCleanupRun;
        public static int AfterContextCleanupRunCount;

        public void AfterContextCleanup()
        {
            ++AfterContextCleanupRunCount;
            AfterContextCleanupRun = true;
        }

        public static void Reset()
        {
            AfterContextCleanupRun = false;
            AfterContextCleanupRunCount = 0;
        }
    }

    [Tags(""foobar"")]
    public class TaggedAssemblyContext : IAssemblyContext
    {
        public static bool OnAssemblyStartRun;
        public static bool OnAssemblyCompleteRun;

        public static void Reset()
        {
            OnAssemblyStartRun = false;
            OnAssemblyCompleteRun = false;
        }

        public void OnAssemblyStart()
        {
            OnAssemblyStartRun = true;
        }

        public void OnAssemblyComplete()
        {
            OnAssemblyCompleteRun = true;
        }
    }

    public class UntaggedAssemblyContext : IAssemblyContext
    {
        public static bool OnAssemblyStartRun;
        public static bool OnAssemblyCompleteRun;

        public static void Reset()
        {
            OnAssemblyStartRun = false;
            OnAssemblyCompleteRun = false;
        }

        public void OnAssemblyStart()
        {
            OnAssemblyStartRun = true;
        }

        public void OnAssemblyComplete()
        {
            OnAssemblyCompleteRun = true;
        }
    }

    [Tags(""foobar"")]
    public class TaggedResultSupplementer : ISupplementSpecificationResults
    {
        public static bool SupplementResultRun;

        public Result SupplementResult(Result result)
        {
            SupplementResultRun = true;
            return result;
        }

        public static void Reset()
        {
            SupplementResultRun = false;
        }
    }

    public class UntaggedResultSupplementer : ISupplementSpecificationResults
    {
        public static bool SupplementResultRun;

        public Result SupplementResult(Result result)
        {
            SupplementResultRun = true;
            return result;
        }

        public static void Reset()
        {
            SupplementResultRun = false;
        }
    }
}

namespace Example.Random.Internal
{
    [Subject(""Internal types"")]
    [Tags(tag.example)]
    class when_a_context_is_internal
    {
        It should_work = () =>
            true.ShouldBeTrue();
    }

    [Subject(""Internal types"")]
    [Tags(tag.example)]
    class when_a_context_is_internal_and_uses_internal_behaviors
    {
        protected static bool MSpecRocks = true;

        Because of = () =>
            MSpecRocks = true;

        Behaves_like<WorkingSpecs> a_working_spec;
    }

    [Behaviors]
    class WorkingSpecs
    {
        protected static bool MSpecRocks;

        It should_work = () =>
            MSpecRocks.ShouldBeTrue();
    }
}

namespace Example.Random.SingleContextInThisNamespace
{
    public class context_without_any_other_in_the_same_namespace
    {
        Establish context = () => { };

        Because of = () => { };

        It spec1 = () => { };

        It spec2 = () => { };
    }
}

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

    public class ContextWithSpecificationExpectingThrowThatDoesnt
    {
        public static bool it_invoked;
        static Exception exception;

        Because of = () =>
            exception = null;

        It should_throw_but_it_wont = () =>
            exception.ShouldNotBeNull();

        public static void Reset()
        {
            it_invoked = false;
        }
    }

    public class ContextWithThrowingWhenAndPassingSpecification
    {
        public static bool it_invoked;

        Because of = () =>
        {
            throw new Exception();
        };

        It should_fail = () =>
            it_invoked = true;

        public static void Reset()
        {
            it_invoked = false;
        }
    }

    public class ContextWithEmptyWhen
    {
        public static bool it_invoked;

        Because of;

        It should_do_stuff = () =>
            it_invoked = true;

        public static void Reset()
        {
            it_invoked = false;
        }
    }

    public class ContextWithTwoWhens
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

        public static void Reset()
        {
            when_1_invoked = false;
            when_2_invoked = false;
            it_for_when_1_invoked = false;
            it_for_when_2_invoked = false;
        }
    }

    public class ContextWithEmptySpecification
    {
        public static bool when_invoked;

        Because of = () =>
            when_invoked = true;

        It should_do_stuff;

        public static void Reset()
        {
            when_invoked = false;
        }
    }

    public class ContextWithThrowingSpecification
    {
        public static bool when_invoked;
        public static bool it_invoked;
        public static Exception exception;

        Because it_happens = () =>
        {
            when_invoked = true;
            exception = Catch.Exception(() => { throw new Exception(); });
        };

        It should_throw_an_exception = () =>
            it_invoked = true;

        public static void Reset()
        {
            when_invoked = false;
            it_invoked = false;
        }
    }

    public class ContextWithSingleSpecification
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

        public static void Reset()
        {
            because_invoked = false;
            it_invoked = false;
            context_invoked = false;
            cleanup_invoked = false;
        }
    }

    public class AsyncSpecifications
    {
        public static bool establish_invoked;

        public static bool because_invoked;

        public static bool async_it_invoked;

        public static bool sync_it_invoked;

        public static bool cleanup_invoked;

        Establish context = async () =>
        {
            establish_invoked = true;

            await Task.Delay(10);
        };

        Because of = async () =>
        {
            because_invoked = true;

            await Task.Delay(10);
        };

        It should_invoke_sync = () =>
            sync_it_invoked = true;

        It should_invoke_async = async () =>
        {
            async_it_invoked = true;
            await Task.Delay(10);
        };

        Cleanup after = async () =>
        {
            cleanup_invoked = true;
            await Task.Delay(10);
        };
    }

    public class AsyncSpecificationsValueTask
    {
        public static int establish_value;

        public static int because_value;

        public static int async_it_value;

        public static int sync_it_value;

        public static int cleanup_value;

        public static ValueTask<int> Test()
        {
            return new ValueTask<int>(10);
        }

        Establish context = async () =>
            establish_value = await Test();

        Because of = async () =>
            because_value = await Test();

        It should_invoke_sync = () =>
            sync_it_value = Test().Result;

        It should_invoke_async = async () =>
            async_it_value = await Test();

        Cleanup after = async () =>
            cleanup_value = await Test();
    }

    public class AsyncSpecificationsWithExceptions
    {
        Because of = async () =>
        {
            throw new InvalidOperationException(""something went wrong"");
        };

        It should_invoke_sync = () =>
        {
            throw new InvalidOperationException(""something went wrong"");
        };

        It should_invoke_async = async () =>
        {
            throw new InvalidOperationException(""something went wrong"");
        };
    }
}";
    }
}
