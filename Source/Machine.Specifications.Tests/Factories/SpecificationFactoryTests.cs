using System;
using System.Linq;
using System.Reflection;
using Machine.Specifications.Model;
using Machine.Specifications.Sdk;
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
      specification.FieldInfo.Name.ShouldEqual("is_a_specification");
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
      specification.IsDefined.ShouldBeFalse();
    }
  }

  public class WithUndefinedSpecification : TestsFor<SpecificationFactory>
  {
    protected Specification specification;
    public override void BeforeEachTest()
    {
      Type type = typeof(ContextWithEmptySpecification);
      FieldInfo field = type.GetInstanceFieldsOfUsage(new AssertDelegateAttributeFullName()).First();
      ContextFactory factory = new ContextFactory();
      var context = factory.CreateContextFrom(new ContextWithEmptySpecification());

      specification = Target.CreateSpecification(context, field);
    }
  }

  public class WithSingleSpecification : TestsFor<SpecificationFactory>
  {
    protected Specification specification;
    public override void BeforeEachTest()
    {
      Type type = typeof(ContextWithSingleSpecification);
      FieldInfo field = type.GetInstanceFieldsOfUsage(new AssertDelegateAttributeFullName()).First();
      ContextFactory factory = new ContextFactory();
      var context = factory.CreateContextFrom(new ContextWithSingleSpecification());

      specification = Target.CreateSpecification(context, field);
    }
  }

  public class WithThrowSpecification : TestsFor<SpecificationFactory>
  {
    protected Specification specification;
    public override void BeforeEachTest()
    {
      Type type = typeof(ContextWithThrowingSpecification);
      FieldInfo field = type.GetInstanceFieldsOfUsage(new AssertDelegateAttributeFullName()).First();
      ContextFactory factory = new ContextFactory();
      var context = factory.CreateContextFrom(new ContextWithThrowingSpecification());

      specification = Target.CreateSpecification(context, field);
    }
  }

}
