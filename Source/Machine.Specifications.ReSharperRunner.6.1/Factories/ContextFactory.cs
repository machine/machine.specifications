using System.Collections.Generic;
using System.Linq;

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
  class ContextFactory
  {
    readonly string _assemblyPath;

    readonly ElementCache _cache;
    readonly CacheManager _cacheManager;
    readonly IUnitTestElementManager _manager;
    readonly IProject _project;
    readonly ProjectModelElementEnvoy _projectEnvoy;
    readonly MSpecUnitTestProvider _provider;
    readonly PsiModuleManager _psiModuleManager;
    readonly ReflectionTypeNameCache _reflectionTypeNameCache = new ReflectionTypeNameCache();

    public ContextFactory(MSpecUnitTestProvider provider,
                          IUnitTestElementManager manager,
                          PsiModuleManager psiModuleManager,
                          CacheManager cacheManager,
                          IProject project,
                          ProjectModelElementEnvoy projectEnvoy,
                          string assemblyPath,
                          ElementCache cache)
    {
      _manager = manager;
      _psiModuleManager = psiModuleManager;
      _cacheManager = cacheManager;
      _provider = provider;
      _cache = cache;
      _project = project;
      _projectEnvoy = projectEnvoy;
      _assemblyPath = assemblyPath;
    }

    public ContextElement CreateContext(ITypeElement type)
    {
      var context = GetOrCreateContext(_provider,
                                       _manager,
                                       _psiModuleManager,
                                       _cacheManager,
                                       _project,
                                       _projectEnvoy,
                                       type.GetClrName().GetPersistent(),
                                       _assemblyPath,
                                       type.GetSubjectString(),
                                       type.GetTags(),
                                       type.IsIgnored());

      foreach (var child in context.Children)
      {
        child.State = UnitTestElementState.Pending;
      }

      _cache.Contexts.Add(type, context);
      return context;
    }

    public ContextElement CreateContext(IMetadataTypeInfo type)
    {
      return GetOrCreateContext(_provider,
                                _manager,
                                _psiModuleManager,
                                _cacheManager,
                                _project,
                                _projectEnvoy,
                                _reflectionTypeNameCache.GetClrName(type),
                                _assemblyPath,
                                type.GetSubjectString(),
                                type.GetTags(),
                                type.IsIgnored());
    }

    public static ContextElement GetOrCreateContext(MSpecUnitTestProvider provider,
                                                    IUnitTestElementManager manager,
                                                    PsiModuleManager psiModuleManager,
                                                    CacheManager cacheManager,
                                                    IProject project,
                                                    ProjectModelElementEnvoy projectEnvoy,
                                                    IClrTypeName typeName,
                                                    string assemblyLocation,
                                                    string subject,
                                                    ICollection<string> tags,
                                                    bool isIgnored)
    {
      var id = ContextElement.CreateId(subject, typeName.FullName, tags);
      var contextElement = manager.GetElementById(project, id) as ContextElement;
      if (contextElement != null)
      {
        contextElement.State = UnitTestElementState.Valid;
        return contextElement;
      }

      return new ContextElement(provider,
                                psiModuleManager,
                                cacheManager,
                                projectEnvoy,
                                typeName,
                                assemblyLocation,
                                subject,
                                tags,
                                isIgnored);
    }

    public void UpdateChildState(ITypeElement type)
    {
      ContextElement context;
      if (!_cache.Contexts.TryGetValue(type, out context))
      {
        return;
      }

      foreach (var element in context
        .Children.Where(x => x.State == UnitTestElementState.Pending)
        .Flatten(x => x.Children))
      {
        element.State = UnitTestElementState.Invalid;
      }
    }
  }
}