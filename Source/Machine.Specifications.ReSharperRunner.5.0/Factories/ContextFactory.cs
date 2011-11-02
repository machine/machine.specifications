using System.Collections.Generic;
using System.Linq;

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
  internal class ContextFactory
  {
    readonly string _assemblyPath;

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
    public ContextFactory(MSpecUnitTestProvider provider, IUnitTestElementManager manager, PsiModuleManager psiModuleManager, CacheManager cacheManager, IProject project, ProjectModelElementEnvoy projectEnvoy, string assemblyPath, ContextCache cache)
    {
      _manager = manager;
      _psiModuleManager = psiModuleManager;
      _cacheManager = cacheManager;
#else
    public ContextFactory(MSpecUnitTestProvider provider, IProject project, ProjectModelElementEnvoy projectEnvoy, string assemblyPath, ContextCache cache)
    {
#endif
      _provider = provider;
      _cache = cache;
      _project = project;
      _projectEnvoy = projectEnvoy;
      _assemblyPath = assemblyPath;
    }

    public ContextElement CreateContext(ITypeElement type)
    {
      if (_cache.Classes.ContainsKey(type))
      {
        return _cache.Classes[type];
      }

      var context = GetOrCreateContextElement(_provider,
#if RESHARPER_61
                                              _manager,
                                              _psiModuleManager,
                                              _cacheManager,
#endif
                                              _project,
                                              _projectEnvoy,
#if RESHARPER_6
                                              type.GetClrName().FullName,
#else
                                              type.CLRName,
#endif
                                              _assemblyPath,
                                              type.GetSubjectString(),
                                              type.GetTags(),
                                              type.IsIgnored());

#if RESHARPER_6
      foreach (var child in context.Children)
      {
        child.State = UnitTestElementState.Pending;
      }
#endif

      _cache.Classes.Add(type, context);
      return context;
    }

    public ContextElement CreateContext(IMetadataTypeInfo type)
    {
      return GetOrCreateContextElement(_provider,
#if RESHARPER_61
                                       _manager,
                                       _psiModuleManager,
                                       _cacheManager,
#endif
                                       _project,
                                       _projectEnvoy,
                                       type.FullyQualifiedName,
                                       _assemblyPath,
                                       type.GetSubjectString(),
                                       type.GetTags(),
                                       type.IsIgnored());
    }

    public static ContextElement GetOrCreateContextElement(MSpecUnitTestProvider provider,
#if RESHARPER_61
                                                           IUnitTestElementManager manager,
                                                           PsiModuleManager psiModuleManager,
                                                           CacheManager cacheManager,
#endif
                                                           IProject project,
                                                           ProjectModelElementEnvoy projectEnvoy,
                                                           string typeName,
                                                           string assemblyLocation,
                                                           string subject,
                                                           ICollection<string> tags,
                                                           bool isIgnored)
    {
#if RESHARPER_6
      var id = ContextElement.CreateId(subject, typeName);
#if RESHARPER_61
      var contextElement = manager.GetElementById(project, id) as ContextElement;
#else
      var contextElement = provider.UnitTestManager.GetElementById(project, id) as ContextElement;
#endif
      if (contextElement != null)
      {
        contextElement.State = UnitTestElementState.Valid;
        return contextElement;
      }
#endif

      return new ContextElement(provider,
#if RESHARPER_6
#if RESHARPER_61
                                psiModuleManager,
                                cacheManager,
#else
                                provider.PsiModuleManager,
                                provider.CacheManager,
#endif
#endif                
                                projectEnvoy,
                                typeName,
                                assemblyLocation,
                                subject,
                                tags,
                                isIgnored);
    }

#if RESHARPER_6
    public void UpdateChildState(ITypeElement type)
    {
      ContextElement element;
      if (!_cache.Classes.TryGetValue(type, out element))
      {
        return;
      }

      foreach (var unitTestElement in element.Children.Where(x => x.State == UnitTestElementState.Pending))
      {
        unitTestElement.State = UnitTestElementState.Invalid;
      }
    }
#endif
  }
}