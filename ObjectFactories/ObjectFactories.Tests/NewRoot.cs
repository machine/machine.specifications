using System;
using System.Collections.Generic;

using Mono.Cecil;

using ObjectFactories.CecilLayer;

using Rhino.Mocks;

namespace ObjectFactories
{
  public class NewRoot
  {
    private MockRepository _mocks;

    public NewRoot(MockRepository mocks)
    {
      _mocks = mocks;
    }

    public TypeReferenceCreator TypeReference
    {
      get { return new TypeReferenceCreator(); }
    }

    public TypeDefinitionCreator TypeDefinition
    {
      get { return new TypeDefinitionCreator(); }
    }

    public AssemblyDefinitionCreator Assembly
    {
      get { return new AssemblyDefinitionCreator(_mocks); }
    }
  }
  public class Creator<TTarget>
  {
    protected TTarget _creation;

    public TTarget Creation
    {
      get { return _creation; }
    }

    public static implicit operator TTarget(Creator<TTarget> creator)
    {
      return creator.Creation;
    }
  }
  public class TypeReferenceCreator : Creator<TypeReference>
  {
    public TypeReferenceCreator()
    {
      _creation = new TypeReference("Type", "Namespace", null, false);
    }
  }
  public class TypeDefinitionCreator : Creator<TypeDefinition>
  {
    public TypeDefinitionCreator()
    {
      _creation = new TypeDefinition("Type", "Namespace", TypeAttributes.Public, null);
    }
  }
  public class AssemblyDefinitionCreator : Creator<IAssembly>
  {
    public AssemblyDefinitionCreator(MockRepository mocks)
    {
      _creation = mocks.DynamicMock<IAssembly>();
    }

    public AssemblyDefinitionCreator With(TypeDefinition type)
    {
      // _creation.Types.Add(type);
      return this;
    }
  }
}
