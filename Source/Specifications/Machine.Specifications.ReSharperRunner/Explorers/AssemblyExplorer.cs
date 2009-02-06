using System.Linq;

using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestExplorer;

using Machine.Specifications.ReSharperRunner.Factories;
using Machine.Specifications.Utility;

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
      _assembly.GetTypes()
        .Where(type => type.IsContext())
        .Each(type =>
          {
            var contextElement = _elementFactory.CreateContextElement(type);
            _consumer(contextElement);

            type.GetSpecifications().Each(specification =>
                                                            _consumer(_elementFactory.CreateSpecificationElement(
                                                                        contextElement, specification)));
            type.GetBehaviors().Each(behavior =>
                                                       _consumer(_elementFactory.CreateBehaviorElement(contextElement,
                                                                                                       behavior)));
          });
    }
  }
}