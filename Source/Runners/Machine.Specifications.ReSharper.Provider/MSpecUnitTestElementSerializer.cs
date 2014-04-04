using System.Xml;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;

using Machine.Specifications.ReSharperRunner.Factories;
using Machine.Specifications.ReSharperRunner.Presentation;

namespace Machine.Specifications.ReSharperRunner
{
  [SolutionComponent]
  public class MSpecUnitTestElementSerializer : IUnitTestElementSerializer
  {
    readonly BehaviorFactory _behaviorFactory;
    readonly BehaviorSpecificationFactory _behaviorSpecificationFactory;
    readonly ContextFactory _contextFactory;
    readonly ContextSpecificationFactory _contextSpecificationFactory;
    readonly MSpecUnitTestProvider _provider;
    readonly ISolution _solution;

    public MSpecUnitTestElementSerializer(ISolution solution,
                                          MSpecUnitTestProvider provider,
                                          ContextFactory contextFactory,
                                          ContextSpecificationFactory contextSpecificationFactory,
                                          BehaviorFactory behaviorFactory,
                                          BehaviorSpecificationFactory behaviorSpecificationFactory)
    {
      _solution = solution;
      _provider = provider;
      _contextFactory = contextFactory;
      _contextSpecificationFactory = contextSpecificationFactory;
      _behaviorFactory = behaviorFactory;
      _behaviorSpecificationFactory = behaviorSpecificationFactory;
    }

    public void SerializeElement(XmlElement parent, IUnitTestElement element)
    {
      var e = element as ISerializableElement;
      if (e == null)
      {
        return;
      }

      e.WriteToXml(parent);
      parent.SetAttribute("elementType", e.GetType().Name);
    }

    public IUnitTestElement DeserializeElement(XmlElement parent, IUnitTestElement parentElement)
    {
      var typeName = parent.GetAttribute("elementType");

      if (Equals(typeName, "ContextElement"))
      {
        return ContextElement.ReadFromXml(parent,
                                          _solution,
                                          _contextFactory);
      }
      if (Equals(typeName, "BehaviorElement"))
      {
        return BehaviorElement.ReadFromXml(parent,
                                           parentElement,
                                           _solution,
                                           _behaviorFactory);
      }
      if (Equals(typeName, "BehaviorSpecificationElement"))
      {
        return BehaviorSpecificationElement.ReadFromXml(parent,
                                                        parentElement,
                                                        _solution,
                                                        _behaviorSpecificationFactory);
      }
      if (Equals(typeName, "ContextSpecificationElement"))
      {
        return ContextSpecificationElement.ReadFromXml(parent,
                                                       parentElement,
                                                       _solution,
                                                       _contextSpecificationFactory);
      }

      return null;
    }

    public IUnitTestProvider Provider
    {
      get { return _provider; }
    }
  }
}