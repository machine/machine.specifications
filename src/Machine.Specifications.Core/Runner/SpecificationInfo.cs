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

        public string Leader { get; set; }
        public string Name { get; set; }
        public string ContainingType { get; set; }
        public string FieldName { get; set; }
        public string CapturedOutput { get; set; }
    }
}