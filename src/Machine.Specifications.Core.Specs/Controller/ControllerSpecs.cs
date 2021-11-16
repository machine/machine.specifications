using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System;
using Machine.Specifications.Specs.Runner;
using Example.Random;

namespace Machine.Specifications.Specs.Controller
{
    public class With_Controller : RunnerSpecs
    {
        protected static Machine.Specifications.Controller.Controller Controller;

        protected static List<string> ListenEvents;

        Establish context = () =>
        {
            ListenEvents = new List<string>();
            TestAssemblyContext.OnAssemblyStartRun = false;
            TestAssemblyContext.OnAssemblyCompleteRun = false;
            Controller = new Machine.Specifications.Controller.Controller(eventText =>
            {
                ListenEvents.Add(eventText);
            });
        };
    }

    [Behaviors]
    public class RunListenerNotificationBehaviors
    {
        protected static List<string> ListenEvents;

        It notifies_listener_of_protocol_version = () =>
            ListenEvents.All(e => e.Contains("<listener version=\"1.0\">")).ShouldBeTrue();

        It notifies_listener_that_the_run_has_started = () =>
            ListenEvents.Count(e => e.Contains("<onrunstart />")).ShouldEqual(1);

        It notifies_listener_that_the_run_has_ended = () =>
            ListenEvents.Count(e => e.Contains("<onrunend />")).ShouldEqual(1);

        It notifies_listener_that_the_assembly_run_has_ended = () =>
            ListenEvents.Count(e => e.Contains("<onassemblyend>")).ShouldEqual(1);

        It notifies_listener_that_the_assembly_run_has_started = () =>
            ListenEvents.Count(e => e.Contains("<onassemblystart>")).ShouldEqual(1);

        It notifies_listener_that_the_context_run_has_ended = () =>
            ListenEvents.Count(e => e.Contains("<oncontextend>")).ShouldBeGreaterThanOrEqualTo(1);

        It notifies_listener_that_the_context_run_has_started = () =>
            ListenEvents.Count(e => e.Contains("<oncontextstart>")).ShouldBeGreaterThanOrEqualTo(1);

        It notifies_listener_that_the_spec_run_has_ended = () =>
            ListenEvents.Count(e => e.Contains("<onspecificationstart>")).ShouldBeGreaterThanOrEqualTo(1);

        It notifies_listener_that_the_spec_run_has_started = () =>
            ListenEvents.Count(e => e.Contains("<onspecificationend>")).ShouldBeGreaterThanOrEqualTo(1);

        It runs_assembly_context_start = () =>
            TestAssemblyContext.OnAssemblyStartRun.ShouldBeTrue();

        It runs_assembly_complete = () =>
            TestAssemblyContext.OnAssemblyCompleteRun.ShouldBeTrue();
    }

    [Subject(typeof(Machine.Specifications.Controller.Controller))]
    class When_running_assemblies : With_Controller
    {
        Because of = () =>
        {
            var type = typeof(Example.Random.SingleContextInThisNamespace.context_without_any_other_in_the_same_namespace);

            Controller.StartRun();
            Controller.RunAssemblies(new[] {type.GetTypeInfo().Assembly});
            Controller.EndRun();
        };

        Behaves_like<RunListenerNotificationBehaviors> run_listener_notifier;
    }

    [Subject(typeof(Machine.Specifications.Controller.Controller))]
    class When_running_namespace : With_Controller
    {
        Because of = () =>
        {
            var type = typeof(Example.Random.SingleContextInThisNamespace.context_without_any_other_in_the_same_namespace);

            Controller.StartRun();
            Controller.RunNamespaces(type.GetTypeInfo().Assembly, new[] {type.Namespace});
            Controller.EndRun();
        };

        Behaves_like<RunListenerNotificationBehaviors> run_listener_notifier;
    }

    [Subject(typeof(Machine.Specifications.Controller.Controller))]
    class When_running_types : With_Controller
    {
        Because of = () =>
        {
            var type = typeof(Example.Random.SingleContextInThisNamespace.context_without_any_other_in_the_same_namespace);

            Controller.StartRun();
            Controller.RunTypes(type.GetTypeInfo().Assembly, new[] {type});
            Controller.EndRun();
        };

        Behaves_like<RunListenerNotificationBehaviors> run_listener_notifier;
    }

    [Subject(typeof(Machine.Specifications.Controller.Controller))]
    class When_running_members : With_Controller
    {
        Because of = () =>
        {
            var type = typeof(Example.Random.SingleContextInThisNamespace.context_without_any_other_in_the_same_namespace);

            Controller.StartRun();
            Controller.RunMembers(type.GetTypeInfo().Assembly, new[]
            {
                type.GetField("spec1", BindingFlags.NonPublic | BindingFlags.Instance),
                type.GetField("spec2", BindingFlags.NonPublic | BindingFlags.Instance),
            });
            Controller.EndRun();
        };

        Behaves_like<RunListenerNotificationBehaviors> run_listener_notifier;
    }

    [Subject(typeof(Machine.Specifications.Controller.Controller))]
    class When_runspecs_is_given_two_standard_specs : With_Controller
    {
        Because of = () =>
        {
            var type = typeof(context_with_specs_and_behaviors);

            Controller.StartRun();
            Controller.RunSpecs(type.GetTypeInfo().Assembly, new[]
            {
                type.FullName + ".spec1",
                type.FullName + ".spec2"
            });
            Controller.EndRun();
        };

        Behaves_like<RunListenerNotificationBehaviors> run_listener_notifier;

        It runs_both_specs = () =>
        {
            // two mentions: one for start and one for end spec run
            ListenEvents.Count(e => e.Contains("spec1")).ShouldEqual(2);
            ListenEvents.Count(e => e.Contains("spec2")).ShouldEqual(2);
        };
    }

    [Subject(typeof(Machine.Specifications.Controller.Controller))]
    class When_runspecs_is_given_a_behavior_spec : With_Controller
    {
        Because of = () =>
        {
            var type = typeof(context_with_specs_and_behaviors);

            Controller.StartRun();
            Controller.RunSpecs(type.Assembly, new[] {type.FullName + ".behavior1"});
            Controller.EndRun();
        };

        Behaves_like<RunListenerNotificationBehaviors> run_listener_notifier;

        It runs_only_the_behavior_spec = () =>
        {
            // two mentions: one for start and one for end spec run
            ListenEvents.Count(e => e.Contains("<onspecificationstart>")).ShouldEqual(1);
            ListenEvents.Count(e => e.Contains("behavior1")).ShouldEqual(2);
        };
    }

    [Subject(typeof(Machine.Specifications.Controller.Controller))]
    class When_runspecs_is_given_a_behaves_like_field : With_Controller
    {
        Because of = () =>
        {
            var type = typeof(context_with_specs_and_behaviors);

            Controller.StartRun();
            Controller.RunSpecs(type.Assembly, new[] {type.FullName + ".behaviors"});
            Controller.EndRun();
        };

        It does_not_run_anything = () =>
        {
            // two mentions: one for start and one for end spec run
            ListenEvents.Count(e => e.Contains("<onspecificationstart>")).ShouldEqual(0);
        };
    }
}
