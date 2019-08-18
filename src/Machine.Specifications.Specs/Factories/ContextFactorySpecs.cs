﻿using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications.Factories;
using Machine.Specifications.Model;
using Machine.Specifications.Specs.Runner;

namespace Machine.Specifications.Specs.Factories
{
    [Subject(typeof(ContextFactory))]
    public class when_creating_a_context_with_a_concern : RandomRunnerSpecs
    {
        static Type context_with_subject;
        static Context newContext;

        Establish context = () =>
        {
            context_with_subject = GetRandom("context_with_subject");

            var factory = new ContextFactory();
            newContext = factory.CreateContextFrom(Activator.CreateInstance(context_with_subject));
        };

        It should_capture_the_concerns_type = () =>
            newContext.Subject.Type.ShouldEqual(typeof(int));

        It should_capture_the_concerns_description = () =>
            newContext.Subject.Description.ShouldEqual("Some description");
    }

    [Subject(typeof(ContextFactory))]
    public class when_creating_a_context_with_a_base_concern_that_has_a_subject_attribute_on_it : RandomRunnerSpecs
    {
        static Type context_with_parent_with_subject;
        static Context newContext;

        Establish context = () =>
        {
            context_with_parent_with_subject = GetRandom("context_with_parent_with_subject");

            var factory = new ContextFactory();
            newContext = factory.CreateContextFrom(Activator.CreateInstance(context_with_parent_with_subject));
        };

        It should_capture_the_base_concerns_subject_details = () =>
            newContext.Subject.Type.ShouldEqual(typeof(int));
    }

    [Subject(typeof(ContextFactory))]
    public class when_using_nested_contexts
    {
        static bool outerContextRan;

        Establish context = () =>
            outerContextRan = true;

        public class and_the_parent_context_has_context_blocks
        {
            It should_run_the_parent_context_blocks_prior_to_the_nested_context_blocks = () =>
                outerContextRan.ShouldBeTrue();
        }
    }

    [Subject(typeof(ContextFactory))]
    public class when_creating_a_context_that_is_contained_within_another_context_class : RandomRunnerSpecs
    {
        Establish context = () =>
        {
            var type = GetRandom("parent_context+nested_context");

            var factory = new ContextFactory();
            new_context = factory.CreateContextFrom(Activator.CreateInstance(type));
        };

        It should_contain_the_details_created_by_the_context_detail_factory =
            () => new_context.Specifications.Count().ShouldEqual(1);

        It should_take_the_subject_from_the_outer_class =
            () => new_context.Subject.FullConcern.ShouldEqual("Int32 Parent description");

        static Context new_context;
    }

    [Subject(typeof(ContextFactory))]
    public class when_creating_a_context_that_is_contained_within_another_context_class_and_inherits_a_concern : RandomRunnerSpecs
    {
        Establish context = () =>
        {
            var type = GetRandom("parent_context+nested_context_inheriting_another_concern");

            var factory = new ContextFactory();
            new_context = factory.CreateContextFrom(Activator.CreateInstance(type));
        };

        It should_take_the_subject_from_the_inherited_concern =
            () => new_context.Subject.FullConcern.ShouldEqual("Int32 Some description");

        static Context new_context;
    }

    [Subject(typeof(ContextFactory))]
    public class when_creating_a_context_that_is_contained_within_another_context_class_and_owns_a_concern : RandomRunnerSpecs
    {
        Establish context = () =>
        {
            var type = GetRandom("parent_context+nested_context_inheriting_and_owning_a_concern");

            var factory = new ContextFactory();
            new_context = factory.CreateContextFrom(Activator.CreateInstance(type));
        };

        It should_take_the_owned_concern =
            () => new_context.Subject.FullConcern.ShouldEqual("Int32 Nested description");

        static Context new_context;
    }

    [Subject(typeof(ContextFactory))]
    public class when_creating_a_nested_context_that_has_its_own_because_block_and_its_outer_class_also_has_its_own_because_block : RandomRunnerSpecs
    {
        Establish context = () =>
        {
            var type = GetRandom("parent_context_that_has_its_own_because_block+nested_context_that_has_a_because_block_which");

            ContextFactory.ChangeAllowedNumberOfBecauseBlocksTo(2);

            var factory = new ContextFactory();
            new_context = factory.CreateContextFrom(Activator.CreateInstance(type));
        };

        It should_be_able_to_be_created_successfully_if_a_testing_tool_has_specified_to_override_the_allowed_number_of_because_blocks = () =>
            new_context.ShouldNotBeNull();

        static Context new_context;
    }

    [Subject(typeof(ContextFactory))]
    public class when_creating_a_context_that_is_contained_within_another_context_class_without_concern : RandomRunnerSpecs
    {
        Establish context = () =>
        {
            var type = GetRandom("parent_context_without_concern+nested_context");

            var factory = new ContextFactory();
            new_context = factory.CreateContextFrom(Activator.CreateInstance(type));
        };

        It should_have_no_concern =
            () => new_context.Subject.ShouldBeNull();

        static Context new_context;
    }

    [Subject(typeof(ContextFactory))]
    public class when_creating_a_context_with_tags : RandomRunnerSpecs
    {
        static Context newContext;

        Establish context = () =>
        {
            var type = GetRandom("context_with_tags");

            var factory = new ContextFactory();
            newContext = factory.CreateContextFrom(Activator.CreateInstance(type));
        };

