using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications.Factories;
using NUnit.Framework;

namespace Machine.Specifications.Model
{
  [TestFixture]
  public class Requirement_Verify : WithSingleRequirement
  {
    public override void BeforeEachTest()
    {
      base.BeforeEachTest();
      SpecificationWithSingleRequirement.ItInvoked = false;
      requirement.Verify(new VerificationContext());
    }

    [Test]
    public void ShouldHaveBeenInvoked()
    {
      SpecificationWithSingleRequirement.ItInvoked.ShouldBeTrue();
    }
  }
}
