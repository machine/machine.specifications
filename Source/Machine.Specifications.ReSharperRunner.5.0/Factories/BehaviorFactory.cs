using System.Collections.Generic;
using System.Text.RegularExpressions;

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
    readonly MSpecUnitTestProvider _provider;
    readonly ContextCache _cache;
    static readonly IDictionary<string, string> TypeNameCache = new Dictionary<string, string>();
    readonly IProject _project;

    public BehaviorFactory(MSpecUnitTestProvider provider, IProject project, ProjectModelElementEnvoy projectEnvoy, ContextCache cache)
    {
      _provider = provider;
      _cache = cache;
      _project = project;
      _projectEnvoy = projectEnvoy;
    }

    public BehaviorElement CreateBehavior(IDeclaredElement field)
    {
#if RESHARPER_6
      IClass clazz = ((ITypeMember)field).GetContainingType() as IClass;
#else
      IClass clazz = field.GetContainingType() as IClass;
#endif
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

      string fullyQualifiedTypeName = null;
      if (field is ITypeOwner)
      {
          // Work around the difference in how the MetaData API and Psi API return different type strings for generics.
          TypeNameCache.TryGetValue(GetFirstGenericNormalizedTypeName(field), out fullyQualifiedTypeName);
      }

      return GetOrCreateBehavior(_provider,
                                 _project,
                                 _projectEnvoy,
                                 context,
#if RESHARPER_6
                                 clazz.GetClrName().FullName,
#else
                                 clazz.CLRName,
#endif
                                 field.ShortName,
                                 field.IsIgnored(),
                                 fullyQualifiedTypeName);
    }

    public static BehaviorElement GetOrCreateBehavior(MSpecUnitTestProvider provider,
                                                      IProject project,
                                                      ProjectModelElementEnvoy projectEnvoy,
                                                      ContextElement context,
                                                      string declaringTypeName,
                                                      string fieldName,
                                                      bool isIgnored,
                                                      string fullyQualifiedTypeName)
    {
#if RESHARPER_6
      var id = BehaviorElement.CreateId(context, fieldName);
      var behavior = provider.UnitTestManager.GetElementById(project, id) as BehaviorElement;
      if (behavior != null)
      {
        behavior.Parent = context;
        behavior.State = UnitTestElementState.Valid;
        return behavior;
      }
#endif

      return new BehaviorElement(provider,
                                 context,
                                 projectEnvoy,
                                 declaringTypeName,
                                 fieldName,
                                 isIgnored,
                                 fullyQualifiedTypeName);
    }

    public BehaviorElement CreateBehavior(ContextElement context, IMetadataField behavior)
    {
      var typeContainingBehaviorSpecifications = behavior.GetFirstGenericArgument();

      var fullyQualifiedTypeName = behavior.FirstGenericArgumentClass().FullyQualifiedName();
      var typeName = GetNormalizedTypeName(fullyQualifiedTypeName);

      var behaviorElement = GetOrCreateBehavior(_provider,
                                                _project,
                                                _projectEnvoy,
                                                context,
                                                behavior.DeclaringType.FullyQualifiedName,
                                                behavior.Name,
                                                behavior.IsIgnored() || typeContainingBehaviorSpecifications.IsIgnored(),
                                                fullyQualifiedTypeName);

      if (!TypeNameCache.ContainsKey(typeName))
      {
        TypeNameCache.Add(typeName, fullyQualifiedTypeName);
      }

      return behaviorElement;
    }

    static string GetFirstGenericNormalizedTypeName(IDeclaredElement field)
    {
      var typeName = ((ITypeOwner) field).Type.ToString();
      typeName = typeName.Substring(typeName.IndexOf("-> ") + 3);
      typeName = typeName.Remove(typeName.Length - 1);
      typeName = Regex.Replace(typeName, @"\[.*->\s", "[");
      return typeName;
    }

    static string GetNormalizedTypeName(string fullyQualifiedTypeName)
    {
      var typeName = Regex.Replace(fullyQualifiedTypeName, @"\,.+]", "]");
      typeName = Regex.Replace(typeName, @"\[\[", "[");
      return typeName;
    }
  }
}
