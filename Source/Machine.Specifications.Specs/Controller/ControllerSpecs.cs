using FluentAssertions;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Example.Random.SingleContextInThisNamespace;
using Example.Random;

namespace Machine.Specifications.Specs.Controller
{
    public class With_Controller
    {
        protected static Machine.Specifications.Controller.Controller Controller;
        protected static List<string> ListenEvents;

        Establish context = () => {
            ListenEvents = new List<string>();
            TestAssemblyContext.OnAssemblyStartRun = false;
            TestAssemblyContext.OnAssemblyCompleteRun = false;
            Controller = new Machine.Specifications.Controller.Controller((string eventText) => {
                ListenEvents.Add(eventText);
            });
        };
    }

    [Behaviors]
    public class RunListenerNotificationBehaviors
    {
        protected static List<string> ListenEvents;

        It notifies_listener_of_protocol_version = () => {
            ListenEvents.All(e => e.Contains("<listener version=\"1.0\">")).Should().BeTrue();
        };

        It notifies_listener_that_the_run_has_started = () => {
            ListenEvents.Count(e => e.Contains("<onrunstart />")).Should().Be(1);
        };

        It notifies_listener_that_the_run_has_ended = () => {
            ListenEvents.Count(e => e.Contains("<onrunend />")).Should().Be(1);
        };

        It notifies_listener_that_the_assembly_run_has_ended = () => {
            ListenEvents.Count(e => e.Contains("<onassemblyend>")).Should().Be(1);
        };

        It notifies_listener_that_the_assembly_run_has_started = () => {
            ListenEvents.Count(e => e.Contains("<onassemblystart>")).Should().Be(1);
        };

        It notifies_listener_that_the_context_run_has_ended = () => {
            ListenEvents.Count(e => e.Contains("<oncontextend>")).Should().BeGreaterOrEqualTo(1);
        };

        It notifies_listener_that_the_context_run_has_started = () => {
            ListenEvents.Count(e => e.Contains("<oncontextstart>")).Should().BeGreaterOrEqualTo(1);
        };
        It notifies_listener_that_the_spec_run_has_ended = () => {
            ListenEvents.Count(e => e.Contains("<onspecificationstart>")).Should().BeGreaterOrEqualTo(1);
        };

        It notifies_listener_that_the_spec_run_has_started = () => {
            ListenEvents.Count(e => e.Contains("<onspecificationend>")).Should().BeGreaterOrEqualTo(1);
        };

        It runs_assembly_context_start = () => {
            TestAssemblyContext.OnAssemblyStartRun.Should().BeTrue();
        };

        It runs_assembly_complete = () => {
            TestAssemblyContext.OnAssemblyCompleteRun.Should().BeTrue();
        };
    }


    [Subject(typeof(Machine.Specifications.Controller.Controller))]
    public class When_running_assemblies : With_Controller
    {
        Because of = () => {
            Controller.StartRun();
            Controller.RunAssemblies(new[] {
                typeof(context_without_any_other_in_the_same_namespace).GetTypeInfo().Assembly,
            });
            Controller.EndRun();
        };

        Behaves_like<RunListenerNotificationBehaviors> run_listener_notifier;
    }

    [Subject(typeof(Machine.Specifications.Controller.Controller))]
    public class When_running_namespace : With_Controller
    {
        Because of = () => {
            Controller.StartRun();
            Controller.RunNamespaces(typeof(context_without_any_other_in_the_same_namespace).GetTypeInfo().Assembly,
                                    new[] { typeof(context_without_any_other_in_the_same_namespace).Namespace});
            Controller.EndRun();
        };

        Behaves_like<RunListenerNotificationBehaviors> run_listener_notifier;
    }

    [Subject(typeof(Machine.Specifications.Controller.Controller))]
    public class When_running_types : With_Controller
    {
        Because of = () => {
            Controller.StartRun();
            Controller.RunTypes(typeof(context_without_any_other_in_the_same_namespace).GetTypeInfo().Assembly,
                                new[] {
                                    typeof(context_without_any_other_in_the_same_namespace),
                                });
            Controller.EndRun();
        };

        Behaves_like<RunListenerNotificationBehaviors> run_listener_notifier;
    }

    [Subject(typeof(Machine.Specifications.Controller.Controller))]
    public class When_running_members : With_Controller
    {
        Because of = () => {
            Controller.StartRun();
            Controller.RunMembers(typeof(context_without_any_other_in_the_same_namespace).GetTypeInfo().Assembly,
                                new[] {
                                    typeof(context_without_any_other_in_the_same_namespace).GetField("spec1", BindingFlags.NonPublic | BindingFlags.Instance),
                                    typeof(context_without_any_other_in_the_same_namespace).GetField("spec2", BindingFlags.NonPublic | BindingFlags.Instance),
                                });
            Controller.EndRun();
        };

        Behaves_like<RunListenerNotificationBehaviors> run_listener_notifier;
    }
}
