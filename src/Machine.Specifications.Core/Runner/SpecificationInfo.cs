using System;

namespace Machine.Specifications.Runner
{
#if !NETSTANDARD
    [Serializable]
#endif
    public class SpecificationInfo
    {
        public SpecificationInfo(string leader, string name, string containingType, string fieldName)
        {
            Leader = leader;
            Name = name;
            ContainingType = containingType;
            FieldName = fieldName;
        }

        public string Leader { get; }

        public string Name { get; }

        public string ContainingType { get; }

        public string FieldName { get; }

        public string CapturedOutput { get; set; }
    }
}
