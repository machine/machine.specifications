using System;
using System.Collections.Generic;

using Machine.Container.Model;

using NUnit.Framework;

namespace Machine.Container.Services.Impl
{
  [TestFixture]
  public class ServiceGraphTests : ScaffoldTests<ServiceGraph>
  {
    [Test]
    public void Lookup_NotInTheGraph_IsNull()
    {
      Assert.IsNull(_target.Lookup(typeof(IService1)));
    }

    [Test]
    public void Lookup_InTheGraph_IsThatEntry()
    {
      ServiceEntry entry = ServiceEntryHelper.NewEntry();
      _target.Add(entry);
      Assert.AreEqual(entry, _target.Lookup(typeof(IService1)));
    }

    [Test]
    public void Lookup_SubclassInGraph_IsThatEntry()
    {
      ServiceEntry entry = ServiceEntryHelper.NewEntry(typeof(Service1DependsOn2));
      _target.Add(entry);
      Assert.AreEqual(entry, _target.Lookup(typeof(IService1)));
    }

    [Test]
    [ExpectedException(typeof(AmbiguousServicesException))]
    public void Lookup_MultipleSubclassesInGraph_Throws()
    {
      _target.Add(ServiceEntryHelper.NewEntry(typeof(Service1DependsOn2)));
      _target.Add(ServiceEntryHelper.NewEntry(typeof(Service1)));
      _target.Lookup(typeof(IService1));
    }
  }
}