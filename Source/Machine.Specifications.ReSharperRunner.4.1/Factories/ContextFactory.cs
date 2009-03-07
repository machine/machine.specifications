using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestExplorer;

using Machine.Specifications.ReSharperRunner.Presentation;

namespace Machine.Specifications.ReSharperRunner.Factories
{
  internal class ContextFactory
  {
    readonly string _assemblyPath;

    readonly IProjectModelElement _project;
    readonly IUnitTestProvider _provider;

    public ContextFactory(IUnitTestProvider provider, IProjectModelElement project, string assemblyPath)
    {
      _provider = provider;
      _project = project;
      _assemblyPath = assemblyPath;
    }

    public ContextElement CreateContext(ITypeElement type)
    {
      if (ContextCache.Classes.ContainsKey(type))
      {
        return ContextCache.Classes[type];
      }

      ContextElement context = new ContextElement(_provider,
                                                  _project,
                                                  type.CLRName,
                                                  _assemblyPath,
                                                  type.GetTags(),
                                                  type.IsIgnored());
      ContextCache.Classes.Add(type, context);
      return context;
    }

    public ContextElement CreateContext(IMetadataTypeInfo type)
    {
      if (type.IsNested || !type.IsPublic)
      {
        return null;
      }

      return new ContextElement(_provider,
                                _project,
                                type.FullyQualifiedName,
                                _assemblyPath,
                                type.GetTags(),
                                type.IsIgnored());
    }
  }
}