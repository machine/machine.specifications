using System;
using System.IO;
using System.Reflection;

namespace Machine.Specifications.Runner.Utility
{
    public class running_specs
    {
        public static AppDomainTestListener listener;
        public static AppDomainRunner runner;

        Establish context = () =>
        {
            listener = new AppDomainTestListener();
            runner = new AppDomainRunner(listener, RunOptions.Default);
        };
    }

    public class when_running_specs_by_assembly : banking_running_specs
    {
        Because of = () =>
            runner.RunAssembly(path);

        It should_run_them_all = () =>
            listener.SpecCount.ShouldEqual(6);
    }

    public class when_running_specs_in_an_assembly_with_a_reference_that_cannot_be_bound : running_specs
    {
        const string SpecCode = @"
using Example.BindingFailure.Ref;
using Machine.Specifications;

namespace Example.BindingFailure
{
    [Subject(""Assembly binding failure"")]
    public class if_a_referenced_assembly_cannot_be_bound
    {
        Referenced referenced;

        It will_fail = () => { };
    }
}";

        const string ReferencedCode = @"
namespace Example.BindingFailure.Ref
{
    public class Referenced
    {
    }
}";

        static Exception Exception;

        static CompileContext compiler;

        static AssemblyPath path;

        Establish context = () =>
        {
            compiler = new CompileContext();

            var referencedPath = compiler.Compile(ReferencedCode);
            path = compiler.Compile(SpecCode, referencedPath);

            if (File.Exists(referencedPath))
                File.Delete(referencedPath);
        };

        Because of = () =>
            runner.RunAssembly(path);

        It should_fail = () =>
            listener.LastFatalError.ShouldNotBeNull();

        It should_record_that_the_referenced_assembly_could_not_be_found = () =>
            listener.LastFatalError.FullTypeName.ShouldEqual(typeof(FileNotFoundException).FullName);
    }

    [Tags("Issue-146")]
    public class when_running_an_assembly_that_does_not_use_mspec : running_specs
    {
        const string Code = "";
        static Exception Exception;

        static CompileContext compiler;

        static AssemblyPath path;

        Establish context = () =>
        {
            compiler = new CompileContext();

            path = compiler.Compile(Code);
        };

        Because of = () =>
            runner.RunAssembly(path);

        It should_succeed = () =>
            true.ShouldBeTrue();

        Cleanup after = () =>
            compiler.Dispose();
    }

    [Ignore("exceptions during cleanup are ignored")]
    public class when_running_specs_in_which_the_cleanup_throws_a_non_serializable_exception : running_specs
    {
        const string Code = @"
using System;
using Machine.Specifications;

namespace Example.CleanupFailure
{
    public class cleanup_failure
    {
        It is_inevitable = () => { };

        Cleanup after = () => throw new NonSerializableException();
    }

    public class NonSerializableException : Exception
    {
    }
}";

        static CompileContext compiler;

        static AssemblyPath path;

        Establish context = () =>
        {
            compiler = new CompileContext();

            path = compiler.Compile(Code);
        };

        Because of = () =>
            runner.RunAssembly(path);

        It should_cause_a_fatal_error = () =>
            listener.LastFatalError.ShouldNotBeNull();

        Cleanup after = () =>
            compiler.Dispose();
    }

    public class banking_running_specs : running_specs
    {
        const string Code = @"
using System;
using Machine.Specifications;

namespace Example
{
    public abstract class AccountSpecs
    {
        protected static Account fromAccount;
        protected static Account toAccount;
    }

    [Subject(typeof(Account), ""Funds transfer"")]
    public class when_transferring_between_two_accounts : AccountSpecs
    {
        Establish context = () =>
        {
            fromAccount = new Account {Balance = 1m};
            toAccount = new Account {Balance = 1m};
        };

        Because of = () =>
            fromAccount.Transfer(1m, toAccount);

        It should_debit_the_from_account_by_the_amount_transferred = () =>
            fromAccount.Balance.ShouldEqual(0m);

        It should_credit_the_to_account_by_the_amount_transferred = () =>
            toAccount.Balance.ShouldEqual(2m);
    }

    [Subject(typeof(Account), ""Funds transfer"")]
    [Tags(""failure"")]
    public class when_transferring_an_amount_larger_than_the_balance_of_the_from_account : AccountSpecs
    {
        static Exception exception;

        Establish context = () =>
        {
            fromAccount = new Account {Balance = 1m};
            toAccount = new Account {Balance = 1m};
        };

        Because of = () =>
            exception = Catch.Exception(() => fromAccount.Transfer(2m, toAccount));

