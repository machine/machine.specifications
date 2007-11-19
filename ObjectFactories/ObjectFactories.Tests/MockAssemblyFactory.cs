using System;
using System.Collections.Generic;
using System.Diagnostics;

using Mono.Cecil;

using ObjectFactories.CecilLayer;

using Rhino.Mocks;

namespace ObjectFactories
{
  public class MockAssemblyFactory
  {
    private static AssemblyDefinition _testingAssembly;
    private readonly MockRepository _mocks;

    public MockAssemblyFactory(MockRepository mocks)
    {
      _mocks = mocks;
    }

    protected static AssemblyDefinition TestingAssembly
    {
      get
      {
        if (_testingAssembly == null)
        {
          _testingAssembly = AssemblyFactory.GetAssembly(typeof(MockAssemblyFactory).Assembly.Location);
        }
        return _testingAssembly;
      }
    }

    public IAssembly CreateMockAssembly(params Type[] include)
    {
      IAssembly assembly = _mocks.DynamicMock<IAssembly>();
      TypeDefinitionCollection mockTypes = new TypeDefinitionCollection(TestingAssembly.MainModule);
      foreach (TypeDefinition type in TestingAssembly.MainModule.Types)
      {
        foreach (Type reflected in include)
        {
          if (reflected.FullName == type.FullName)
          {
            mockTypes.Add(type.Clone());
          }
        }
      }
      SetupResult.For(assembly.Types).Return(mockTypes);
      return assembly;
    }
  }
  public class IncludeInMockAssemblyAttribute : Attribute
  {
  }
  public static class AssemblyHelper
  {
    public static TypeDefinition GetType(IAssembly assembly, string name)
    {
      foreach (TypeDefinition typeDefinition in assembly.Types)
      {
        if (typeDefinition.FullName == name)
        {
          return typeDefinition;
        }
      }
      return null;
    }
  }
}
