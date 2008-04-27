using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications.Model;
using NUnit.Framework;

namespace Machine.Specifications.Factories
{
  [TestFixture]
  public class SpecificationFactoryTests : WithSpecificationWithSingleRequirement
  {
    [Test]
    public void ShouldSetType()
    {
      specification.Type.Name.ShouldEqual("SpecificationWithSingleRequirement");
    }
  }
}
