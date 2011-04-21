using System;
using System.Linq;

using JetBrains.Application;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
#if RESHARPER_6
using JetBrains.ReSharper.TaskRunnerFramework.UnitTesting;
#endif
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;

using Machine.Specifications.ReSharperRunner.Factories;
using Machine.Specifications.ReSharperRunner.Presentation;

namespace Machine.Specifications.ReSharperRunner.Explorers
{
  internal class AssemblyExplorer
  {
    readonly IMetadataAssembly _assembly;
    readonly BehaviorFactory _behaviorFactory;
    readonly BehaviorSpecificationFactory _behaviorSpecificationFactory;
    readonly UnitTestElementConsumer _consumer;
    readonly ContextFactory _contextFactory;
    readonly ContextSpecificationFactory _contextSpecificationFactory;

    public AssemblyExplorer(MSpecUnitTestProvider provider,
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
        _contextFactory = new ContextFactory(provider, projectEnvoy, _assembly.Location, cache);
        _contextSpecificationFactory = new ContextSpecificationFactory(provider, projectEnvoy, cache);
        _behaviorFactory = new BehaviorFactory(provider, projectEnvoy, cache);
        _behaviorSpecificationFactory = new BehaviorSpecificationFactory(provider, projectEnvoy);
      }
    }

    public void Explore()
    {
      if (!_assembly.ReferencedAssembliesNames.Any(x => String.Equals(x.AssemblyName.Name,
                                                                      typeof(It).Assembly.GetName().Name,
                                                                      StringComparison.InvariantCultureIgnoreCase)))
      {
        return;
      }

      _assembly.GetTypes()
        .Where(type => type.IsContext())
        .ForEach(type =>
          {
            var contextElement = _contextFactory.CreateContext(type);
            _consumer(contextElement);

            type.GetSpecifications().ForEach(x => _consumer(_contextSpecificationFactory.CreateContextSpecification(contextElement, x)));

            type.GetBehaviors().ForEach(x =>
              {
                BehaviorElement behaviorElement = _behaviorFactory.CreateBehavior(contextElement, x);
                _consumer(behaviorElement);

                _behaviorSpecificationFactory.CreateBehaviorSpecificationsFromBehavior(behaviorElement, x).ForEach(y => _consumer(y));
              });
          });
    }
  }
}
