using System;
using System.Linq;

using Example.Random;

using FluentAssertions;

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
      () => newContext.Subject.Type.Should().Be<int>();

    It should_capture_the_concerns_description =
      () => newContext.Subject.Description.Should().Be("Some description");
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
      () => newContext.Subject.Type.Should().Be<int>();
  }

  [Subject(typeof(ContextFactory))]
  public class when_using_nested_contexts
  {
    Establish context = () =>
    {
      outerContextRan = true;
    };

    public class and_the_parent_context_has_context_blocks
    {
      It should_run_the_parent_context_blocks_prior_to_the_nested_context_blocks =
        () => outerContextRan.Should().BeTrue();
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
      () => new_context.Specifications.Count().Should().Be(1);

    It should_take_the_subject_from_the_outer_class =
      () => new_context.Subject.FullConcern.Should().Be("Int32 Parent description");

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
      () => new_context.Subject.FullConcern.Should().Be("Int32 Some description");

    static Context new_context;
  }

  [Subject(typeof(ContextFactory))]
  public class when_creating_a_context_that_is_contained_within_another_context_class_and_owns_a_concern
  {
    Establish context = () =>
    {
      var factory = new ContextFactory();
      new_context = factory.CreateContextFrom(new parent_context.nested_context_inheriting_and_owning_a_concern());
    };

    It should_take_the_owned_concern =
      () => new_context.Subject.FullConcern.Should().Be("Int32 Nested description");

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

    It should_be_able_to_be_created_successfully_if_a_testing_tool_has_specified_to_override_the_allowed_number_of_because_blocks =
      () => new_context.Should().NotBeNull();

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
      () => new_context.Subject.Should().BeNull();

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
      () => newContext.Tags.Should().BeEquivalentTo(new Tag(tag2.example),
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
      () => newContext.Tags.Count().Should().Be(1);
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
        () => newContext.Should().NotBeNull();
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
        () => newContext.Should().NotBeNull();
    }

    public class and_there_are_multiple_generic_parents
    {
      public class and_the_nested_context_is_not_generic
      {
        static Context newContext;

        Establish context = () =>
        {
          var factory = new ContextFactory();
          newContext = factory.CreateContextFrom(new generic_container<int>.nested_generic_context<string>.nested_nested_non_generic());
        };

        It should_be_able_to_create_the_context_even_though_the_enclosing_class_is_generic =
          () => newContext.Should().NotBeNull();
      }

      public class and_the_nested_context_is_generic
      {
        static Context newContext;

        Establish context = () =>
        {
          var factory = new ContextFactory();
          newContext = factory.CreateContextFrom(new generic_container<int>.nested_generic_context<string>.nested_nested_generic<bool>());
        };

        It should_be_able_to_create_the_context_even_though_the_enclosing_class_is_generic =
          () => newContext.Should().NotBeNull();
      }
    }
  }
}
