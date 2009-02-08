using System;
using System.Linq;

using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestExplorer;
using JetBrains.Util;

using Machine.Specifications.ReSharperRunner.Factories;

namespace Machine.Specifications.ReSharperRunner.Explorers
{
  internal class AssemblyExplorer
  {
    readonly IMetadataAssembly _assembly;
    readonly UnitTestElementConsumer _consumer;
    readonly ElementFactory _elementFactory;
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

      _elementFactory = new ElementFactory(_provider, _project, _assembly.Location);
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
            var contextElement = _elementFactory.CreateContextElement(type);
            _consumer(contextElement);

            type.GetSpecifications().ForEach(x => _consumer(_elementFactory.CreateSpecificationElement(contextElement, x)));
            type.GetBehaviors().ForEach(x => _consumer(_elementFactory.CreateBehaviorElement(contextElement, x)));
          });
    }
  }
}