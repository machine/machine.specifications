using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications.Factories;
using Machine.Specifications.Model;

namespace Machine.Specifications.Specs.Factories
{
    [Subject(typeof(ContextFactory))]
    public class when_creating_a_context_with_a_concern
    {
        static Context newContext;

        Establish context = () =>
        {
            var factory = new ContextFactory();
            newContext = factory.CreateContextFrom(new context_with_subject());
        };

        It should_capture_the_concerns_type =
            () => newContext.Subject.Type.ShouldEqual(typeof(int));

        It should_capture_the_concerns_description =
            () => newContext.Subject.Description.ShouldEqual("Some description");
    }

    [Subject(typeof(ContextFactory))]
    public class when_creating_a_context_with_a_base_concern_that_has_a_subject_attribute_on_it
    {
        static Context newContext;

        Establish context = () =>
        {
            var factory = new ContextFactory();
            newContext = factory.CreateContextFrom(new context_with_parent_with_subject());
        };

        It should_capture_the_base_concerns_subject_details =
            () => newContext.Subject.Type.ShouldEqual(typeof(int));
    }

    [Subject(typeof(ContextFactory))]
    public class when_using_nested_contexts
    {
        Establish context = () => { outerContextRan = true; };

        public class and_the_parent_context_has_context_blocks
        {
            It should_run_the_parent_context_blocks_prior_to_the_nested_context_blocks =
                () => outerContextRan.ShouldBeTrue();
        }

        static bool outerContextRan;
    }

    [Subject(typeof(ContextFactory))]
    public class when_creating_a_context_that_is_contained_within_another_context_class
    {
        Establish context = () =>
        {
            var factory = new ContextFactory();
            new_context = factory.CreateContextFrom(new parent_context.nested_context());
        };

        It should_contain_the_details_created_by_the_context_detail_factory =
            () => new_context.Specifications.Count().ShouldEqual(1);

        It should_take_the_subject_from_the_outer_class =
            () => new_context.Subject.FullConcern.ShouldEqual("Int32 Parent description");

        static Context new_context;
    }

    [Subject(typeof(ContextFactory))]
    public class when_creating_a_context_that_is_contained_within_another_context_class_and_inherits_a_concern
    {
        Establish context = () =>
        {
            var factory = new ContextFactory();
            new_context = factory.CreateContextFrom(new parent_context.nested_context_inheriting_another_concern());
        };

        It should_take_the_subject_from_the_inherited_concern =
            () => new_context.Subject.FullConcern.ShouldEqual("Int32 Some description");

        static Context new_context;
    }

    [Subject(typeof(ContextFactory))]
    public class when_creating_a_context_that_is_contained_within_another_context_class_and_owns_a_concern
    {
        Establish context = () =>
        {
            var factory = new ContextFactory();
            new_context =
                factory.CreateContextFrom(new parent_context.nested_context_inheriting_and_owning_a_concern());
        };

        It should_take_the_owned_concern =
            () => new_context.Subject.FullConcern.ShouldEqual("Int32 Nested description");

        static Context new_context;
    }

    [Subject(typeof(ContextFactory))]
    public class
        when_creating_a_nested_context_that_has_its_own_because_block_and_its_outer_class_also_has_its_own_because_block
    {
        Establish context = () =>
        {
            ContextFactory.ChangeAllowedNumberOfBecauseBlocksTo(2);

            var factory = new ContextFactory();
            new_context = factory.CreateContextFrom(new parent_context_that_has_its_own_because_block
                .nested_context_that_has_a_because_block_which());
        };

        It should_be_able_to_be_created_successfully_if_a_testing_tool_has_specified_to_override_the_allowed_number_of_because_blocks
            =
            () => new_context.ShouldNotBeNull();

        static Context new_context;
    }

    [Subject(typeof(ContextFactory))]
    public class when_creating_a_context_that_is_contained_within_another_context_class_without_concern
    {
        Establish context = () =>
        {
            var factory = new ContextFactory();
            new_context = factory.CreateContextFrom(new parent_context_without_concern.nested_context());
        };

