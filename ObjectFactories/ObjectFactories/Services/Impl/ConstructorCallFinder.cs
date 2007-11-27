using System;
using System.Collections.Generic;
using System.Reflection;

using Mono.Cecil;
using Mono.Cecil.Cil;

using ObjectFactories.CecilLayer;
using ObjectFactories.Model;

namespace ObjectFactories.Services.Impl
{
  public class ConstructorCallFinder : IConstructorCallFinder
  {
    #region Member Data
    private readonly ILog _log;
    #endregion

    #region ConstructorCallFinder()
    public ConstructorCallFinder(ILog log)
    {
      _log = log;
    }
    #endregion

    #region INewOperatorFinder Members
    public IEnumerable<ConstructorCallWeave> FindConstructorCallWeaves(IAssembly assembly, FactoryMap factories)
    {
      List<ConstructorCallWeave> spots = new List<ConstructorCallWeave>();
      foreach (TypeDefinition type in GetTypes(assembly, factories))
      {
        foreach (MethodDefinition method in type.Methods)
        {
          if (method.Body == null)
          {
            continue;
          }
          foreach (Instruction instruction in method.Body.Instructions)
          {
            if (instruction.OpCode == OpCodes.Newobj)
            {
              MethodDefinition constructor = instruction.Operand as MethodDefinition;
              if (constructor != null && constructor.Parameters.Count == 0)
              {
                if (factories.HasForObjectType(constructor.DeclaringType))
                {
                  spots.Add(new ConstructorCallWeave(assembly, method, instruction, constructor));
                }
              }
            }
          }
        }
      }
      return spots;
    }
    #endregion

    #region Private Methods
    private static IEnumerable<TypeDefinition> GetTypes(IAssembly assembly, FactoryMap factories)
    {
      foreach (TypeDefinition type in assembly.Types)
      {
        if (!factories.HasForFactoryType(type))
        {
          yield return type;
        }
      }
    }
    #endregion
  }
}
