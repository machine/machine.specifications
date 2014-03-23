using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Impl.Reflection2;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;

using Machine.Specifications.ReSharperRunner.Presentation;
using Machine.Specifications.ReSharperRunner.Shims;

using ICache = Machine.Specifications.ReSharperRunner.Shims.ICache;

namespace Machine.Specifications.ReSharperRunner.Factories
{
  [SolutionComponent]
  public class ContextSpecificationFactory
  {
    readonly ElementCache _cache;
    readonly ICache _cacheManager;
    readonly IUnitTestElementManager _manager;
    readonly MSpecUnitTestProvider _provider;
    readonly IPsi _psiModuleManager;
    readonly ReflectionTypeNameCache _reflectionTypeNameCache = new ReflectionTypeNameCache();

    public ContextSpecificationFactory(MSpecUnitTestProvider provider,
                                       IUnitTestElementManager manager,
                                       IPsi psiModuleManager,
                                       ICache cacheManager,
                                       ElementCache cache)
    {
      _manager = manager;
      _psiModuleManager = psiModuleManager;
      _cacheManager = cacheManager;
      _provider = provider;
      _cache = cache;
    }

    public ContextSpecificationElement CreateContextSpecification(IDeclaredElement field)
    {
      var clazz = ((ITypeMember) field).GetContainingType() as IClass;
      if (clazz == null)
      {
        return null;
      }

      var context = _cache.TryGetContext(clazz);
      if (context == null)
      {
        return null;
      }

      return GetOrCreateContextSpecification(context,
                                             clazz.GetClrName().GetPersistent(),
                                             field.ShortName,
                                             field.IsIgnored());
    }

    public ContextSpecificationElement CreateContextSpecification(ContextElement context, IMetadataField specification)
    {
      return GetOrCreateContextSpecification(context,
                                             _reflectionTypeNameCache.GetClrName(specification.DeclaringType),
                                             specification.Name,
                                             specification.IsIgnored());
    }

    public ContextSpecificationElement GetOrCreateContextSpecification(ContextElement context,
                                                                       IClrTypeName declaringTypeName,
                                                                       string fieldName,
                                                                       bool isIgnored)
    {
      var id = ContextSpecificationElement.CreateId(context, fieldName);
      var contextSpecification = _manager.GetElementById(context.GetProject(), id) as ContextSpecificationElement;
      if (contextSpecification != null)
      {
        contextSpecification.Parent = context;
        contextSpecification.State = UnitTestElementState.Valid;
        return contextSpecification;
      }

      return new ContextSpecificationElement(_provider,
                                             _psiModuleManager,
                                             _cacheManager,
                                             new ProjectModelElementEnvoy(context.GetProject()),
                                             context,
                                             declaringTypeName,
                                             fieldName, isIgnored);
    }
  }
}