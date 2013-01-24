using System;
using System.Collections.Generic;
using System.Linq;

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
  internal class ContextFactory
  {
    readonly string _assemblyPath;

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
    public ContextFactory(MSpecUnitTestProvider provider, IUnitTestElementManager manager, PsiModuleManager psiModuleManager, CacheManager cacheManager, IProject project, ProjectModelElementEnvoy projectEnvoy, string assemblyPath, ElementCache cache)
    {
      _manager = manager;
      _psiModuleManager = psiModuleManager;
      _cacheManager = cacheManager;
#else
    public ContextFactory(MSpecUnitTestProvider provider, IProject project, ProjectModelElementEnvoy projectEnvoy, string assemblyPath, ElementCache cache)
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
      var context = GetOrCreateContext(_provider,
#if RESHARPER_61
                                              _manager,
                                              _psiModuleManager,
                                              _cacheManager,
#endif
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
#if RESHARPER_61
                                       _manager,
                                       _psiModuleManager,
                                       _cacheManager,
#endif
                                       _project,
                                       _projectEnvoy,
#if RESHARPER_61
                                       _reflectionTypeNameCache.GetClrName(type),
#else
                                       new ClrTypeName(type.FullyQualifiedName), // may work incorrect in ReSharper 6.0
#endif
                                       _assemblyPath,
                                       type.GetSubjectString(),
                                       type.GetTags(),
                                       type.IsIgnored());
    }

    public static ContextElement GetOrCreateContext(MSpecUnitTestProvider provider,
#if RESHARPER_61
                                                    IUnitTestElementManager manager,
                                                    PsiModuleManager psiModuleManager,
                                                    CacheManager cacheManager,
#endif
                                                    IProject project,
                                                    ProjectModelElementEnvoy projectEnvoy,
                                                    IClrTypeName typeName,
                                                    string assemblyLocation,
                                                    string subject,
                                                    ICollection<string> tags,
                                                    bool isIgnored)
    {
      var id = ContextElement.CreateId(subject, typeName.FullName, tags);
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

      return new ContextElement(provider,
#if RESHARPER_61
                                psiModuleManager,
                                cacheManager,
#else
                                provider.PsiModuleManager,
                                provider.CacheManager,
#endif
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
        .Traverse(x => x.Children))
      {
        element.State = UnitTestElementState.Invalid;
      }
    }
  }

  static class EnumExt
  {
    internal static IEnumerable<T> Traverse<T>(this IEnumerable<T> source,
                                              Func<T, IEnumerable<T>> childSelector)
    {
      foreach (var s in source)
      {
        yield return s;

        var childs = childSelector(s);
        foreach (var c in childs.Traverse(childSelector))
        {
          yield return c;
        }
      }
    }
  }

}