using System;
using System.Linq;

using JetBrains.Application;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;
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
                            IUnitTestElementManager manager,
                            PsiModuleManager psiModuleManager,
                            CacheManager cacheManager,
                            IMetadataAssembly assembly,
                            IProject project,
                            UnitTestElementConsumer consumer)
    {
      _assembly = assembly;
      _consumer = consumer;

      using (ReadLockCookie.Create())
      {
        var projectEnvoy = new ProjectModelElementEnvoy(project);

        var cache = new ElementCache();
        _contextFactory = new ContextFactory(provider, manager, psiModuleManager, cacheManager, project, projectEnvoy, _assembly.Location.FullPath, cache);
        _contextSpecificationFactory = new ContextSpecificationFactory(provider, manager, psiModuleManager, cacheManager, project, projectEnvoy, cache);
        _behaviorFactory = new BehaviorFactory(provider, manager, psiModuleManager, cacheManager, project, projectEnvoy, cache);
        _behaviorSpecificationFactory = new BehaviorSpecificationFactory(provider, manager, psiModuleManager, cacheManager, project, projectEnvoy);
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