        It should_not_allow_the_transfer = () =>
            exception.ShouldBeOfExactType<Exception>();
    }

    [Subject(""Recent Account Activity Summary page"")]
    public class when_a_customer_first_views_the_account_summary_page
    {
        It should_display_all_account_transactions_for_the_past_thirty_days;
        It should_display_debit_amounts_in_red_text;
        It should_display_deposit_amounts_in_black_text;
    }

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

    public class Account
    {
        public decimal Balance { get; set; }

        public void Transfer(decimal amount, Account toAccount)
        {
            if (amount > Balance)
                throw new Exception(String.Format(""Cannot transfer ${0}. The available balance is ${1}."", amount, Balance));

            Balance -= amount;
            toAccount.Balance += amount;
        }
    }
}";

        static CompileContext compiler;

        protected static AssemblyPath path;

        Establish context = () =>
        {
            compiler = new CompileContext();

            path = compiler.Compile(Code);
        };

        Cleanup after = () =>
            compiler.Dispose();
    }

    public class when_running_specs_by_namespace : banking_running_specs
    {
        Because of = () =>
            runner.RunNamespace(path, "Example");

        It should_run_them_all = () =>
            listener.SpecCount.ShouldEqual(6);
    }

    public class when_running_specs_by_member : banking_running_specs
    {
        Because of = () =>
        {
            var type = Assembly.LoadFile(path).GetType("Example.when_transferring_an_amount_larger_than_the_balance_of_the_from_account");

            runner.RunMember(path, type.GetField("should_not_allow_the_transfer", BindingFlags.NonPublic | BindingFlags.Instance));
        };

        It should_run = () =>
            listener.SpecCount.ShouldEqual(1);
    }

    public class random_running_specs : running_specs
    {
        const string Code = @"
using System;
using System.Diagnostics;
using System.Reflection;
using Machine.Specifications;

namespace Example.Random
{
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
        public static int EstablishRunCount;
        public static int BecauseClauseRunCount;

        Establish context = () => EstablishRunCount++;

        Because of = () => BecauseClauseRunCount++;

        It spec1 = () => { };
        It spec2 = () => { };
    }

    [Tags(tag.example, ""foobar"")]
    public class context_with_multiple_specifications
    {
        public static int EstablishRunCount;
        public static int BecauseClauseRunCount;

        Establish context = () => EstablishRunCount++;

        Because of = () => BecauseClauseRunCount++;

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
        public static bool IgnoredSpecRan;

        It should_be_ignored = () =>
            IgnoredSpecRan = true;
    }

    public class context_with_ignore_on_one_spec : context_with_no_specs
    {
        public static bool IgnoredSpecRan;

        [Ignore(""example reason"")] It should_be_ignored = () =>
            IgnoredSpecRan = true;
    }

    [Ignore(""example reason"")]
    public class context_with_ignore_and_reason : context_with_no_specs
    {
        public static bool IgnoredSpecRan;

        It should_be_ignored = () =>
            IgnoredSpecRan = true;
    }

    public class context_with_ignore_and_reason_on_one_spec : context_with_no_specs
    {
        public static bool IgnoredSpecRan;

        [Ignore(""example reason"")] It should_be_ignored = () =>
            IgnoredSpecRan = true;
    }

    [Tags(tag.example)]
    public class context_with_no_specs
    {
        public static bool ContextEstablished;
        public static bool CleanupOccurred;

        Establish context = () => { ContextEstablished = true; };

        Cleanup after_each = () => { CleanupOccurred = true; };
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
        It should = () => { throw new InvalidOperationException(""something went wrong""); };
    }

    [Tags(tag.example)]
    public class context_with_failing_establish
    {
        Establish context = () => { throw new InvalidOperationException(""something went wrong""); };
        It should = () => { };
    }

    [Tags(tag.example)]
    public class context_with_failing_because
    {
        Because of = () => { throw new InvalidOperationException(""something went wrong""); };
        It should = () => { };
    }

    [Tags(tag.example)]
    public class context_with_console_output
    {
        Establish context =
            () => Console.Out.WriteLine(""Console.Out message in establish"");

        Because of =
            () => Console.Out.WriteLine(""Console.Out message in because"");

        Cleanup after =
            () => Console.Out.WriteLine(""Console.Out message in cleanup"");

        It should_log_messages =
            () => Console.Out.WriteLine(""Console.Out message in spec"");

        It should_log_messages_also_for_the_nth_spec =
            () => Console.Out.WriteLine(""Console.Out message in nth spec"");
    }

