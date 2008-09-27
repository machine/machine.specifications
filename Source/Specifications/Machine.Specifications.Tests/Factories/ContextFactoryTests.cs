using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications.Model;
using NUnit.Framework;

namespace Machine.Specifications.Factories
{
  [TestFixture]
  public class ContextFactoryTests : With<ContextWithSingleSpecification>
  {
    [Test]
    public void ShouldSetType()
    {
      context.Type.Name.ShouldEqual("ContextWithSingleSpecification");
    }
  }

  [Subject(typeof(ContextFactory))]
  public class when_creating_a_context_with_a_concern
  {
    static Context newContext;

    Establish context = ()=>
    {
      var factory = new ContextFactory();
      newContext = factory.CreateContextFrom(new ContextWithConcern());
    };

    It should_capture_the_concerns_type = ()=>
      newContext.Subject.Type.ShouldEqual(typeof(int));

    It should_capture_the_concerns_description = ()=>
      newContext.Subject.Description.ShouldEqual("Some description");
  }
}
