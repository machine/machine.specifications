using System.Linq;
using Gallio.Common.Markup;
using Gallio.Model;
using Machine.Specifications.GallioAdapter.TestResources;
using NUnit.Framework;

namespace Machine.Specifications.GallioAdapter.Tests
{
  [TestFixture]
  public class SpecificationFailedDueToExceptionTests
  {
    Gallio.Runner.Reports.Schema.TestStepRun _run;

    [SetUp]
    public void Setup()
    {
      _run = GallioRunner.RunAllSpecificationsFor<at_any_given_moment>();
    }

    [Test]
    public void ShouldHaveFailedTheContext()
    {
      _run.Result.Outcome.ShouldEqual( TestOutcome.Failed);
    }

    [Test]
    public void ShouldHaveFailedTheOnlySpecification()
    {
      _run.Children.Count().ShouldEqual(1);
      _run.Children[0].Result.Outcome.ShouldEqual( TestOutcome.Failed);
    }

    [Test]
    public void ShouldWriteExceptionToTheFailureStream()
    {            
      var stream = _run.Children[0].TestLog.GetStream(MarkupStreamNames.Failures);
      stream.ShouldNotBeNull();
      string content = stream.ToString();
      content.ShouldContain("hi scott, love you, miss you");      
    }
  }

  [TestFixture]
  public class SpecificationFailedDueToAssertionTests
  {
    Gallio.Runner.Reports.Schema.TestStepRun _run;

    [SetUp]
    public void Setup()
    {
      _run = GallioRunner.RunAllSpecificationsFor<failing_specification_assertions>();
    }

    [Test]
    public void ShouldHaveFailedTheContext()
    {
      _run.Result.Outcome.ShouldEqual(TestOutcome.Failed);
    }

    [Test]
    public void ShouldHaveFailedTheChildSpecifications()
    {
      _run.Children.Count().ShouldEqual(3);
      _run.Children.All( child => child.Result.Outcome == TestOutcome.Failed).ShouldBeTrue();
    }

    [Test]
    public void ShouldWriteExceptionToTheFailureStreamForBooleanAssertion()
    {
      var child = _run.Children.Single(x => x.Step.Name == "failing boolean assertion");
      var stream = child.TestLog.GetStream(MarkupStreamNames.Failures);
      stream.ShouldNotBeNull();
      string content = stream.ToString();

      content.ShouldContain("Should");
      content.ShouldContain("false");
      content.ShouldContain("true");
    }

    [Test]
    public void ShouldWriteExceptionToTheFailureStreamForEqualityAssertion()
    {
      var child = _run.Children.Single(x => x.Step.Name == "failing equality assertion");
      var stream = child.TestLog.GetStream(MarkupStreamNames.Failures);
      stream.ShouldNotBeNull();
      string content = stream.ToString();      

      content.ShouldContain("Expected");
      content.ShouldContain("1");
      content.ShouldContain("2");
    }

    [Test]
    public void ShouldWriteExceptionToTheFailureStreamForContainsAssertion()
    {
      var child = _run.Children.Single(x => x.Step.Name == "failing contains assertion");
      var stream = child.TestLog.GetStream(MarkupStreamNames.Failures);
      stream.ShouldNotBeNull();
      string content = stream.ToString();       

      content.ShouldContain("Should");      
      content.ShouldContain("4");
    }
  }
}
