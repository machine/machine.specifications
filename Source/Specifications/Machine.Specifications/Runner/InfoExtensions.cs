using System.Reflection;
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
      string concern = "";
      if (context.Subject != null)
      {
        concern = context.Subject.FullConcern;
      }

      return new ContextInfo(context.Name, concern);
    }

    public static SpecificationInfo GetInfo(this Specification specification)
    {
      return new SpecificationInfo(specification.Name);
    }
  }
}
