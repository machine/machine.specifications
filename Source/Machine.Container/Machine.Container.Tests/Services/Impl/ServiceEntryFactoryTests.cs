using System;
using System.Collections.Generic;

using Machine.Container.Model;

using NUnit.Framework;

namespace Machine.Container.Services.Impl
{
  [TestFixture]
  public class ServiceEntryFactoryTests : ScaffoldTests<ServiceEntryFactory>
  {
    [Test]
    public void CreateServiceEntry_Always_FillsInProperly()
    {
      ServiceEntry entry = _target.CreateServiceEntry(typeof(IService1), typeof(Service1DependsOn2), LifestyleType.Singleton);
      Assert.AreEqual(typeof(IService1), entry.ServiceType);
      Assert.AreEqual(typeof(Service1DependsOn2), entry.ImplementationType);
      Assert.AreEqual(LifestyleType.Singleton, entry.LifestyleType);
    }
  }
}