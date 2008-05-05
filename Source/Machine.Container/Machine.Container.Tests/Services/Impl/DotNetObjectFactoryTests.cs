using System;
using System.Collections.Generic;
using System.Reflection;

using Machine.Container.Model;
using Machine.Container.Services.Impl;

using NUnit.Framework;

namespace Machine.Container.Services.Impl
{
  [TestFixture]
  public class DotNetObjectFactoryTests : MachineContainerTestsFixture
  {
    #region Member Data
    private DotNetObjectFactory _factory;
    #endregion

    #region Test Setup and Teardown Methods
    public override void Setup()
    {
      base.Setup();
      _factory = new DotNetObjectFactory();
    }
    #endregion

    #region Test Methods
    [Test]
    public void CreateObject_NoParameters_CreatesInstance()
    {
      object instance = _factory.CreateObject(CreateCandidate(typeof(Service1)), new object[0]);
      Assert.IsNotNull(instance);
    }

    [Test]
    public void CreateObject_OneParameters_CreatesInstance()
    {
      object instance = _factory.CreateObject(CreateCandidate(typeof(Service1DependsOn2), typeof(IService2)), new object[] { null });
      Assert.IsNotNull(instance);
    }

    [Test]
    [ExpectedException(typeof(TargetParameterCountException))]
    public void CreateObject_OneParameterMismatched_Throws()
    {
      _factory.CreateObject(CreateCandidate(typeof(Service2DependsOn1), typeof(IService1)), new object[0]);
    }
    #endregion
  }
}