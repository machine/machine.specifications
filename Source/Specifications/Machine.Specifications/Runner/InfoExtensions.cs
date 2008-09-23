using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Machine.Specifications.Model;

namespace Machine.Specifications.Runner
{
  public static class InfoExtensions
  {
    public static AssemblyInfo GetInfo(this Assembly assembly)
    {
      return new AssemblyInfo(assembly.GetName().Name);
    }

    public static ContextInfo GetInfo(this Context context)
    {
      return new ContextInfo(context.Name, context.Concern.FullConcern);
    }

    public static SpecificationInfo GetInfo(this Specification specification)
    {
      return new SpecificationInfo(specification.Name);
    }
  }
}
