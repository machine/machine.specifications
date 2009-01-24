using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Machine.Specifications.Factories;
using Machine.Specifications.Model;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Explorers
{
  public class AssemblyExplorer
  {
    readonly ContextFactory contextFactory;

    public AssemblyExplorer()
    {
      contextFactory = new ContextFactory();
    }

    public IEnumerable<Context> FindContextsIn(Assembly assembly)
    {
      return EnumerateContextsIn(assembly).Select(x => CreateContextFrom(x));
    }

    public IEnumerable<Context> FindContextsIn(Assembly assembly, string targetNamespace)
    {
      return EnumerateContextsIn(assembly)
        .Where(x => x.Namespace == targetNamespace)
        .Select(x => CreateContextFrom(x));
    }

    public IEnumerable<IAssemblyContext> FindAssemblyContextsIn(Assembly assembly)
    {
      return assembly.GetExportedTypes()
        .Where(x =>
               x.GetInterfaces().Contains(typeof(IAssemblyContext)))
        .Select(x => (IAssemblyContext) Activator.CreateInstance(x));
    }

    Context CreateContextFrom(Type type)
    {
      object instance = Activator.CreateInstance(type);
      return contextFactory.CreateContextFrom(instance);
    }

    Context CreateContextFrom(Type type, FieldInfo fieldInfo)
    {
      object instance = Activator.CreateInstance(type);
      return contextFactory.CreateContextFrom(instance, fieldInfo);
    }

    static bool IsContext(Type type)
    {
      return HasSpecificationMembers(type);
    }

    static bool HasSpecificationMembers(Type type)
    {
      return !type.IsAbstract &&
             (type.GetPrivateFieldsWith(typeof(It)).Any() ||
              type.GetPrivateFieldsWith(typeof(It_should_behave_like)).Any());
    }

    static IEnumerable<Type> EnumerateContextsIn(Assembly assembly)
    {
      return assembly.GetExportedTypes().Where(IsContext)
        .OrderBy(t => t.Namespace);
    }

    public Context FindContexts(Type type)
    {
      if (IsContext(type))
      {
        return CreateContextFrom(type);
      }

      return null;
    }

    public Context FindContexts(FieldInfo info)
    {
      Type type = info.ReflectedType;
      if (IsContext(type))
      {
        return CreateContextFrom(type, info);
      }

      return null;
    }
  }
}