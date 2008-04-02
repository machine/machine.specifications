using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Machine.Core.Services.Impl
{
  [TestFixture]
  public class DotNetObjectActivatorTests : StandardFixture<DotNetObjectActivator>
  {
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void LookupAndActivate_BadTypeName_Throws()
    {
      _target.LookupAndActivate("SomeTypeThatDoesNotExist, Whatever");
    }

    [Test]
    public void LookupAndActivate_GoodType_Creates()
    {
      ClassWithDefaultConstructor value = _target.LookupAndActivate<ClassWithDefaultConstructor>("Machine.Core.Services.Impl.ClassWithDefaultConstructor, Machine.Core.Tests");
      Assert.IsNotNull(value);
    }

    [Test]
    public void ActivateGeneric_GoodType_Creates()
    {
      ClassWithDefaultConstructor value = _target.Activate<ClassWithDefaultConstructor>();
      Assert.IsNotNull(value);
    }

    [Test]
    public void ActivateJustType_GoodType_Creates()
    {
      object value = _target.Activate(typeof(ClassWithDefaultConstructor));
      Assert.IsNotNull(value);
    }

    [Test]
    public void ActivateJustTypeAndArguments_GoodType_Creates()
    {
      ClassWithConstructorParameter value = _target.Activate<ClassWithConstructorParameter>("Hello");
      Assert.IsNotNull(value);
    }

    public override DotNetObjectActivator Create()
    {
      return new DotNetObjectActivator();
    }
  }
  public class ClassWithDefaultConstructor
  {
  }
  public class ClassWithConstructorParameter
  {
    private string _name;
    public ClassWithConstructorParameter(string name)
    {
      _name = name;
    }
  }
}
