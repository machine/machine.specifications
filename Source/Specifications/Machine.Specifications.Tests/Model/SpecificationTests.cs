using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Machine.Specifications.Factories;
using Machine.Testing;
using NUnit.Framework;

namespace Machine.Specifications.Model
{
  [TestFixture]
  public class SpecificationTests : With<ContextWithSingleSpecification>
  {
    public override void BeforeEachTest()
    {
      base.BeforeEachTest();
      var results = context.VerifyAllSpecifications();
    }

    [Test]
    public void ShouldEstablishContext()
    {
      ContextWithSingleSpecification.BecauseInvoked.ShouldBeTrue();
    }

    [Test]
    public void ShouldCallBeforeEach()
    {
      ContextWithSingleSpecification.ContextInvoked.ShouldBeTrue();
    }

    [Test]
    public void ShouldCleanup()
    {
      ContextWithSingleSpecification.CleanupInvoked.ShouldBeTrue();
    }
  }

  public class With<T> : TestsFor<ContextFactory> where T : IFakeContext, new()
  {
    protected Context context;
    public override void BeforeEachTest()
    {
      IFakeContext fakeContext = new T();
      fakeContext.Reset();

      context = Target.CreateContextFrom(fakeContext);
    }
  }
}
