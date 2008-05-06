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
      var results = description.Verify();
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

  public class With<T> : TestsFor<DescriptionFactory> where T : IFakeDescription, new()
  {
    protected Description description;
    public override void BeforeEachTest()
    {
      IFakeDescription fakeDescription = new T();
      fakeDescription.Reset();

      description = Target.CreateDescriptionFrom(fakeDescription);
    }
  }
}
