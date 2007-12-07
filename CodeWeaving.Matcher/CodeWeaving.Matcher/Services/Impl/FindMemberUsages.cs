using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CodeWeaving.Matcher.Services.Impl
{
  public class FindMemberUsages
  {
    public IEnumerable<MemberUsage> FindUsages(AssemblyDefinition assembly, MemberReference memberReference)
    {
      foreach (TypeDefinition type in assembly.MainModule.Types)
      {
        foreach (MethodDefinition method in type.Methods)
        {
          if (method.Body == null)
          {
            continue;
          }
          foreach (Instruction instruction in method.Body.Instructions)
          {
            if (instruction.Operand == memberReference)
            {
              yield return new MemberUsage(instruction, method);
            }
          }
        }
      }
    }
  }
}