        It should_have_no_concern =
            () => new_context.Subject.ShouldBeNull();

        static Context new_context;
    }

    [Subject(typeof(ContextFactory))]
    public class when_creating_a_context_with_tags
    {
        static Context newContext;

        Establish context = () =>
        {
            var factory = new ContextFactory();
            newContext = factory.CreateContextFrom(new context_with_tags());
        };

        It should_capture_the_tags =
            () => newContext.Tags.ShouldContainOnly(new Tag(tag2.example),
                new Tag(tag2.some_other_tag),
                new Tag(tag2.one_more_tag));
    }

    [Subject(typeof(ContextFactory))]
    public class when_creating_a_context_with_duplicate_tags
    {
        static Context newContext;

        Establish context = () =>
        {
            var factory = new ContextFactory();
            newContext = factory.CreateContextFrom(new context_with_duplicate_tags());
        };

        It should_capture_the_tags_once =
            () => newContext.Tags.Count().ShouldEqual(1);
    }

    [Subject(typeof(ContextFactory))]
    public class when_creating_a_context_that_is_nested_inside_of_a_generic_class
    {
        public class and_the_nested_context_is_not_a_generic_type_definition
        {
            static Context newContext;

            Establish context = () =>
            {
                var factory = new ContextFactory();
                newContext = factory.CreateContextFrom(new generic_container<int>.nested_context());
            };

            It should_be_able_to_create_the_context_even_though_the_enclosing_class_is_generic =
                () => newContext.ShouldNotBeNull();
        }

        public class and_the_nested_context_is_a_generic_type_definition
        {
            static Context newContext;

            Establish context = () =>
            {
                var factory = new ContextFactory();
                newContext = factory.CreateContextFrom(new generic_container<int>.nested_generic_context<String>());
            };

            It should_be_able_to_create_the_context_even_though_the_enclosing_class_is_generic =
                () => newContext.ShouldNotBeNull();
        }

        public class and_there_are_multiple_generic_parents
        {
            public class and_the_nested_context_is_not_generic
            {
                static Context newContext;

                Establish context = () =>
                {
                    var factory = new ContextFactory();
                    newContext = factory.CreateContextFrom(
                        new generic_container<int>.nested_generic_context<string>.nested_nested_non_generic());
                };

                It should_be_able_to_create_the_context_even_though_the_enclosing_class_is_generic =
                    () => newContext.ShouldNotBeNull();
            }

            public class and_the_nested_context_is_generic
            {
                static Context newContext;

                Establish context = () =>
                {
                    var factory = new ContextFactory();
                    newContext = factory.CreateContextFrom(
                        new generic_container<int>.nested_generic_context<string>.nested_nested_generic<bool>());
                };

                It should_be_able_to_create_the_context_even_though_the_enclosing_class_is_generic =
                    () => newContext.ShouldNotBeNull();
            }
        }
    }

    [Subject(typeof(ContextFactory))]
    public class when_creating_behavior_specifications_and_tracking_original_behavior_field
    {
        static Context newContext;

        Establish context = () =>
        {
            var factory = new ContextFactory();
            newContext = factory.CreateContextFrom(new context_with_behaviors());
        };

        It should_create_behavior_specs_with_original_behavior_field =
            () => newContext.Specifications
                .OfType<BehaviorSpecification>()
                .First()
                .BehaviorFieldInfo.Name.ShouldContainOnly("behavior");
    }

    [Subject(typeof(ContextFactory))]
    public class ContextFactoryTests
    {
        static Context single_context;
        static ContextFactory factory;

        Establish context = () =>
            factory = new ContextFactory();

        Because of = () =>
            single_context = factory.CreateContextFrom(new ContextWithSingleSpecification());

        It should_set_type = () =>
            single_context.Type.Name.ShouldEqual("ContextWithSingleSpecification");
    }

