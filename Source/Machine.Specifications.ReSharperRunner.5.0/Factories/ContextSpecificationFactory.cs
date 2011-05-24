using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;

using Machine.Specifications.ReSharperRunner.Presentation;

namespace Machine.Specifications.ReSharperRunner.Factories
{
  internal class ContextSpecificationFactory
  {
    readonly ProjectModelElementEnvoy _projectEnvoy;
    readonly MSpecUnitTestProvider _provider;
    readonly ContextCache _cache;

    public ContextSpecificationFactory(MSpecUnitTestProvider provider, ProjectModelElementEnvoy projectEnvoy, ContextCache cache)
    {
      _provider = provider;
      _cache = cache;
      _projectEnvoy = projectEnvoy;
    }

    public ContextSpecificationElement CreateContextSpecification(IDeclaredElement field)
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

      return new ContextSpecificationElement(_provider,
                                             context,
                                             _projectEnvoy,
#if RESHARPER_6
                                             clazz.GetClrName().FullName,
#else
                                             clazz.CLRName,
#endif
                                             field.ShortName,
                                             clazz.GetTags(),
                                             field.IsIgnored());
    }

    public ContextSpecificationElement CreateContextSpecification(ContextElement context, IMetadataField specification)
    {
      return new ContextSpecificationElement(_provider,
                                             context,
                                             _projectEnvoy,
                                             specification.DeclaringType.FullyQualifiedName,
                                             specification.Name,
                                             specification.DeclaringType.GetTags(),
                                             specification.IsIgnored());
    }
  }
}