        It should_capture_the_tags =
            () => newContext.Tags.ShouldContainOnly(new Tag(tag2.example),
                new Tag(tag2.some_other_tag),
                new Tag(tag2.one_more_tag));
    }

    [Subject(typeof(ContextFactory))]
    public class when_creating_a_context_with_duplicate_tags : RandomRunnerSpecs
    {
        static Context newContext;

        Establish context = () =>
        {
            var type = GetRandom("context_with_duplicate_tags");

            var factory = new ContextFactory();
            newContext = factory.CreateContextFrom(Activator.CreateInstance(type));
        };

        It should_capture_the_tags_once =
            () => newContext.Tags.Count().ShouldEqual(1);
    }

    [Subject(typeof(ContextFactory))]
    public class when_creating_a_context_that_is_nested_inside_of_a_generic_class : RandomRunnerSpecs
    {
        public class and_the_nested_context_is_not_a_generic_type_definition
        {
            static Context newContext;

            Establish context = () =>
            {
                var type = GetRandom("generic_container`1+nested_context")
                    .MakeGenericType(typeof(int));

                var factory = new ContextFactory();
                newContext = factory.CreateContextFrom(Activator.CreateInstance(type));
            };

            It should_be_able_to_create_the_context_even_though_the_enclosing_class_is_generic =
                () => newContext.ShouldNotBeNull();
        }

        public class and_the_nested_context_is_a_generic_type_definition
        {
            static Context newContext;

            Establish context = () =>
            {
                var type = GetRandom("generic_container`1+nested_generic_context`1")
                    .MakeGenericType(typeof(int), typeof(string));

                var factory = new ContextFactory();
                newContext = factory.CreateContextFrom(Activator.CreateInstance(type));
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
                    var type = GetRandom("generic_container`1+nested_generic_context`1+nested_nested_non_generic")
                        .MakeGenericType(typeof(int), typeof(string));

                    var factory = new ContextFactory();
                    newContext = factory.CreateContextFrom(Activator.CreateInstance(type));
                };

                It should_be_able_to_create_the_context_even_though_the_enclosing_class_is_generic =
                    () => newContext.ShouldNotBeNull();
            }

            public class and_the_nested_context_is_generic
            {
                static Context newContext;

                Establish context = () =>
                {
                    var type = GetRandom("generic_container`1+nested_generic_context`1+nested_nested_generic`1")
                        .MakeGenericType(typeof(int), typeof(string), typeof(bool));

                    var factory = new ContextFactory();
                    newContext = factory.CreateContextFrom(Activator.CreateInstance(type));
                };

                It should_be_able_to_create_the_context_even_though_the_enclosing_class_is_generic =
                    () => newContext.ShouldNotBeNull();
            }
        }
    }

    [Subject(typeof(ContextFactory))]
    public class when_creating_behavior_specifications_and_tracking_original_behavior_field : RandomRunnerSpecs
    {
        static Context newContext;

        Establish context = () =>
        {
            var type = GetRandom("context_with_behaviors");

            var factory = new ContextFactory();
            newContext = factory.CreateContextFrom(Activator.CreateInstance(type));
        };

        It should_create_behavior_specs_with_original_behavior_field =
            () => newContext.Specifications
                .OfType<BehaviorSpecification>()
                .First()
                .BehaviorFieldInfo.Name.ShouldContainOnly("behavior");
    }

    [Subject(typeof(ContextFactory))]
    public class ContextFactoryTests : RandomRunnerSpecs
    {
        static Context single_context;
        static ContextFactory factory;

        Establish context = () =>
            factory = new ContextFactory();

        Because of = () =>
        {
            var type = GetFramework("ContextWithSingleSpecification");

            single_context = factory.CreateContextFrom(Activator.CreateInstance(type));
        };

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

        Establish first = () =>
            Numbers.Add(1);

        public class inner
        {
            Establish second = () =>
                Numbers.Add(2);
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
    public class when_creating_a_context_that_is_nested_inside_of_a_generic_class_hierarchy : RandomRunnerSpecs
    {
        class and_the_nested_context_is_not_a_generic_type_definition
        {
            static Context newContext;
            static ContextFactory factory;

            Establish context = () =>
                factory = new ContextFactory();

            Because of = () =>
            {
                var type = GetRandom("generic_container`1+nested_context")
                    .MakeGenericType(typeof(int));

                newContext = factory.CreateContextFrom(Activator.CreateInstance(type));
            };

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
            {
                var type = GetRandom("generic_container`1+nested_generic_context`1")
                    .MakeGenericType(typeof(int), typeof(string));

                newContext = factory.CreateContextFrom(Activator.CreateInstance(type));
            };

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
                {
                    var type = GetRandom("generic_container`1+nested_generic_context`1+nested_nested_non_generic")
                        .MakeGenericType(typeof(int), typeof(string));

                    newContext = factory.CreateContextFrom(Activator.CreateInstance(type));
                };

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
                {
                    var type = GetRandom("generic_container`1+nested_generic_context`1+nested_nested_generic`1")
                        .MakeGenericType(typeof(int), typeof(string), typeof(bool));

                    newContext = factory.CreateContextFrom(Activator.CreateInstance(type));
                };

                It should_be_able_to_create_the_context_even_though_the_enclosing_class_is_generic = () =>
                    newContext.ShouldNotBeNull();
            }
        }
    }
}