    [Subject(typeof(ContextFactory))]
    public class when_using_nested_contexts_hierarchy
    {
        static IList<int> numbers;
        static ContextFactory factory;
        static Context newContext;

        Establish context = () =>
        {
            numbers = new List<int>();
            factory = new ContextFactory();

            top_of_hierarchy.Numbers = numbers;
        };

        Because of = () =>
        {
            newContext = factory.CreateContextFrom(new top_of_hierarchy.inner());
            newContext.EstablishContext();
        };

        It establish_blocks_run_in_the_correct_order = () =>
        {
            numbers.Count.ShouldEqual(2);
            numbers[0].ShouldEqual(1);
            numbers[1].ShouldEqual(2);
        };
    }

    public class top_of_hierarchy
    {
        public static IList<int> Numbers;

        Establish first = () => { Numbers.Add(1); };

        public class inner
        {
            Establish second = () => { Numbers.Add(2); };
        }
    }

    [Subject(typeof(ContextFactory))]
    public class when_creating_a_nested_context
    {
        static List<int> numbers;
        static ContextFactory factory;
        static Context newContext;

        Establish context = () =>
        {
            numbers = new List<int>();
            factory = new ContextFactory();

            top_of_hierarchy_nested_context.Numbers = numbers;
        };

        Because of = () =>
        {
            newContext = factory.CreateContextFrom(new top_of_hierarchy_nested_context.inner());
            newContext.EstablishContext();
        };

        It establish_blocks_run_in_the_correct_order = () =>
        {
            var results = numbers.Distinct().ToList();
            results[0].ShouldEqual(1);
            results[1].ShouldEqual(2);
            results[2].ShouldEqual(3);
        };
    }

    public class top_of_hierarchy_nested_context
    {
        public static List<int> Numbers;

        Establish first = () =>
            Numbers.Add(1);

        public class some_other_base
        {
            Establish c = () =>
                Numbers.Add(2);
        }

        public class inner : some_other_base
        {
            Establish third = () =>
                Numbers.Add(3);
        }
    }

    [Subject(typeof(ContextFactory))]
    public class when_creating_a_context_that_is_nested_inside_of_a_generic_class_hierarchy
    {
        class and_the_nested_context_is_not_a_generic_type_definition
        {
            static Context newContext;
            static ContextFactory factory;

            Establish context = () =>
                factory = new ContextFactory();

            Because of = () =>
                newContext = factory.CreateContextFrom(new generic_container<int>.nested_context());

            It should_be_able_to_create_the_context_even_though_the_enclosing_class_is_generic = () =>
                newContext.ShouldNotBeNull();
        }

        class and_the_nested_context_is_a_generic_type_definition
        {
            static Context newContext;
            static ContextFactory factory;

            Establish context = () =>
                factory = new ContextFactory();

            Because of = () =>
                newContext = factory.CreateContextFrom(new generic_container<int>.nested_generic_context<string>());

            It should_be_able_to_create_the_context_even_though_the_enclosing_class_is_generic = () =>
                newContext.ShouldNotBeNull();
        }

        class and_there_are_multiple_generic_parents
        {
            class and_the_nested_context_is_not_generic
            {
                static Context newContext;
                static ContextFactory factory;

                Establish context = () =>
                    factory = new ContextFactory();

                Because of = () =>
                    newContext = factory.CreateContextFrom(
                            new generic_container<int>.nested_generic_context<string>.nested_nested_non_generic());

                It should_be_able_to_create_the_context_even_though_the_enclosing_class_is_generic = () =>
                    newContext.ShouldNotBeNull();
            }

            class and_the_nested_context_generic
            {
                static Context newContext;
                static ContextFactory factory;

                Establish context = () =>
                    factory = new ContextFactory();

                Because of = () =>
                    newContext = factory.CreateContextFrom(
                        new generic_container<int>.nested_generic_context<string>.nested_nested_generic<bool>());

                It should_be_able_to_create_the_context_even_though_the_enclosing_class_is_generic = () =>
                    newContext.ShouldNotBeNull();
            }
        }
    }
}
