namespace Machine.Specifications.Runner
{
    using System.Xml.Linq;

    internal static class ContextInfoExtensions
    {
        public static XElement ToXml(this ContextInfo contextInfo)
        {
            return
                new XElement(
                    "contextinfo",
                    new XElement("name", contextInfo.Name),
                    new XElement("concern", contextInfo.Concern),
                    new XElement("typename", contextInfo.TypeName),
                    new XElement("namespace", contextInfo.Namespace),
                    new XElement("assemblyname", contextInfo.AssemblyName),
                    new XElement("assemblyname", contextInfo.AssemblyName),
                    new XElement("fullname", contextInfo.FullName),
                    new XElement("capturedoutput", contextInfo.CapturedOutput));
        }
    }
}