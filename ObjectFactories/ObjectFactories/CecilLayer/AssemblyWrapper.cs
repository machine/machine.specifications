using System;
using System.Collections.Generic;
using System.Reflection;

using Mono.Cecil;

namespace ObjectFactories.CecilLayer
{
  public class AssemblyWrapper : IAssembly
  {
    #region Member Data
    private readonly AssemblyDefinition _assembly;
    #endregion

    #region AssemblyWrapper()
    public AssemblyWrapper(AssemblyDefinition assembly)
    {
      _assembly = assembly;
    }
    #endregion

    #region IAssembly Members
    public TypeDefinitionCollection Types
    {
      get { return _assembly.MainModule.Types;  }
    }

    public void Save(string path)
    {
      AssemblyFactory.SaveAssembly(_assembly, path);
    }

    public TypeReference Import(Type type)
    {
      return _assembly.MainModule.Import(type);
    }

    public MethodReference Import(MethodInfo method)
    {
      return _assembly.MainModule.Import(method);
    }

    public MethodReference Import(ConstructorInfo constructor)
    {
      return _assembly.MainModule.Import(constructor);
    }

    public void Inject(TypeDefinition type)
    {
      _assembly.MainModule.Inject(type);
    }
    #endregion
  }
}
