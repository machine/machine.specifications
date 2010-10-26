#if RESHARPER_5
using System.Collections.Generic;
using System.Text.RegularExpressions;
#endif
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;

using Machine.Specifications.ReSharperRunner.Presentation;

namespace Machine.Specifications.ReSharperRunner.Factories
{
  internal class BehaviorFactory
  {
    readonly ProjectModelElementEnvoy _projectEnvoy;
    readonly IUnitTestProvider _provider;
    readonly ContextCache _cache;
#if RESHARPER_5
    static readonly IDictionary<string, BehaviorElement> TypeCache = new Dictionary<string, BehaviorElement>();
#endif

    public BehaviorFactory(IUnitTestProvider provider, ProjectModelElementEnvoy projectEnvoy, ContextCache cache)
    {
      _provider = provider;
      _cache = cache;
      _projectEnvoy = projectEnvoy;
    }

    public BehaviorElement CreateBehavior(IDeclaredElement field)
    {
      IClass clazz = field.GetContainingType() as IClass;
      if (clazz == null)
      {
        return null;
      }

      ContextElement context;
      _cache.Classes.TryGetValue(clazz, out context);
      if (context == null)
      {
        return null;
      }

#if RESHARPER_5
      if (field is ITypeOwner)
      {
        // Work around the difference in how the MetaData API and Psi API return different type strings for generics.
        string typeName = ((ITypeOwner) field).Type.ToString();
        typeName = typeName.Substring(typeName.IndexOf("-> ") + 3);
        typeName = typeName.Remove(typeName.Length - 1);
        typeName = Regex.Replace(typeName, @"\[.*->\s", "[");

        BehaviorElement behaviorElement;
        if (TypeCache.TryGetValue(typeName, out behaviorElement))
          return behaviorElement;
      }
#endif

      return new BehaviorElement(_provider,
                                 context,
                                 _projectEnvoy,
                                 clazz.CLRName,
                                 field.ShortName,
                                 field.IsIgnored(),
                                 null);
    }

    public BehaviorElement CreateBehavior(ContextElement context, IMetadataField behavior)
    {
      IMetadataTypeInfo typeContainingBehaviorSpecifications = behavior.GetFirstGenericArgument();

      string fullyQualifiedTypeName = null;
        
#if RESHARPER_5
      fullyQualifiedTypeName = behavior.FirstGenericArgumentClass().FullyQualifiedName();
      string typeName = Regex.Replace(fullyQualifiedTypeName, @"\,.+]", "]");
      typeName = Regex.Replace(typeName, @"\[\[", "[");
#endif

      var behaviorElement = new BehaviorElement(_provider,
                                 context,
                                 _projectEnvoy,
                                 behavior.DeclaringType.FullyQualifiedName,
                                 behavior.Name,
                                 behavior.IsIgnored() || typeContainingBehaviorSpecifications.IsIgnored(),
                                 fullyQualifiedTypeName);

#if RESHARPER_5
      if (!TypeCache.ContainsKey(typeName))
        TypeCache.Add(typeName, behaviorElement);
#endif

      return behaviorElement;
    }
  }
}
