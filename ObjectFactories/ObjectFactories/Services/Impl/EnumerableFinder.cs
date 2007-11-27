using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using ObjectFactories.CecilLayer;

namespace ObjectFactories.Services.Impl
{
  public class EnumerableFinder
  {
    #region Member Data
    private readonly ILog _log;
    #endregion

    #region EnumerableFinder()
    public EnumerableFinder(ILog log)
    {
      _log = log;
    }
    #endregion

    #region IEnumerableFinder Members
    public IEnumerable<object> FindEnumerables(IAssembly assembly)
    {
      foreach (TypeDefinition type in assembly.Types)
      {
        foreach (PropertyDefinition property in type.Properties)
        {
          if (property.GetMethod == null)
          {
             continue;
          }
          if (property.PropertyType.Name == "IEnumerable`1")
          {
            property.PropertyType = ChangeReturnType(assembly, property.PropertyType as GenericInstanceType);
            if (property.GetMethod.Body != null)
            {
              VariableDefinition returnedVariable = IsReturningLocalVariable(property.GetMethod.Body);
              if (returnedVariable != null)
              {
                returnedVariable.VariableType = property.PropertyType;
              }
            }
            property.GetMethod.ReturnType.ReturnType = property.PropertyType;
          }
        }
      }
      return null;
    }
    #endregion

    private static TypeReference ChangeReturnType(IAssembly assembly, GenericInstanceType oldType)
    {
      GenericInstanceType newType = assembly.Import(typeof(List<string>)) as GenericInstanceType;
      newType.GenericArguments.Clear();
      newType.GenericArguments.Add(oldType.GenericArguments[0]);
      return newType;
    }

    private static VariableDefinition IsReturningLocalVariable(MethodBody body)
    {
      Stack<Instruction> stack = new Stack<Instruction>();
      foreach (Instruction i in body.Instructions)
      {
        stack.Push(i);
      }
      Instruction instruction = stack.Pop();
      Console.WriteLine("{0}", instruction.OpCode);
      if (instruction.OpCode != OpCodes.Ret)
      {
        return null;
      }
      instruction = stack.Pop();
      Console.WriteLine("{0} {1}", instruction.OpCode, instruction.OpCode == OpCodes.Ldloc);
      if (!IsLoadLocal(instruction.OpCode))
      {
        return null;
      }
      foreach (VariableDefinition variable in body.Variables)
      {
        Console.WriteLine("{0} {1}", variable.VariableType, variable.Name);
        if (variable.Index == 0)
          return variable;
      }
      return null;
    }

    private static bool IsLoadLocal(OpCode opcode)
    {
      return opcode == OpCodes.Ldloc || opcode == OpCodes.Ldloc_0 || opcode == OpCodes.Ldloc_1 || 
             opcode == OpCodes.Ldloc_2 || opcode == OpCodes.Ldloc_3;
    }
  }
}
