using System;
using System.Diagnostics;
using System.Reflection;
using Machine.Specifications;

namespace Example.Random
{
    public static class tag
    {
        public const string example = "example";
        public const string some_other_tag = "some other tag";
        public const string one_more_tag = "one more tag";
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

    [Tags(tag.example, "foobar")]
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

    [Ignore("example reason")]
    public class context_with_ignore : context_with_no_specs
    {
        public static bool ignored_spec_ran;

        It should_be_ignored = () =>
            ignored_spec_ran = true;
    }

    public class context_with_ignore_on_one_spec : context_with_no_specs
    {
        public static bool ignored_spec_ran;

        [Ignore("example reason")]
        It should_be_ignored = () =>
            ignored_spec_ran = true;
    }

    [Ignore("example reason")]
    public class context_with_ignore_and_reason : context_with_no_specs
    {
        public static bool ignored_spec_ran;

        It should_be_ignored = () =>
            ignored_spec_ran = true;
    }

    public class context_with_ignore_and_reason_on_one_spec : context_with_no_specs
    {
        public static bool ignored_spec_ran;

        [Ignore("example reason")]
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

    [Subject(typeof(int), "Some description")]
    [Tags(tag.example)]
    public class context_with_subject
    {
    }

    public class context_with_parent_with_subject : context_with_subject
    {
    }

    [Subject(typeof(int), "Parent description")]
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

        [Subject(typeof(int), "Nested description")]
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
            throw new InvalidOperationException("something went wrong");
    }

    [Tags(tag.example)]
    public class context_with_failing_establish
    {
        Establish context = () =>
            throw new InvalidOperationException("something went wrong");

        It should = () => { };
    }

    [Tags(tag.example)]
    public class context_with_failing_because
    {
        Because of = () =>
            throw new InvalidOperationException("something went wrong");

        It should = () => { };
    }

    [Tags(tag.example)]
    public class context_with_failing_cleanup
    {
        public static readonly Exception ExceptionThrownByCleanup = new InvalidOperationException("something went wrong");

        It should = () => { };

        Cleanup after = () =>
            throw ExceptionThrownByCleanup;
    }

    [Tags(tag.example)]
    public class context_with_console_output
    {
        Establish context = () =>
            Console.Out.WriteLine("Console.Out message in establish");

        Because of = () =>
            Console.Out.WriteLine("Console.Out message in because");

        Cleanup after = () =>
            Console.Out.WriteLine("Console.Out message in cleanup");

        It should_log_messages = () =>
            Console.Out.WriteLine("Console.Out message in spec");

        It should_log_messages_also_for_the_nth_spec = () =>
            Console.Out.WriteLine("Console.Out message in nth spec");
    }

    [Tags(tag.example)]
    public class context_with_console_error_output
    {
        Establish context = () =>
            Console.Error.WriteLine("Console.Error message in establish");

        Because of = () =>
            Console.Error.WriteLine("Console.Error message in because");

        Cleanup after = () =>
            Console.Error.WriteLine("Console.Error message in cleanup");

        It should_log_messages = () =>
            Console.Error.WriteLine("Console.Error message in spec");

        It should_log_messages_also_for_the_nth_spec = () =>
            Console.Error.WriteLine("Console.Error message in nth spec");
    }

    [Tags(tag.example)]
    public class context_with_debug_output
    {
        Establish context = () =>
            Debug.WriteLine("Debug.WriteLine message in establish");

        Because of = () =>
            Debug.WriteLine("Debug.WriteLine message in because");

        Cleanup after = () =>
            Debug.WriteLine("Debug.WriteLine message in cleanup");

        It should_log_messages = () =>
            Debug.WriteLine("Debug.WriteLine message in spec");

        It should_log_messages_also_for_the_nth_spec = () =>
            Debug.WriteLine("Debug.WriteLine message in nth spec");
    }

    [SetupForEachSpecification, Tags(tag.example)]
    public class context_with_console_output_and_setup_for_each
    {
        Establish context = () =>
            Console.Out.WriteLine("Console.Out message in establish");

        Because of = () =>
            Console.Out.WriteLine("Console.Out message in because");

        Cleanup after = () =>
            Console.Out.WriteLine("Console.Out message in cleanup");

        It should_log_messages = () =>
            Console.Out.WriteLine("Console.Out message in spec");

        It should_log_messages_also_for_the_nth_spec = () =>
            Console.Out.WriteLine("Console.Out message in nth spec");
    }

    [Tags(tag.example)]
    public class context_with_inner_exception
    {
        It should_throw = () =>
        {
            try
            {
                throw new Exception("INNER123");
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
}
