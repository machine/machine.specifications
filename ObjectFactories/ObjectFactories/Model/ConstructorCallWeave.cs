using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Cecil.Cil;

using ObjectFactories.CecilLayer;

namespace ObjectFactories.Model
{
  public class ConstructorCallWeave
  {
    private readonly IAssembly _parentAssembly;
    private readonly MethodDefinition _parentMethod;
    private readonly Instruction _newInstruction;
    private readonly MethodDefinition _constructor;

    public IAssembly ParentAssembly
    {
      get { return _parentAssembly; }
    }

    public MethodDefinition ParentMethod
    {
      get { return _parentMethod; }
    }

    public MethodDefinition Constructor
    {
      get { return _constructor; }
    }

    public Instruction NewInstruction
    {
      get { return _newInstruction; }
    }

    public ConstructorCallWeave(IAssembly parentAssembly, MethodDefinition parentMethod, Instruction newInstruction, MethodDefinition constructor)
    {
      _parentAssembly = parentAssembly;
      _parentMethod = parentMethod;
      _constructor = constructor;
      _newInstruction = newInstruction;
    }
  }
}
