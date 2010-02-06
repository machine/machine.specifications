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

      return new BehaviorElement(_provider,
                                 context,
                                 _projectEnvoy,
                                 clazz.CLRName,
                                 field.ShortName,
                                 field.IsIgnored());
    }

    public BehaviorElement CreateBehavior(ContextElement context, IMetadataField behavior)
    {
      IMetadataTypeInfo typeContainingBehaviorSpecifications = behavior.GetFirstGenericArgument();

      return new BehaviorElement(_provider,
                                 context,
                                 _projectEnvoy,
                                 behavior.DeclaringType.FullyQualifiedName,
                                 behavior.Name,
                                 behavior.IsIgnored() || typeContainingBehaviorSpecifications.IsIgnored());
    }
  }
}
