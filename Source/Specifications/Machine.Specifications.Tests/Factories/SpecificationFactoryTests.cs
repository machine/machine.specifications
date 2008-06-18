using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Machine.Specifications.Example;
using Machine.Specifications.Model;
using Machine.Specifications.Utility;
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
      specification.SpecificationField.Name.ShouldEqual("is_a_specification");
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

  [TestFixture]
  public  class SpecificationFactory_CreateUndefinedSpecification : WithUndefinedSpecification
  {
    [Test]
    public void ShouldCreateUnknownSpecification()
    {
      specification.IsDefined(new VerificationContext(new ContextWithEmptySpecification())).ShouldBeFalse();
    }
  }

  public class WithUndefinedSpecification : TestsFor<SpecificationFactory>
  {
    protected Specification specification;
    public override void BeforeEachTest()
    {
      Type type = typeof(ContextWithEmptySpecification);
      FieldInfo field = type.GetPrivateFieldsWith(typeof(It)).First();
      FieldInfo whenField = type.GetPrivateFieldsWith(typeof(Because)).First();

      specification = Target.CreateSpecification(field, whenField);
    }
  }

  public class WithSingleSpecification : TestsFor<SpecificationFactory>
  {
    protected Specification specification;
    public override void BeforeEachTest()
    {
      Type type = typeof(ContextWithSingleSpecification);
      FieldInfo field = type.GetPrivateFieldsWith(typeof(It)).First();
      FieldInfo whenField = type.GetPrivateFieldsWith(typeof(Because)).First();

      specification = Target.CreateSpecification(field, whenField);
    }
  }

  public class WithThrowSpecification : TestsFor<SpecificationFactory>
  {
    protected Specification specification;
    public override void BeforeEachTest()
    {
      Type type = typeof(ContextWithThrowingSpecification);
      FieldInfo field = type.GetPrivateFieldsWith(typeof(It)).First();
      FieldInfo whenField = type.GetPrivateFieldsWith(typeof(Because)).First();

      specification = Target.CreateSpecification(field, whenField);
    }
  }

}
