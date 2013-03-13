using System.Linq;

using Example.Random;

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
    Establish context = () =>
    {
      outerContextRan = true;
    };

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
      new_context = factory.CreateContextFrom(new parent_context.nested_context_inheriting_and_owning_a_concern());
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

    It should_be_able_to_be_created_successfully_if_a_testing_tool_has_specified_to_override_the_allowed_number_of_because_blocks =
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
}