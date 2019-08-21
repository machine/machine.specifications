using System.Reflection;
using Machine.Specifications.Reporting.Generation;
using Machine.Specifications.Runner.Utility;
using RunOptions = Machine.Specifications.Runner.Utility.RunOptions;

namespace Machine.Specifications.Reporting.Specs
{
    [Subject(typeof(CollectReportingInformationRunListener))]
    public class when_running_two_contexts_that_use_the_same_behavior
    {
        private const string code = @"
using Machine.Specifications;

namespace Example.Random
{
    public static class tag
    {
        public const string example = ""example"";
        public const string some_other_tag = ""some other tag"";
        public const string one_more_tag = ""one more tag"";
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

        [Ignore(""example reason"")]
        Behaves_like<Behaviors> behavior;
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

        [Ignore(""example reason"")]
        It should_not_run = () => BehaviorSpecRan = true;
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

        static ISpecificationRunner runner;
        static CollectReportingInformationRunListener reportListener;
        static RunOptions runOptions;
        static AssemblyPath specAssemblyPath;
        static CompileContext compiler;

        Establish context = () =>
          {
              reportListener = new CollectReportingInformationRunListener();
              runOptions = RunOptions.Custom.Include(new[] { "behavior usage" });
              compiler = new CompileContext(code);
              specAssemblyPath = compiler.Compile();
              runner = new AppDomainRunner(reportListener, runOptions);
          };

        Because of = () => runner.RunAssemblies(new[] { specAssemblyPath });

        It should_collect_behavior_specifications_and_context_specifications =
          () => reportListener.ResultsBySpecification.Count.ShouldEqual(3);

        Cleanup cleanup = () =>
            compiler.Dispose();
    }
}
