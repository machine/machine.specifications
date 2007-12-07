using System;
using System.Collections.Generic;

using CodeWeaving.Matcher.Matchers;

using Mono.Cecil;

namespace CodeWeaving.Matcher.Services.Impl
{
  public class MemberFinder
  {
    public IEnumerable<MemberReference> FindMembers(AssemblyDefinition assembly, IMemberMatcher matcher)
    {
      foreach (TypeDefinition type in assembly.MainModule.Types)
      {
        foreach (PropertyDefinition property in type.Properties)
        {
          if (matcher.Includes(property))
          {
            yield return property;
          }
        }
        foreach (MethodDefinition method in type.Methods)
        {
          if (matcher.Includes(method))
          {
            yield return method;
          }
        }
      }
    }
  }
}
