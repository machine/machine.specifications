using System.Xml;

namespace Machine.Specifications.ReSharperRunner.Presentation
{
  public interface ISerializableElement
  {
    void WriteToXml(XmlElement parent);
    //static IUnitTestElement ReadFromXml(XmlElement parent, IUnitTestElement parentElement, MSpecUnitTestProvider provider);
  }
}