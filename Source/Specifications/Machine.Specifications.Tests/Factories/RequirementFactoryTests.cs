using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Machine.Specifications.Example;
using Machine.Specifications.Model;
using Machine.Testing;
using NUnit.Framework;

namespace Machine.Specifications.Factories
{
  public class RequirementFactory_CreateRequirement_FailureTests : TestsFor<RequirementFactory>
  {

  }

  [TestFixture]
  public class RequirementFactory_CreateRequirement : WithSingleRequirement
  {
    [Test]
    public void ShouldHaveCorrectItClause()
    {
      requirement.ItClause.ShouldEqual("is a requirement");
    }

    [Test]
    public void ShouldHaveFieldInfo()
    {
      requirement.Field.Name.ShouldEqual("is_a_requirement");
    }
  }

  [TestFixture]
  public class RequirementFactory_CreateThrowRequirement : WithThrowRequirement
  {
    [Test]
    public void ShouldHaveCorrectItClause()
    {
      requirement.ItClause.ShouldEqual("should throw an exception");
    }
  }

  public class WithSingleRequirement : TestsFor<RequirementFactory>
  {
    protected Requirement requirement;
    public override void BeforeEachTest()
    {
      Type type = typeof(SpecificationWithSingleRequirement);
      FieldInfo field = type.GetPrivateFieldsWith(typeof(It)).First();

      requirement = Target.CreateRequirement(new SpecificationWithSingleRequirement(), field);
    }
  }

  public class WithThrowRequirement : TestsFor<RequirementFactory>
  {
    protected Requirement requirement;
    public override void BeforeEachTest()
    {
      Type type = typeof(SpecificationWithThrowingRequirement);
      FieldInfo field = type.GetPrivateFieldsWith(typeof(It_should_throw)).First();

      requirement = Target.CreateRequirement(new SpecificationWithThrowingRequirement(), field);
    }
  }

}
