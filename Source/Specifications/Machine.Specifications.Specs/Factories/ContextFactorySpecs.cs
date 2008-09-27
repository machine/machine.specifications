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
}
