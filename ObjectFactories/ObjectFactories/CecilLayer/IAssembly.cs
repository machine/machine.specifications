using System;
using System.Collections.Generic;
using System.Reflection;

using Mono.Cecil;

namespace ObjectFactories.CecilLayer
{
  public interface IAssembly
  {
    TypeDefinitionCollection Types
    {
      get;
    }
    TypeReference Import(Type type);
    MethodReference Import(MethodInfo method);
    MethodReference Import(ConstructorInfo constructor);
    void Inject(TypeDefinition type);
    void Save(string path);
  }
}
