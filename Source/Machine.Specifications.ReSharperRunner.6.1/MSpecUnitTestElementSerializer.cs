using System.Xml;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;

using Machine.Specifications.ReSharperRunner.Presentation;

namespace Machine.Specifications.ReSharperRunner
{
  [SolutionComponent]
  public class MSpecUnitTestElementSerializer : IUnitTestElementSerializer
  {
    readonly MSpecUnitTestProvider _provider;

    public MSpecUnitTestElementSerializer(MSpecUnitTestProvider provider)
    {
      _provider = provider;
    }

    public void SerializeElement(XmlElement parent, IUnitTestElement element)
    {
      var e = element as ISerializableElement;
      if (e != null)
      {
        e.WriteToXml(parent);
        parent.SetAttribute("elementType", e.GetType().Name);
      }
    }

    public IUnitTestElement DeserializeElement(XmlElement parent, IUnitTestElement parentElement)
    {
      var typeName = parent.GetAttribute("elemenType");

      if (Equals(typeName, "ContextElement"))
        return ContextElement.ReadFromXml(parent, parentElement, _provider);
      if (Equals(typeName, "BehaviorElement"))
        return BehaviorElement.ReadFromXml(parent, parentElement, _provider);
      if (Equals(typeName, "BehaviorSpecificationElement"))
        return BehaviorSpecificationElement.ReadFromXml(parent, parentElement, _provider);
      if (Equals(typeName, "ContextSpecificationElement"))
        return ContextSpecificationElement.ReadFromXml(parent, parentElement, _provider);

      return null;
    }

    public IUnitTestProvider Provider
    {
      get { return _provider; }
    }
  }
}