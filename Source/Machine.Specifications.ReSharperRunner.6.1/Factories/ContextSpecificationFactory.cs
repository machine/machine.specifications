using System.Collections.Generic;

using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Impl.Reflection2;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;

using Machine.Specifications.ReSharperRunner.Presentation;

namespace Machine.Specifications.ReSharperRunner.Factories
{
  class ContextSpecificationFactory
  {
    readonly ElementCache _cache;
    readonly CacheManager _cacheManager;
    readonly IUnitTestElementManager _manager;
    readonly IProject _project;
    readonly ProjectModelElementEnvoy _projectEnvoy;
    readonly MSpecUnitTestProvider _provider;
    readonly PsiModuleManager _psiModuleManager;
    readonly ReflectionTypeNameCache _reflectionTypeNameCache = new ReflectionTypeNameCache();

    public ContextSpecificationFactory(MSpecUnitTestProvider provider,
                                       IUnitTestElementManager manager,
                                       PsiModuleManager psiModuleManager,
                                       CacheManager cacheManager,
                                       IProject project,
                                       ProjectModelElementEnvoy projectEnvoy,
                                       ElementCache cache)
    {
      _manager = manager;
      _psiModuleManager = psiModuleManager;
      _cacheManager = cacheManager;
      _provider = provider;
      _cache = cache;
      _project = project;
      _projectEnvoy = projectEnvoy;
    }

    public ContextSpecificationElement CreateContextSpecification(IDeclaredElement field)
    {
      var clazz = ((ITypeMember) field).GetContainingType() as IClass;
      if (clazz == null)
      {
        return null;
      }

      ContextElement context;
      _cache.Contexts.TryGetValue(clazz, out context);
      if (context == null)
      {
        return null;
      }

      return GetOrCreateContextSpecification(_provider,
                                             _manager,
                                             _psiModuleManager,
                                             _cacheManager,
                                             _project,
                                             context,
                                             _projectEnvoy,
                                             clazz.GetClrName().GetPersistent(),
                                             field.ShortName,
                                             clazz.GetTags(),
                                             field.IsIgnored());
    }

    public ContextSpecificationElement CreateContextSpecification(ContextElement context, IMetadataField specification)
    {
      return GetOrCreateContextSpecification(_provider,
                                             _manager,
                                             _psiModuleManager,
                                             _cacheManager,
                                             _project,
                                             context,
                                             _projectEnvoy,
                                             _reflectionTypeNameCache.GetClrName(specification.DeclaringType),
                                             specification.Name,
                                             specification.DeclaringType.GetTags(),
                                             specification.IsIgnored());
    }

    public static ContextSpecificationElement GetOrCreateContextSpecification(MSpecUnitTestProvider provider,
                                                                              IUnitTestElementManager manager,
                                                                              PsiModuleManager psiModuleManager,
                                                                              CacheManager cacheManager,
                                                                              IProject project,
                                                                              ContextElement context,
                                                                              ProjectModelElementEnvoy projectEnvoy,
                                                                              IClrTypeName declaringTypeName,
                                                                              string fieldName,
                                                                              ICollection<string> tags,
                                                                              bool isIgnored)
    {
      var id = ContextSpecificationElement.CreateId(context, fieldName);
      var contextSpecification = manager.GetElementById(project, id) as ContextSpecificationElement;
      if (contextSpecification != null)
      {
        contextSpecification.Parent = context;
        contextSpecification.State = UnitTestElementState.Valid;
        return contextSpecification;
      }

      return new ContextSpecificationElement(provider,
                                             psiModuleManager,
                                             cacheManager,
                                             context,
                                             projectEnvoy,
                                             declaringTypeName,
                                             fieldName,
                                             tags,
                                             isIgnored);
    }
  }
}