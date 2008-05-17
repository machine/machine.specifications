using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Machine.Specifications.Factories;
using Machine.Specifications.Model;

namespace Machine.Specifications.Explorers
{
  public class AssemblyExplorer
  {
    private readonly DescriptionFactory _descriptionFactory;

    public AssemblyExplorer()
    {
      _descriptionFactory = new DescriptionFactory();
    }

    public IEnumerable<Description> FindDescriptionsIn(Assembly assembly)
    {
      return EnumerateDescriptionsIn(assembly).Select(x => CreateDescriptionFrom(x));
    }

    public IEnumerable<Description> FindDescriptionsIn(Assembly assembly, string targetNamespace)
    {
      return EnumerateDescriptionsIn(assembly)
        .Where(x => x.Namespace == targetNamespace)
        .Select(x => CreateDescriptionFrom(x));
    }

    private Description CreateDescriptionFrom(Type type)
    {
      object instance = Activator.CreateInstance(type);
      return _descriptionFactory.CreateDescriptionFrom(instance);
    }

    private Description CreateDescriptionFrom(Type type, FieldInfo fieldInfo)
    {
      object instance = Activator.CreateInstance(type);
      return _descriptionFactory.CreateDescriptionFrom(instance, fieldInfo);
    }

    private static bool IsDescription(Type type)
    {
      return HasSpecificationMembers(type);
    }

    private static bool HasSpecificationMembers(Type type)
    {
      return type.GetPrivateFieldsWith(typeof(It)).Any() || type.GetPrivateFieldsWith(typeof(It_should_throw)).Any();
    }

    /*
    private static bool HasDescriptionAttribute(Type type)
    {
      return type.IsDefined(typeof(DescriptionAttribute), false);
    }
    */

    private static IEnumerable<Type> EnumerateDescriptionsIn(Assembly assembly)
    {
      return assembly.GetExportedTypes().Where(IsDescription);
    }

    public Description FindDescription(Type type)
    {
      if (IsDescription(type))
      {
        return CreateDescriptionFrom(type);
      }

      return null;
    }

    public Description FindDescription(FieldInfo info)
    {
      Type type = info.ReflectedType;
      if (IsDescription(type))
      {
        return CreateDescriptionFrom(type, info);
      }

      return null;
    }
  }
}
