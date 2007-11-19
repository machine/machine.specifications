using System;
using System.Collections.Generic;

using Mono.Cecil;

using NUnit.Framework;

namespace ObjectFactories.Model
{
  [TestFixture]
  public class FactoryMapTests : ObjectFactoriesTests<FactoryMap>
  {
    public override FactoryMap Create()
    {
      return new FactoryMap();
    }

    [Test]
    public void AddFactory_Always_EnumeratesAFactoryInstance()
    {
      TypeReference t1 = New.TypeReference;
      TypeDefinition t2 = New.TypeDefinition;
      _target.AddFactory(t1, t2);
      foreach (Factory factory in _target.Factories)
      {
        Assert.AreEqual(t1, factory.ObjectType);
        Assert.AreEqual(t2, factory.FactoryType);
      }
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void AddFactory_ObjectAlreadyHasFactory_Throws()
    {
      TypeReference t1 = New.TypeReference;
      TypeDefinition t2 = New.TypeDefinition;
      _target.AddFactory(t1, t2);
      _target.AddFactory(t1, t2);
    }

    [Test]
    public void HasForFactoryType_ItDoesnt_IsFalse()
    {
      TypeDefinition t2 = New.TypeDefinition;
      Assert.IsFalse(_target.HasForFactoryType(t2));
    }

    [Test]
    public void HasForObjectType_ItDoesnt_IsFalse()
    {
      TypeReference t1 = New.TypeReference;
      Assert.IsFalse(_target.HasForObjectType(t1));
    }

    [Test]
    public void HasForFactoryType_ItDoes_IsTrue()
    {
      TypeReference t1 = New.TypeReference;
      TypeDefinition t2 = New.TypeDefinition;
      _target.AddFactory(t1, t2);
      Assert.IsTrue(_target.HasForFactoryType(t2));
    }

    [Test]
    public void HasForObjectType_ItDoes_IsTrue()
    {
      TypeReference t1 = New.TypeReference;
      TypeDefinition t2 = New.TypeDefinition;
      _target.AddFactory(t1, t2);
      Assert.IsTrue(_target.HasForObjectType(t1));
    }

    [Test]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void GetForFactoryType_ItDoesnt_Throws()
    {
      TypeDefinition t2 = New.TypeDefinition;
      _target.GetForFactoryType(t2);
    }

    [Test]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void GetForObjectType_ItDoesnt_Throws()
    {
      TypeReference t1 = New.TypeReference;
      _target.GetForObjectType(t1);
    }

    [Test]
    public void GetForFactoryType_ItDoes_IsExpected()
    {
      TypeReference t1 = New.TypeReference;
      TypeDefinition t2 = New.TypeDefinition;
      Factory factory = _target.AddFactory(t1, t2);
      Assert.AreEqual(factory, _target.GetForFactoryType(t2));
    }

    [Test]
    public void GetForObjectType_ItDoes_IsExpected()
    {
      TypeReference t1 = New.TypeReference;
      TypeDefinition t2 = New.TypeDefinition;
      Factory factory = _target.AddFactory(t1, t2);
      Assert.AreEqual(factory, _target.GetForObjectType(t1));
    }
  }
}
