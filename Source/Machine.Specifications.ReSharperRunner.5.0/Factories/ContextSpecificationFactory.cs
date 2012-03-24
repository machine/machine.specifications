using System.Collections.Generic;

using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
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
    readonly ContextCache _cache;
    readonly IProject _project;
#if RESHARPER_61
    readonly IUnitTestElementManager _manager;
    readonly PsiModuleManager _psiModuleManager;
    readonly CacheManager _cacheManager;
#endif

#if RESHARPER_61
    public ContextSpecificationFactory(MSpecUnitTestProvider provider, IUnitTestElementManager manager, PsiModuleManager psiModuleManager, CacheManager cacheManager, IProject project, ProjectModelElementEnvoy projectEnvoy, ContextCache cache)
    {
      _manager = manager;
      _psiModuleManager = psiModuleManager;
      _cacheManager = cacheManager;
#else
    public ContextSpecificationFactory(MSpecUnitTestProvider provider, IProject project, ProjectModelElementEnvoy projectEnvoy, ContextCache cache)
    {
#endif
      _provider = provider;
      _cache = cache;
      _project = project;
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

      return GetOrCreateContextSpecification(_provider,
#if RESHARPER_61
                                             _manager, _psiModuleManager, _cacheManager, 
#endif
                                             _project,
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
      return GetOrCreateContextSpecification(_provider,
#if RESHARPER_61
                                             _manager, _psiModuleManager, _cacheManager, 
#endif
                                             _project,
                                             context,
                                             _projectEnvoy,
                                             specification.DeclaringType.FullyQualifiedName,
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
                                                                              string declaringTypeName,
                                                                              string fieldName,
                                                                              ICollection<string> tags,
                                                                              bool isIgnored)
    {
#if RESHARPER_6
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
#endif

      return new ContextSpecificationElement(provider,
#if RESHARPER_6
#if RESHARPER_61
                                 psiModuleManager, cacheManager, 
#else
                                 provider.PsiModuleManager, provider.CacheManager,
#endif
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
