using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;

using Machine.Specifications.ReSharperRunner.Presentation;

namespace Machine.Specifications.ReSharperRunner.Factories
{
  internal class ContextFactory
  {
    readonly string _assemblyPath;

    readonly ProjectModelElementEnvoy _projectEnvoy;
    readonly MSpecUnitTestProvider _provider;
    readonly ContextCache _cache;

    public ContextFactory(MSpecUnitTestProvider provider, ProjectModelElementEnvoy projectEnvoy, string assemblyPath, ContextCache cache)
    {
      _provider = provider;
      _cache = cache;
      _projectEnvoy = projectEnvoy;
      _assemblyPath = assemblyPath;
    }

    public ContextElement CreateContext(ITypeElement type)
    {
      if (_cache.Classes.ContainsKey(type))
      {
        return _cache.Classes[type];
      }

      ContextElement context = new ContextElement(_provider,
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
      _cache.Classes.Add(type, context);
      return context;
    }

    public ContextElement CreateContext(IMetadataTypeInfo type)
    {
      return new ContextElement(_provider,
                                _projectEnvoy,
                                type.FullyQualifiedName,
                                _assemblyPath,
                                type.GetSubjectString(),
                                type.GetTags(),
                                type.IsIgnored());
    }
  }
}
