using System.Xml;

using JetBrains.ReSharper.UnitTestFramework;

namespace Machine.Specifications.ReSharperRunner
{
    public interface ISerializableUnitTestElement
    {
        void WriteToXml(XmlElement parent);
        IUnitTestElement ReadFromXml(XmlElement parent, IUnitTestElement parentElement, MSpecUnitTestProvider provider);
    }
}