using System;
using System.Collections.Generic;

using Machine.Container.Model;

using NUnit.Framework;

namespace Machine.Container.Services.Impl
{
  [TestFixture]
  public class ServiceDependencyInspectorTests : BunkerTests
  {
    #region Member Data
    private ServiceDependencyInspector _inspector;
    #endregion

    #region Test Setup and Teardown Methods
    public override void Setup()
    {
      base.Setup();
      _inspector = new ServiceDependencyInspector();
    }
    #endregion

    #region Test Methods
    [Test]
    public void NoArguments_Always_Works()
    {
      List<ServiceDependency> actual = _inspector.SelectConstructor(typeof(ExampleNoArguments)).Dependencies;
      CollectionAssert.IsEmpty(actual);
    }

    [Test]
    [ExpectedException(typeof(InvalidOperationException))]
    public void TwoArgumentsPrivate_Always_Throws()
    {
      List<ServiceDependency> actual = _inspector.SelectConstructor(typeof(ExampleTwoArgumentsPrivate)).Dependencies;
      CollectionAssert.IsEmpty(actual);
    }

    [Test]
    public void OneArgument_Always_Works()
    {
      List<ServiceDependency> actual = _inspector.SelectConstructor(typeof(ExampleOneArgument)).Dependencies;
      Assert.AreEqual(1, actual.Count);
      Assert.AreEqual(typeof(IService1), actual[0].DependencyType);
    }

    [Test]
    public void TwoArguments_Always_Works()
    {
      List<ServiceDependency> actual = _inspector.SelectConstructor(typeof(ExampleTwoArguments)).Dependencies;
      Assert.AreEqual(2, actual.Count);
      Assert.AreEqual(typeof(IService1), actual[0].DependencyType);
      Assert.AreEqual(typeof(IService2), actual[1].DependencyType);
    }

    [Test]
    public void TwoWithSeparateArguments_FewerFirst_Works()
    {
      List<ServiceDependency> actual = _inspector.SelectConstructor(typeof(ExampleTwoWithSeparateArgument1)).Dependencies;
      Assert.AreEqual(1, actual.Count);
      Assert.AreEqual(typeof(IService1), actual[0].DependencyType);
    }

    [Test]
    public void TwoWithSeparateArguments_FewerLast_Works()
    {
      List<ServiceDependency> actual = _inspector.SelectConstructor(typeof(ExampleTwoWithSeparateArgument2)).Dependencies;
      Assert.AreEqual(1, actual.Count);
      Assert.AreEqual(typeof(IService1), actual[0].DependencyType);
    }

    [Test]
    [ExpectedException(typeof(InvalidOperationException))]
    public void TwoWithOneArgument_Always_Throws()
    {
      _inspector.SelectConstructor(typeof(ExampleTwoWithOneArgument));
    }

    [Test]
    [ExpectedException(typeof(InvalidOperationException))]
    public void AbstractType_Always_Throws()
    {
      _inspector.SelectConstructor(typeof(ExampleAbstract));
    }
    #endregion
  }

  public interface IService1
  {
  }

  public interface IService2
  {
  }

  public class ExampleNoArguments
  {
  }

  public class ExampleOneArgument
  {
    private readonly IService1 _s1;
    public ExampleOneArgument(IService1 s1)
    {
      _s1 = s1;
    }
  }

  public class ExampleTwoWithOneArgument
  {
    private readonly IService1 _s1;
    private readonly IService2 _s2;
    public ExampleTwoWithOneArgument(IService1 s1)
    {
      _s1 = s1;
    }
    public ExampleTwoWithOneArgument(IService2 s2)
    {
      _s2 = s2;
    }
  }

  public class ExampleTwoWithSeparateArgument1
  {
    private readonly IService1 _s1;
    private readonly IService2 _s2;
    public ExampleTwoWithSeparateArgument1(IService1 s1)
    {
      _s1 = s1;
    }
    public ExampleTwoWithSeparateArgument1(IService1 s1, IService2 s2)
    {
      _s1 = s1;
      _s2 = s2;
    }
  }

  public class ExampleTwoWithSeparateArgument2
  {
    private readonly IService1 _s1;
    private readonly IService2 _s2;
    public ExampleTwoWithSeparateArgument2(IService1 s1, IService2 s2)
    {
      _s1 = s1;
      _s2 = s2;
    }
    public ExampleTwoWithSeparateArgument2(IService1 s1)
    {
      _s1 = s1;
    }
  }

  public class ExampleTwoArguments
  {
    private readonly IService1 _s1;
    private readonly IService2 _s2;
    public ExampleTwoArguments(IService1 s1, IService2 s2)
    {
      _s1 = s1;
      _s2 = s2;
    }
  }

  public class ExampleTwoArgumentsPrivate
  {
    private readonly IService1 _s1;
    private readonly IService2 _s2;
    private ExampleTwoArgumentsPrivate(IService1 s1, IService2 s2)
    {
      _s1 = s1;
      _s2 = s2;
    }
  }

  public abstract class ExampleAbstract
  {
  }
}