using System;
using System.Collections.Generic;

using Mono.Cecil;

using ObjectFactories.CecilLayer;
using ObjectFactories.Model;

using NUnit.Framework;

namespace ObjectFactories.Services.Impl
{
  [TestFixture]
  public class FactoryFinderTests : ObjectFactoriesTests<FactoryFinder>
  {
    public override FactoryFinder Create()
    {
      return new FactoryFinder(new NullLogger());
    }

    [Test]
    public void FindFactories_NoFactories_IsEmpty()
    {
      IAssembly assembly = MockAssemblyFactory.CreateMockAssembly();
      _mocks.ReplayAll();
      FactoryMap map = _target.FindFactories(assembly);
      EnumerableAssert.IsEmpty(map.Factories);
    }

    [Test]
    public void FindFactories_HasObjectFactory_BuildsProperFactoryMap()
    {
      IAssembly assembly = MockAssemblyFactory.CreateMockAssembly(typeof(MyObjectFactory), typeof(MyObject));
      _mocks.ReplayAll();
      FactoryMap map = _target.FindFactories(assembly);
      TypeDefinition myObjectFactory = AssemblyHelper.GetType(assembly, typeof(MyObjectFactory).FullName);
      Assert.IsTrue(map.HasForFactoryType(myObjectFactory));
    }
  }
}
