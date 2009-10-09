using System;
using System.Linq;

using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
#if RESHARPER_5
using JetBrains.ReSharper.UnitTestFramework;
#else
using JetBrains.ReSharper.UnitTestExplorer;
#endif
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
    readonly IProject _project;
    readonly MSpecUnitTestProvider _provider;

    public AssemblyExplorer(MSpecUnitTestProvider provider,
                            IMetadataAssembly assembly,
                            IProject project,
                            UnitTestElementConsumer consumer)
    {
      _provider = provider;
      _assembly = assembly;
      _project = project;
      _consumer = consumer;

      var cache = new ContextCache();
      _contextFactory = new ContextFactory(_provider, _project, _assembly.Location, cache);
      _contextSpecificationFactory = new ContextSpecificationFactory(_provider, _project, cache);
      _behaviorFactory = new BehaviorFactory(_provider, _project, cache);
      _behaviorSpecificationFactory = new BehaviorSpecificationFactory(_provider, _project);
    }

    public void Explore()
    {
      if (!_assembly.ReferencedAssembliesNames.Exists(x => String.Equals(x.AssemblyName.Name,
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
