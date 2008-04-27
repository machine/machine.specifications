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
  public class SpecificationTests : WithSpecificationWithSingleRequirement
  {
    public override void BeforeEachTest()
    {
      base.BeforeEachTest();
      var results = specification.Verify();
    }

    [Test]
    public void ShouldCallWhen()
    {
      SpecificationWithSingleRequirement.WhenInvoked.ShouldBeTrue();
    }
  }

  public class WithSpecificationWithSingleRequirement : TestsFor<SpecificationFactory>
  {
    protected Specification specification;
    public override void BeforeEachTest()
    {
      specification = Target.CreateSpecificationFrom(new SpecificationWithSingleRequirement());
    }
  }
}
