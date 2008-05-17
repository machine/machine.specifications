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
  public class DescriptionWithTwoWhensTests : With<DescriptionWithTwoWhens>
  {
    [Test]
    public void ShouldHaveTwoSpecifications()
    {
      description.Specifications.Count().ShouldEqual(2);
    }

    [Test]
    public void ShouldHaveTwoDifferentWhens()
    {
      //description.Specifications.Select()
    }
  }

  [TestFixture]
  public class ExpectingThrowButDoesntTests : With<DescriptionWithSpecificationExpectingThrowThatDoesnt>
  {
    DescriptionVerificationResult results;

    public override void BeforeEachTest()
    {
      base.BeforeEachTest();
      results = description.Verify();
    }

    [Test]
    public void ShouldHaveException()
    {
      results.SpecificationResults.First().Exception.ShouldNotBeNull();
    }

    [Test]
    public void ShouldFail()
    {
      results.SpecificationResults.First().Passed.ShouldBeFalse();
    }
  }

  [TestFixture]
  public class ThrowingWhenTests : With<DescriptionWithThrowingWhenAndPassingSpecification>
  {
    DescriptionVerificationResult results;

    public override void BeforeEachTest()
    {
      base.BeforeEachTest();
      results = description.Verify();
    }

    [Test]
    public void ShouldNotCallIt()
    {
      DescriptionWithThrowingWhenAndPassingSpecification.ItInvoked.ShouldBeFalse();
    }

    [Test]
    public void ShouldFail()
    {
      results.SpecificationResults.First().Passed.ShouldBeFalse();
    }
  }

  [TestFixture]
  public class EmptyDescriptionTests : With<DescriptionWithEmptyWhen>
  {
    public override void BeforeEachTest()
    {
      base.BeforeEachTest();
      var results = description.Verify();
    }

    [Test]
    public void ShouldCallIt()
    {
      DescriptionWithEmptyWhen.ItInvoked.ShouldBeTrue();
    }
  }

  [TestFixture]
  public class EmptySpecificationTests : With<DescriptionWithEmptySpecification>
  {
    DescriptionVerificationResult results;

    public override void BeforeEachTest()
    {
      base.BeforeEachTest();
      results = description.Verify();
    }

    [Test]
    public void ShouldNotCallWhen()
    {
      DescriptionWithEmptySpecification.WhenInvoked.ShouldBeFalse();
    }

    [Test]
    public void ShouldHaveUnknownResult()
    {
      results.SpecificationResults.First().Result.ShouldEqual(Result.Unknown);
    }
    
    [Test]
    public void ShouldHaveFailedResult()
    {
      results.SpecificationResults.First().Passed.ShouldBeFalse();
    }
  }

  [TestFixture]
  public class DescriptionTests : With<DescriptionWithSingleSpecification>
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
}
