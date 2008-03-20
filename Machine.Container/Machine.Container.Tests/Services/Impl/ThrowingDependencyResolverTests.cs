using System;
using System.Collections.Generic;

using Machine.Container.Model;

using NUnit.Framework;

namespace Machine.Container.Services.Impl
{
  [TestFixture]
  public class ThrowingDependencyResolverTests : ScaffoldTests<ThrowingDependencyResolver>
  {
    [Test]
    [ExpectedException(typeof(PendingDependencyException))]
    public void Resolve_Always_Throws()
    {
      ServiceEntry serviceEntry = ServiceEntryHelper.NewEntry();
      ICreationServices creationServices = new CreationServices(Get<IActivatorStrategy>(), Get<IActivatorStore>(), Get<IOverrideLookup>());
      _target.ResolveDependency(creationServices, serviceEntry);
    }
  }
}