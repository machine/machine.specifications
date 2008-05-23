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
  public class SpecificationTests : With<DescriptionWithSingleSpecification>
  {
    public override void BeforeEachTest()
    {
      base.BeforeEachTest();
      var results = context.Verify();
    }

    [Test]
    public void ShouldCallWhen()
    {
      DescriptionWithSingleSpecification.WhenInvoked.ShouldBeTrue();
    }

    [Test]
    public void ShouldCallBeforeAll()
    {
      DescriptionWithSingleSpecification.BeforeAllInvoked.ShouldBeTrue();
    }

    [Test]
    public void ShouldCallBeforeEach()
    {
      DescriptionWithSingleSpecification.BeforeEachInvoked.ShouldBeTrue();
    }

    [Test]
    public void ShouldCallAfterEach()
    {
      DescriptionWithSingleSpecification.AfterEachInvoked.ShouldBeTrue();
    }

    [Test]
    public void ShouldCallAfterAll()
    {
      DescriptionWithSingleSpecification.AfterAllInvoked.ShouldBeTrue();
    }
  }

  public class With<T> : TestsFor<ContextFactory> where T : IFakeDescription, new()
  {
    protected Context context;
    public override void BeforeEachTest()
    {
      IFakeDescription fakeDescription = new T();
      fakeDescription.Reset();

      context = Target.CreateContextFrom(fakeDescription);
    }
  }
}
