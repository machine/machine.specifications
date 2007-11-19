using System;
using System.Collections.Generic;

using NUnit.Framework;
using ObjectFactories.CecilLayer;
using ObjectFactories.Model;
using Rhino.Mocks;

namespace ObjectFactories.Services.Impl
{
  [TestFixture]
  public class ObjectFactoriesProcessorTests : ObjectFactoriesTests<ObjectFactoriesProcessor>
  {
    private IFactoryMapSerializer _factoryMapSerializer;
    private IAssemblies _assemblies;
    private IFactoryFinder _factoryFinder;
    private IConstructorCallFinder _constructorCallFinder;
    private IFactoryCallWeaver _factoryCallWeaver;
    private IAssembly _assembly;

    [Test]
    public void ProcessAssembly_Always_LoadsAssembly()
    {
      using (_mocks.Record())
      {
        Expect.Call(_assemblies.LoadAssembly("Assembly.dll")).Return(_assembly);
      }
      _target.ProcessAssembly("Assembly.dll");
      _mocks.VerifyAll();
    }

    [Test]
    public void ProcessAssembly_Always_SavesAssembly()
    {
      using (_mocks.Record())
      {
        Expect.Call(_assemblies.LoadAssembly("Assembly.dll")).Return(_assembly);
        _assembly.Save("Assembly.dll");
      }
      _target.ProcessAssembly("Assembly.dll");
      _mocks.VerifyAll();
    }

    [Test]
    public void ProcessAssembly_Always_AppliesStandardProcessing()
    {
      FactoryMap factoryMap = new FactoryMap();
      List<ConstructorCallWeave> weaves = new List<ConstructorCallWeave>();
      using (_mocks.Record())
      {
        Expect.Call(_assemblies.LoadAssembly("Assembly.dll")).Return(_assembly);
        Expect.Call(_factoryFinder.FindFactories(_assembly)).Return(factoryMap);
        Expect.Call(_constructorCallFinder.FindConstructorCallWeaves(_assembly, factoryMap)).Return(weaves);
        _factoryCallWeaver.WeaveConstructorCalls(weaves, factoryMap);
        _factoryMapSerializer.StoreFactoryMap(_assembly, factoryMap);
        _assembly.Save("Assembly.dll");
      }
      _target.ProcessAssembly("Assembly.dll");
      _mocks.VerifyAll();
    }

    public override ObjectFactoriesProcessor Create()
    {
      _assembly = _mocks.DynamicMock<IAssembly>();
      _assemblies = _mocks.DynamicMock<IAssemblies>();
      _factoryFinder = _mocks.DynamicMock<IFactoryFinder>();
      _constructorCallFinder = _mocks.DynamicMock<IConstructorCallFinder>();
      _factoryCallWeaver = _mocks.DynamicMock<IFactoryCallWeaver>();
      _factoryMapSerializer = _mocks.DynamicMock<IFactoryMapSerializer>();
      return new ObjectFactoriesProcessor(new NullLogger(), _assemblies, _factoryFinder, _constructorCallFinder, _factoryCallWeaver, _factoryMapSerializer);
    }
  }
}
