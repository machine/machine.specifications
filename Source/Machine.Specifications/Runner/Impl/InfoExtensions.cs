using System.Reflection;
using Machine.Specifications.Model;

namespace Machine.Specifications.Runner.Impl
{
  public static class InfoExtensions
  {
    public static AssemblyInfo GetInfo(this Assembly assembly)
    {
      return new AssemblyInfo(assembly.GetName().Name, assembly.Location);
    }

    public static ContextInfo GetInfo(this Context context)
    {
      string concern = "";
      if (context.Subject != null)
      {
        concern = context.Subject.FullConcern;
      }

      return new ContextInfo(context.Name, concern, context.Type.FullName, context.Type.Namespace, context.Type.Assembly.GetName().Name);
    }

    public static SpecificationInfo GetInfo(this Specification specification)
    {
      return new SpecificationInfo(specification.Name, specification.FieldInfo.DeclaringType.FullName, specification.FieldInfo.Name);
    }
  }
}