using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Machine.Specifications.Model
{
  [TestFixture]
  public class ExpectingThrowButDoesntTests : With<ContextWithSpecificationExpectingThrowThatDoesnt>
  {
    IEnumerable<SpecificationVerificationResult> results;

    public override void BeforeEachTest()
    {
      base.BeforeEachTest();
      results = context.VerifyAllSpecifications();
    }

    [Test]
    public void ShouldHaveException()
    {
      results.First().Exception.ShouldNotBeNull();
    }

    [Test]
    public void ShouldFail()
    {
      results.First().Passed.ShouldBeFalse();
    }
  }

  [TestFixture]
  public class ThrowingWhenTests : With<ContextWithThrowingWhenAndPassingSpecification>
  {
    IEnumerable<SpecificationVerificationResult> results;

    public override void BeforeEachTest()
    {
      base.BeforeEachTest();
      results = context.VerifyAllSpecifications();
    }

    [Test]
    public void ShouldNotCallIt()
    {
      ContextWithThrowingWhenAndPassingSpecification.ItInvoked.ShouldBeFalse();
    }

    [Test]
    public void ShouldFail()
    {
      results.First().Passed.ShouldBeFalse();
    }
  }

  [TestFixture]
  public class EmptyContextTests : With<ContextWithEmptyWhen>
  {
    public override void BeforeEachTest()
    {
      base.BeforeEachTest();
      var results = context.VerifyAllSpecifications();
    }

    [Test]
    public void ShouldCallIt()
    {
      ContextWithEmptyWhen.ItInvoked.ShouldBeTrue();
    }
  }

  [TestFixture]
  public class EmptySpecificationTests : With<ContextWithEmptySpecification>
  {
    IEnumerable<SpecificationVerificationResult> results;

    public override void BeforeEachTest()
    {
      base.BeforeEachTest();
      results = context.VerifyAllSpecifications();
    }

    [Test]
    public void ShouldNotCallWhen()
    {
      ContextWithEmptySpecification.WhenInvoked.ShouldBeFalse();
    }

    [Test]
    public void ShouldHaveNotImplementedResult()
    {
      results.First().Status.ShouldEqual(Status.NotImplemented);
    }
    
    [Test]
    public void ShouldHaveFailedResult()
    {
      results.First().Passed.ShouldBeFalse();
    }
  }

  [TestFixture]
  public class ContextTests : With<ContextWithSingleSpecification>
  {
    public override void BeforeEachTest()
    {
      base.BeforeEachTest();
      var results = context.VerifyAllSpecifications();
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
}
