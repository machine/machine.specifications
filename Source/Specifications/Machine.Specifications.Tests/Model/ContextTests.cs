using System.Collections.Generic;
using System.Linq;
using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;
using NUnit.Framework;

namespace Machine.Specifications.Model
{
  [TestFixture]
  public class ExpectingThrowButDoesntTests : With<ContextWithSpecificationExpectingThrowThatDoesnt>
  {
    IEnumerable<Result> results;

    public override void BeforeEachTest()
    {
      base.BeforeEachTest();
      results = Run(context);
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
    IEnumerable<Result> results;

    public override void BeforeEachTest()
    {
      base.BeforeEachTest();
      results = Run(context);
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
      Run(context);
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
    IEnumerable<Result> results;

    public override void BeforeEachTest()
    {
      base.BeforeEachTest();
      results = Run(context);
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
      
      Run(context);
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
}
