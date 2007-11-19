using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Castle.Facilities.DeferredServiceResolution
{
  [TestFixture]
  public class AttributedBasedRegistererTests : StandardFixture<AttributeBasedRegisterer>
  {
    [Test]
    public void Register_ThisAssembly_RegistersOurExampleTypes()
    {
      using (_mocks.Record())
      {
        _kernel.AddComponent(typeof(Service1And2).FullName, typeof(Service1And2));
        _kernel.AddComponent(typeof(Service1).FullName, typeof(Service1));
        _kernel.AddComponent(typeof(Service2).FullName, typeof(Service2));
        _kernel.AddComponent(typeof(Service3).FullName, typeof(Service3));
      }
      _target.RegisterAssembly(_kernel, GetType().Assembly);
      _mocks.VerifyAll();
    }

    [Test]
    [ExpectedException(typeof(InvalidOperationException))]
    public void RegisterType_IsInterface_Throws()
    {
      _target.RegisterType(_kernel, typeof(IService1));
    }

    [Test]
    [ExpectedException(typeof(InvalidOperationException))]
    public void RegisterType_IsAbstract_Throws()
    {
      _target.RegisterType(_kernel, typeof(SomethingAbstract));
    }

    public override AttributeBasedRegisterer Create()
    {
      return new AttributeBasedRegisterer();
    }
  }
}
