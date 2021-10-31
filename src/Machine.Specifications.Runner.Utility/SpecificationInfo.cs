﻿using System;
using System.Xml.Linq;

namespace Machine.Specifications.Runner.Utility
{
    [Serializable]
    public class SpecificationInfo
    {
        public SpecificationInfo()
        {
        }

        public SpecificationInfo(string leader, string name, string containingType, string fieldName)
        {
            Leader = leader;
            Name = name;
            ContainingType = containingType;
            FieldName = fieldName;
        }

        public string Leader { get; set; }

        public string Name { get; set; }

        public string ContainingType { get; set; }

        public string FieldName { get; set; }

        public string CapturedOutput { get; set; }

        public static SpecificationInfo Parse(string specificationInfoXml)
        {
            var document = XDocument.Parse(specificationInfoXml);
            var leader = document.SafeGet<string>("/specificationinfo/leader");
            var name = document.SafeGet<string>("/specificationinfo/name");
            var containingType = document.SafeGet<string>("/specificationinfo/containingtype");
            var fieldName = document.SafeGet<string>("/specificationinfo/fieldname");
            var capturedOutput = document.SafeGet<string>("/specificationinfo/capturedoutput");

            return new SpecificationInfo(leader, name, containingType, fieldName)
            {
                CapturedOutput = capturedOutput
            };
        }
    }
}
