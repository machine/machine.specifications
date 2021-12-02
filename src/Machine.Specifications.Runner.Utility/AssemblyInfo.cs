using System;
using System.Xml.Linq;

namespace Machine.Specifications.Runner.Utility
{
    [Serializable]
    public class AssemblyInfo
    {
        public AssemblyInfo(string name, string location)
        {
            Name = name;
            Location = location;
        }

        public AssemblyInfo()
        {
        }

        public string Name { get; }

        public string Location { get; }

        public string CapturedOutput { get; set; }

        public static AssemblyInfo Parse(string assemblyInfoXml)
        {
            var document = XDocument.Parse(assemblyInfoXml);
            var name = document.SafeGet<string>("/assemblyinfo/name");
            var location = document.SafeGet<string>("/assemblyinfo/location");
            var capturedOutput = document.SafeGet<string>("/assemblyinfo/capturedoutput");

            return new AssemblyInfo(name, location)
            {
                CapturedOutput = capturedOutput
            };
        }
    }
}
