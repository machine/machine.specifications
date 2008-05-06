using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications.Factories;
using NUnit.Framework;

namespace Machine.Specifications.Model
{
  [TestFixture]
  public class Specification_Verify : WithSingleSpecification
  {
    public override void BeforeEachTest()
    {
      base.BeforeEachTest();
      DescriptionWithSingleSpecification.ItInvoked = false;
      specification.Verify(new VerificationContext());
    }

    [Test]
    public void ShouldHaveBeenInvoked()
    {
      DescriptionWithSingleSpecification.ItInvoked.ShouldBeTrue();
    }
  }
}
