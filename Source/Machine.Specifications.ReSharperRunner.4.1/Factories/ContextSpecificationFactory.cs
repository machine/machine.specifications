using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
#if RESHARPER_5
using JetBrains.ReSharper.UnitTestFramework;
#else
using JetBrains.ReSharper.UnitTestExplorer;
#endif

using Machine.Specifications.ReSharperRunner.Presentation;

namespace Machine.Specifications.ReSharperRunner.Factories
{
  internal class ContextSpecificationFactory
  {
    readonly IProjectModelElement _project;
    readonly IUnitTestProvider _provider;
    readonly ContextCache _cache;

    public ContextSpecificationFactory(IUnitTestProvider provider, IProjectModelElement project, ContextCache cache)
    {
      _provider = provider;
      _cache = cache;
      _project = project;
    }

    public ContextSpecificationElement CreateContextSpecification(IDeclaredElement field)
    {
      IClass clazz = field.GetContainingType() as IClass;
      if (clazz == null)
      {
        return null;
      }

      ContextElement context = _cache.Classes[clazz];
      if (context == null)
      {
        return null;
      }

      return new ContextSpecificationElement(_provider,
                                             context,
                                             _project,
                                             clazz.CLRName,
                                             field.ShortName,
                                             field.IsIgnored());
    }

    public ContextSpecificationElement CreateContextSpecification(ContextElement context, IMetadataField specification)
    {
      return new ContextSpecificationElement(_provider,
                                             context,
                                             _project,
                                             specification.DeclaringType.FullyQualifiedName,
                                             specification.Name,
                                             specification.IsIgnored());
    }
  }
}
