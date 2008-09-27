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
      var results = context.Verify();
    }

    [Test]
    public void ShouldCallWhen()
    {
      ContextWithSingleSpecification.BecauseInvoked.ShouldBeTrue();
    }

    [Test]
    public void ShouldCallBeforeAll()
    {
      ContextWithSingleSpecification.BeforeAllInvoked.ShouldBeTrue();
    }

    [Test]
    public void ShouldCallBeforeEach()
    {
      ContextWithSingleSpecification.BeforeEachInvoked.ShouldBeTrue();
    }

    [Test]
    public void ShouldCallAfterEach()
    {
      ContextWithSingleSpecification.AfterEachInvoked.ShouldBeTrue();
    }

    [Test]
    public void ShouldCallAfterAll()
    {
      ContextWithSingleSpecification.AfterAllInvoked.ShouldBeTrue();
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
