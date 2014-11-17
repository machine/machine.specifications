namespace Machine.Specifications.Runner
{
    using System.Xml.Linq;

    internal static class SpecificationInfoExtensions
    {
        public static XElement ToXml(this SpecificationInfo specificationInfo)
        {
            return new XElement(
                "specificationinfo",
                new XElement("leader", specificationInfo.Leader),
                new XElement("name", specificationInfo.Name),
                new XElement("containingtype", specificationInfo.ContainingType),
                new XElement("fieldname", specificationInfo.FieldName),
                new XElement("capturedoutput", specificationInfo.CapturedOutput));
        }
    }
}