    [Tags(tag.example)]
    public class context_with_console_error_output
    {
        Establish context =
            () => Console.Error.WriteLine(""Console.Error message in establish"");

        Because of =
            () => Console.Error.WriteLine(""Console.Error message in because"");

        Cleanup after =
            () => Console.Error.WriteLine(""Console.Error message in cleanup"");

        It should_log_messages =
            () => Console.Error.WriteLine(""Console.Error message in spec"");

        It should_log_messages_also_for_the_nth_spec =
            () => Console.Error.WriteLine(""Console.Error message in nth spec"");
    }

    [Tags(tag.example)]
    public class context_with_debug_output
    {
        Establish context =
            () => System.Diagnostics.Debug.WriteLine(""Debug.WriteLine message in establish"");

        Because of =
            () => System.Diagnostics.Debug.WriteLine(""Debug.WriteLine message in because"");

        Cleanup after =
            () => System.Diagnostics.Debug.WriteLine(""Debug.WriteLine message in cleanup"");

        It should_log_messages =
            () => System.Diagnostics.Debug.WriteLine(""Debug.WriteLine message in spec"");

        It should_log_messages_also_for_the_nth_spec =
            () => System.Diagnostics.Debug.WriteLine(""Debug.WriteLine message in nth spec"");
    }

    [SetupForEachSpecification, Tags(tag.example)]
    public class context_with_console_output_and_setup_for_each
    {
        Establish context =
            () => Console.Out.WriteLine(""Console.Out message in establish"");

        Because of =
            () => Console.Out.WriteLine(""Console.Out message in because"");

        Cleanup after =
            () => Console.Out.WriteLine(""Console.Out message in cleanup"");

        It should_log_messages =
            () => Console.Out.WriteLine(""Console.Out message in spec"");

        It should_log_messages_also_for_the_nth_spec =
            () => Console.Out.WriteLine(""Console.Out message in nth spec"");
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

    [Tags(tag.example, ""behavior usage"")]
    public class context_with_behaviors
    {
        public static bool LocalSpecRan;

        It should_run = () => LocalSpecRan = true;
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
        public static bool LocalSpecRan;

        It should_run = () => LocalSpecRan = true;

        [Ignore(""example reason"")] Behaves_like<Behaviors> behavior;
    }

    [Tags(tag.example)]
    public class context_with_behaviors_where_the_behavior_is_ignored
    {
        public static bool LocalSpecRan;

        It should_run = () => LocalSpecRan = true;
        Behaves_like<IgnoredBehaviors> behavior;
    }

    [Tags(tag.example)]
    public class context_with_behaviors_where_the_behavior_specs_are_ignored
    {
        public static bool LocalSpecRan;

        It should_run = () => LocalSpecRan = true;
        Behaves_like<BehaviorsWithIgnoredSpec> behavior;
    }

    [Tags(tag.example)]
    public class context_with_nested_behaviors
    {
        public static bool LocalSpecRan;

        It should_run = () => LocalSpecRan = true;
        Behaves_like<BehaviorsWithNestedBehavior> behavior_with_nested_behavior;
    }

    [Tags(tag.example)]
    public class context_with_behaviors_without_behaviors_attribute
    {
        public static bool LocalSpecRan;

        It should_run = () => LocalSpecRan = true;
        Behaves_like<BehaviorsWithoutBehaviorsAttribute> behavior_without_behavior_attribute;
    }

    [Tags(tag.example)]
    public class context_with_behaviors_with_establish
    {
        public static bool LocalSpecRan;

        It should_run = () => LocalSpecRan = true;
        Behaves_like<BehaviorsWithEstablish> behavior_with_establish;
    }

    [Tags(tag.example)]
    public class context_with_behaviors_with_because
    {
        public static bool LocalSpecRan;

        It should_run = () => LocalSpecRan = true;
        Behaves_like<BehaviorsWithBecause> behavior_with_because;
    }

    [Tags(tag.example)]
    public class context_missing_protected_fields_that_are_in_behaviors
    {
        public static bool LocalSpecRan;

        It should_not_run = () => LocalSpecRan = true;
        Behaves_like<BehaviorsWithProtectedFields> behavior_with_protected_fields;
    }

    [Tags(tag.example)]
    public class context_with_protected_fields_having_different_types_than_in_behaviors
    {
        public static bool LocalSpecRan;
        protected static bool fieldThatShouldBeCopiedOverFromContext;

        It should_not_run = () => LocalSpecRan = true;
        Behaves_like<BehaviorsWithProtectedFields> behavior_with_protected_fields;
    }

