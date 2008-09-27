using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications.Factories;
using Machine.Specifications.Model;

namespace Machine.Specifications.Specs.Factories
{
  [Subject(typeof(ContextFactory))]
  public class when_creating_a_context_with_a_concern
  {
    static Context newContext;

    Establish context = ()=>
    {
      var factory = new ContextFactory();
      newContext = factory.CreateContextFrom(new context_with_subject());
    };

    It should_capture_the_concerns_type = ()=>
      newContext.Subject.Type.ShouldEqual(typeof(int));

    It should_capture_the_concerns_description = ()=>
      newContext.Subject.Description.ShouldEqual("Some description");
  }

  [Subject(typeof(ContextFactory))]
  public class when_creating_a_context_with_tags
  {
    static Context newContext;

    Establish context = ()=>
    {
      var factory = new ContextFactory();
      newContext = factory.CreateContextFrom(new context_with_tags());
    };

    It should_capture_the_tags = ()=>
      newContext.Tags.ShouldContainOnly(new Tag(typeof(example)), new Tag(typeof(some_other_tag)), new Tag(typeof(one_more_tag)));
  }

  [Subject(typeof(ContextFactory))]
  public class when_creating_a_context_with_duplicate_tags
  {
    static Context newContext;

    Establish context = ()=>
    {
      var factory = new ContextFactory();
      newContext = factory.CreateContextFrom(new context_with_duplicate_tags());
    };

    It should_capture_the_tags_once = ()=>
      newContext.Tags.Count().ShouldEqual(1);
  }
}
