namespace Machine.Specifications.Runner
{
    using System.Xml.Linq;

    internal static class AssemblyInfoExtensions
    {
        public static XElement ToXml(this AssemblyInfo assemblyInfo)
        {
            return new XElement(
                "assemblyinfo",
                new XElement("name", assemblyInfo.Name),
                new XElement("location", assemblyInfo.Location),
                new XElement("captureoutput", assemblyInfo.CapturedOutput));
        }
    }
}