    [Behaviors]
    public class Behaviors
    {
        public static bool BehaviorSpecRan;

        It should_run_if_behavior_is_not_ignored = () => BehaviorSpecRan = true;
    }

    [Ignore(""example reason"")]
    [Behaviors]
    public class IgnoredBehaviors
    {
        public static bool BehaviorSpecRan;

        It should_not_run = () => BehaviorSpecRan = true;
    }

    [Behaviors]
    public class BehaviorsWithIgnoredSpec
    {
        public static bool BehaviorSpecRan;

        [Ignore(""example reason"")] It should_not_run = () => BehaviorSpecRan = true;
    }

    [Behaviors]
    public class BehaviorsWithNestedBehavior
    {
        Behaves_like<object> diallowed_nested_behavior;
    }

    public class BehaviorsWithoutBehaviorsAttribute
    {
        public static bool BehaviorSpecRan;

        It should_not_run = () => BehaviorSpecRan = true;
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
        public static bool BehaviorSpecRan;
        protected static int fieldThatShouldBeCopiedOverFromContext;

        It should_not_run = () => BehaviorSpecRan = true;
    }

    [Subject(""Context that inherits"")]
    [Tags(tag.example)]
    public abstract class context_that_inherits
    {
        public static int BaseEstablishRunCount;

        Establish context = () => BaseEstablishRunCount++;

        protected It should_be_inherited_but_not_executed = () => { };
        It should_not_be_executed = () => { };
    }

    public class context_with_inherited_specifications : context_that_inherits
    {
        public static int BecauseClauseRunCount;
        public static int EstablishRunCount;

        Establish context = () => EstablishRunCount++;

        Because of = () => BecauseClauseRunCount++;

        It spec1 = () => { };
        It spec2 = () => { };
    }

    [SetupForEachSpecification]
    public class context_with_inherited_specifications_and_setup_for_each : context_that_inherits
    {
        public static int BecauseClauseRunCount;
        public static int EstablishRunCount;

        Establish context = () => EstablishRunCount++;

        Because of = () => BecauseClauseRunCount++;

        It spec1 = () => { };
        It spec2 = () => { };
    }

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

        Because of = () => { MSpecRocks = true; };

        Behaves_like<WorkingSpecs> a_working_spec;
    }

    [Behaviors]
    class WorkingSpecs
    {
        protected static bool MSpecRocks;

        It should_work = () =>
            MSpecRocks.ShouldBeTrue();
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
}";

        static CompileContext compiler;

        protected static AssemblyPath path;

        Establish context = () =>
        {
            compiler = new CompileContext();

            path = compiler.Compile(Code);
        };

        Cleanup after = () =>
            compiler.Dispose();
    }

    public class when_running_a_nested_context_by_member : random_running_specs
    {
        Because of = () =>
        {
            Type member = Assembly.LoadFile(path).GetType("Example.Random.Container+nested_context");

            runner.RunMember(path, member);
        };

        It should_run = () =>
            listener.SpecCount.ShouldEqual(1);
    }

    public class when_running_specs_of_a_nested_context_by_member : random_running_specs
    {
        Because of = () =>
        {
            Type member = Assembly.LoadFile(path).GetType("Example.Random.Container+nested_context");

            runner.RunMember(path, member.GetField("should_be_run", BindingFlags.NonPublic | BindingFlags.Instance));
        };

        It should_run = () =>
            listener.SpecCount.ShouldEqual(1);
    }

    public class AppDomainTestListener : ISpecificationRunListener
    {
        public int SpecCount;

        public AssemblyInfo LastAssembly { get; private set; }
        public ContextInfo LastContext { get; private set; }
        public SpecificationInfo LastSpecification { get; private set; }
        public ExceptionResult LastFatalError { get; private set; }
        public Result LastResult { get; private set; }

        public void OnRunStart()
        {
            LastAssembly = null;
            LastContext = null;
            LastResult = null;
        }

        public void OnRunEnd()
        {
        }

        public void OnAssemblyStart(AssemblyInfo assembly)
        {
        }

        public void OnAssemblyEnd(AssemblyInfo assembly)
        {
            LastAssembly = assembly;
        }

        public void OnContextStart(ContextInfo context)
        {
        }

        public void OnContextEnd(ContextInfo context)
        {
            LastContext = context;
        }

        public void OnSpecificationStart(SpecificationInfo specification)
        {
        }

        public void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            LastSpecification = specification;
            LastResult = result;
            SpecCount++;
        }

        public void OnFatalError(ExceptionResult exception)
        {
            LastFatalError = exception;
        }
    }
}
