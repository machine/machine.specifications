using System;
using System.Linq;

using JetBrains.Application;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.UnitTestFramework;
#if RESHARPER_61
using JetBrains.ReSharper.UnitTestFramework.Elements;
#endif
using JetBrains.Util;

using Machine.Specifications.ReSharperRunner.Factories;

namespace Machine.Specifications.ReSharperRunner.Explorers
{
  class AssemblyExplorer
  {
    readonly IMetadataAssembly _assembly;
    readonly BehaviorFactory _behaviorFactory;
    readonly BehaviorSpecificationFactory _behaviorSpecificationFactory;
    readonly UnitTestElementConsumer _consumer;
    readonly ContextFactory _contextFactory;
    readonly ContextSpecificationFactory _contextSpecificationFactory;

    public AssemblyExplorer(MSpecUnitTestProvider provider,
#if RESHARPER_61
                            IUnitTestElementManager manager,
                            PsiModuleManager psiModuleManager,
                            CacheManager cacheManager,
#endif
                            IMetadataAssembly assembly,
                            IProject project,
                            UnitTestElementConsumer consumer)
    {
      _assembly = assembly;
      _consumer = consumer;

      using (ReadLockCookie.Create())
      {
        var projectEnvoy = new ProjectModelElementEnvoy(project);

        var cache = new ContextCache();
#if RESHARPER_61
        _contextFactory = new ContextFactory(provider, manager, psiModuleManager, cacheManager, project, projectEnvoy, _assembly.Location.FullPath, cache);
        _contextSpecificationFactory = new ContextSpecificationFactory(provider, manager, psiModuleManager, cacheManager, project, projectEnvoy, cache);
        _behaviorFactory = new BehaviorFactory(provider, manager, psiModuleManager, cacheManager, project, projectEnvoy, cache);
        _behaviorSpecificationFactory = new BehaviorSpecificationFactory(provider, manager, psiModuleManager, cacheManager, project, projectEnvoy);
#else
#if RESHARPER_6
        _contextFactory = new ContextFactory(provider, project, projectEnvoy, _assembly.Location.FullPath, cache);
#else
        _contextFactory = new ContextFactory(provider, project, projectEnvoy, _assembly.Location, cache);
#endif
        _contextSpecificationFactory = new ContextSpecificationFactory(provider, project, projectEnvoy, cache);
        _behaviorFactory = new BehaviorFactory(provider, project, projectEnvoy, cache);
        _behaviorSpecificationFactory = new BehaviorSpecificationFactory(provider, project, projectEnvoy);
#endif
      }
    }

    public void Explore()
    {
      if (!_assembly.ReferencedAssembliesNames.Any(x => String.Equals(
                                                          x.Name,
                                                          typeof(It).Assembly.GetName().Name,
                                                          StringComparison.InvariantCultureIgnoreCase)))
      {
        return;
      }

      _assembly.GetTypes().Where(type => type.IsContext()).ForEach(type =>
      {
        var contextElement = _contextFactory.CreateContext(type);
        _consumer(contextElement);

        type
          .GetSpecifications()
          .ForEach(x => _consumer(_contextSpecificationFactory.CreateContextSpecification(contextElement, x)));

        type.GetBehaviors().ForEach(x =>
        {
          var behaviorElement = _behaviorFactory.CreateBehavior(contextElement, x);
          _consumer(behaviorElement);

          _behaviorSpecificationFactory
            .CreateBehaviorSpecificationsFromBehavior(behaviorElement, x)
            .ForEach(y => _consumer(y));
        });
      });
    }
  }
}