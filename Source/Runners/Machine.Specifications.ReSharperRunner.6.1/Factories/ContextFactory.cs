using System.Collections.Generic;
using System.Linq;

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
  public class ContextFactory
  {
    readonly string _assemblyPath;

    readonly ElementCache _cache;
    readonly ICache _cacheManager;
    readonly IUnitTestElementManager _manager;
    readonly IProject _project;
    readonly ProjectModelElementEnvoy _projectEnvoy;
    readonly MSpecUnitTestProvider _provider;
    readonly IPsi _psiModuleManager;
    readonly ReflectionTypeNameCache _reflectionTypeNameCache = new ReflectionTypeNameCache();

    public ContextFactory(MSpecUnitTestProvider provider,
                          IUnitTestElementManager manager,
                          IPsi psiModuleManager,
                          ICache cacheManager,
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
      var context = GetOrCreateContext(type.GetClrName().GetPersistent(),
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
      return GetOrCreateContext(_reflectionTypeNameCache.GetClrName(type),
                                _assemblyPath,
                                type.GetSubjectString(),
                                type.GetTags(),
                                type.IsIgnored());
    }

    public ContextElement GetOrCreateContext(IClrTypeName typeName,
                                             string assemblyLocation,
                                             string subject,
                                             ICollection<string> tags,
                                             bool isIgnored)
    {
      var id = ContextElement.CreateId(subject, typeName.FullName, tags);
      var contextElement = _manager.GetElementById(_project, id) as ContextElement;
      if (contextElement != null)
      {
        contextElement.State = UnitTestElementState.Valid;
        return contextElement;
      }

      return new ContextElement(_provider,
                                _psiModuleManager,
                                _cacheManager,
                                _projectEnvoy,
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