using System.Collections.Generic;
using System.Linq;

using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Impl.Reflection2;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;

using Machine.Specifications.ReSharperRunner.AssemblySource;
using Machine.Specifications.ReSharperRunner.Presentation;
using Machine.Specifications.ReSharperRunner.Shims;
using Machine.Specifications.Sdk;

using ICache = Machine.Specifications.ReSharperRunner.Shims.ICache;

namespace Machine.Specifications.ReSharperRunner.Factories
{
  [SolutionComponent]
  public class ContextFactory
  {
    readonly ElementCache _cache;
    readonly ICache _cacheManager;
    readonly IUnitTestElementManager _manager;
    readonly MSpecUnitTestProvider _provider;
    readonly IPsi _psiModuleManager;
    readonly ReflectionTypeNameCache _reflectionTypeNameCache = new ReflectionTypeNameCache();

    public ContextFactory(MSpecUnitTestProvider provider,
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

    public IUnitTestElement CreateContext(string assemblyPath, IDeclaration declaration)
    {
      var type = (ITypeElement) declaration.DeclaredElement;
      var context = GetOrCreateContext(assemblyPath,
                                       declaration.GetProject(),
                                       type.GetClrName().GetPersistent(),
                                       type.GetSubjectString(),
                                       type.GetTags(), type.IsIgnored());

      foreach (var child in context.Children)
      {
        child.State = UnitTestElementState.Pending;
      }

      _cache.AddContext(type, context);
      return context;
    }

      //Todo either IMetadataTypeInfo or ITypeInfo
    public ContextElement CreateContext(IProject project, string assemblyPath, IMetadataTypeInfo type, ITypeInfo infoType)
    {
        return this.GetOrCreateContext(assemblyPath, project, _reflectionTypeNameCache.GetClrName(type), infoType.GetStringOfSubjectAttribute(),
                                type.GetTags(), type.IsIgnored()); //type.GetTags and type.IsIgnored -> are IMetadataTypeInfos
    }


    public ContextElement GetOrCreateContext(string assemblyPath, IProject project, IClrTypeName typeName, string subject, 
        ICollection<string> tags, bool isIgnored)
    {
        var id = ContextElement.CreateId(subject, typeName.FullName, tags);
        //if tests already exists in resharper, do not create new test class
        var contextElement = _manager.GetElementById(project, id) as ContextElement; 
        if (contextElement != null)
        {
            contextElement.State = UnitTestElementState.Valid;
            return contextElement;
        }
        return new ContextElement(_provider, _psiModuleManager, _cacheManager, new ProjectModelElementEnvoy(project), typeName,
                                    assemblyPath, subject, tags, isIgnored);
    }

    public void UpdateChildState(ITypeElement type)
    {
      var context = _cache.TryGetContext(type);
      if (context == null)
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