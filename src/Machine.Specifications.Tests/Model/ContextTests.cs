using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

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
      results.First().Exception.Should().NotBeNull();
    }

    [Test]
    public void ShouldFail()
    {
      results.First().Passed.Should().BeFalse();
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
      ContextWithThrowingWhenAndPassingSpecification.ItInvoked.Should().BeFalse();
    }

    [Test]
    public void ShouldFail()
    {
      results.First().Passed.Should().BeFalse();
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
      ContextWithEmptyWhen.ItInvoked.Should().BeTrue();
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
      ContextWithEmptySpecification.WhenInvoked.Should().BeFalse();
    }

    [Test]
    public void ShouldHaveNotImplementedResult()
    {
      results.First().Status.Should().Be(Status.NotImplemented);
    }
    
    [Test]
    public void ShouldHaveFailedResult()
    {
      results.First().Passed.Should().BeFalse();
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
      ContextWithSingleSpecification.BecauseInvoked.Should().BeTrue();
    }

    [Test]
    public void ShouldCallBeforeEach()
    {
      ContextWithSingleSpecification.ContextInvoked.Should().BeTrue();
    }

    [Test]
    public void ShouldCleanup()
    {
      ContextWithSingleSpecification.CleanupInvoked.Should().BeTrue();
    }
  }
}
