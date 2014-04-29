namespace Machine.Specifications.ReSharperProvider.Presentation
{
    using System.Xml;

    public interface ISerializableElement
    {
        void WriteToXml(XmlElement parent);
    }
}