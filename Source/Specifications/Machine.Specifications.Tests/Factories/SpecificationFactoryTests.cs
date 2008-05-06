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
  public class SpecificationFactory_CreateSpecification_FailureTests : TestsFor<SpecificationFactory>
  {

  }

  [TestFixture]
  public class SpecificationFactory_CreateSpecification : WithSingleSpecification
  {
    [Test]
    public void ShouldHaveCorrectItClause()
    {
      specification.Name.ShouldEqual("is a specification");
    }

    [Test]
    public void ShouldHaveFieldInfo()
    {
      specification.Field.Name.ShouldEqual("is_a_specification");
    }
  }

  [TestFixture]
  public class SpecificationFactory_CreateThrowSpecification : WithThrowSpecification
  {
    [Test]
    public void ShouldHaveCorrectItClause()
    {
      specification.Name.ShouldEqual("should throw an exception");
    }
  }

  public class WithSingleSpecification : TestsFor<SpecificationFactory>
  {
    protected Specification specification;
    public override void BeforeEachTest()
    {
      Type type = typeof(DescriptionWithSingleSpecification);
      FieldInfo field = type.GetPrivateFieldsWith(typeof(It)).First();

      specification = Target.CreateSpecification(new DescriptionWithSingleSpecification(), field);
    }
  }

  public class WithThrowSpecification : TestsFor<SpecificationFactory>
  {
    protected Specification specification;
    public override void BeforeEachTest()
    {
      Type type = typeof(DescriptionWithThrowingSpecification);
      FieldInfo field = type.GetPrivateFieldsWith(typeof(It_should_throw)).First();

      specification = Target.CreateSpecification(new DescriptionWithThrowingSpecification(), field);
    }
  }

}
