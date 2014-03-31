namespace Machine.Specifications.Runner
{
    using System.Xml.Linq;

    public static class SpecificationInfoExtensions
    {
        public static string ToXml(this SpecificationInfo specificationInfo)
        {
            return new XElement("specificationinfo",
                new XElement("leader", specificationInfo.Leader),
                new XElement("name", specificationInfo.Name),
                new XElement("containingtype", specificationInfo.ContainingType),
                new XElement("fieldname", specificationInfo.FieldName),
                new XElement("captureoutput", specificationInfo.CapturedOutput))
                .ToString();
        }
    }
}