using System.Collections.Generic;

using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Impl.Reflection2;
using JetBrains.ReSharper.UnitTestFramework;
#if RESHARPER_61
using JetBrains.ReSharper.UnitTestFramework.Elements;
#endif

using Machine.Specifications.ReSharperRunner.Presentation;

namespace Machine.Specifications.ReSharperRunner.Factories
{
  internal class ContextSpecificationFactory
  {
    readonly ProjectModelElementEnvoy _projectEnvoy;
    readonly MSpecUnitTestProvider _provider;
    readonly ElementCache _cache;
    readonly IProject _project;
#if RESHARPER_61
    readonly ReflectionTypeNameCache _reflectionTypeNameCache = new ReflectionTypeNameCache();
    readonly IUnitTestElementManager _manager;
    readonly PsiModuleManager _psiModuleManager;
    readonly CacheManager _cacheManager;
#endif

#if RESHARPER_61
    public ContextSpecificationFactory(MSpecUnitTestProvider provider, IUnitTestElementManager manager, PsiModuleManager psiModuleManager, CacheManager cacheManager, IProject project, ProjectModelElementEnvoy projectEnvoy, ElementCache cache)
    {
      _manager = manager;
      _psiModuleManager = psiModuleManager;
      _cacheManager = cacheManager;
#else
    public ContextSpecificationFactory(MSpecUnitTestProvider provider, IProject project, ProjectModelElementEnvoy projectEnvoy, ElementCache cache)
    {
#endif
      _provider = provider;
      _cache = cache;
      _project = project;
      _projectEnvoy = projectEnvoy;
    }

    public ContextSpecificationElement CreateContextSpecification(IDeclaredElement field)
    {
      var clazz = ((ITypeMember)field).GetContainingType() as IClass;
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
#if RESHARPER_61
                                             _manager, _psiModuleManager, _cacheManager,
#endif
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
#if RESHARPER_61
                                             _manager, _psiModuleManager, _cacheManager,
#endif
                                             _project,
                                             context,
                                             _projectEnvoy,
#if RESHARPER_61
                                             _reflectionTypeNameCache.GetClrName(specification.DeclaringType),
#else
                                             new ClrTypeName(specification.DeclaringType.FullyQualifiedName), // may work incorrect in ReSharper 6.0
#endif
                                             specification.Name,
                                             specification.DeclaringType.GetTags(),
                                             specification.IsIgnored());
    }

    public static ContextSpecificationElement GetOrCreateContextSpecification(MSpecUnitTestProvider provider,
#if RESHARPER_61
                                                                              IUnitTestElementManager manager,
                                                                              PsiModuleManager psiModuleManager,
                                                                              CacheManager cacheManager,
#endif
                                                                              IProject project,
                                                                              ContextElement context,
                                                                              ProjectModelElementEnvoy projectEnvoy,
                                                                              IClrTypeName declaringTypeName,
                                                                              string fieldName,
                                                                              ICollection<string> tags,
                                                                              bool isIgnored)
    {
      var id = ContextSpecificationElement.CreateId(context, fieldName);
#if RESHARPER_61
      var contextSpecification = manager.GetElementById(project, id) as ContextSpecificationElement;
#else
      var contextSpecification = provider.UnitTestManager.GetElementById(project, id) as ContextSpecificationElement;
#endif
      if (contextSpecification != null)
      {
        contextSpecification.Parent = context;
        contextSpecification.State = UnitTestElementState.Valid;
        return contextSpecification;
      }

      return new ContextSpecificationElement(provider,
#if RESHARPER_61
                                 psiModuleManager, cacheManager,
#else
                                 provider.PsiModuleManager, provider.CacheManager,
#endif
                                             context,
                                             projectEnvoy,
                                             declaringTypeName,
                                             fieldName,
                                             tags,
                                             isIgnored);
    }
  }
}
