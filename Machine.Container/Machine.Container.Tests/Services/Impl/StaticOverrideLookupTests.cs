using System;
using System.Collections.Generic;

using Machine.Container.Model;

using NUnit.Framework;

namespace Machine.Container.Services.Impl
{
  [TestFixture]
  public class StaticOverrideLookupTests : ScaffoldTests<StaticOverrideLookup>
  {
    #region Member Data
    private readonly ServiceEntry _serviceEntry = ServiceEntryHelper.NewEntry();
    #endregion

    #region Test Methods
    [Test]
    public void LookupOverride_NoneGiven_ReturnsNull()
    {
      _target = new StaticOverrideLookup(new object[0]);
      Assert.IsNull(_target.LookupOverride(_serviceEntry));
    }

    [Test]
    public void LookupOverride_NoneMatching_ReturnsNull()
    {
      _target = new StaticOverrideLookup(new object[] { "string", 10 });
      Assert.IsNull(_target.LookupOverride(_serviceEntry));
    }

    [Test]
    public void LookupOverride_OneMatching_ReturnsOverride()
    {
      Service1 service1 = new Service1();
      _target = new StaticOverrideLookup(new object[] { "string", service1, 10 });
      Assert.AreEqual(service1, _target.LookupOverride(_serviceEntry));
    }
    #endregion

    protected override StaticOverrideLookup Create()
    {
      return null;
    }
  }
}