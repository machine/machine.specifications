namespace Machine.Specifications.Runner.Utility.SpecsRunner
{
    using System;

    [Serializable]
    public class RemoteSpecificationInfo
    {
        public string Leader { get; set; }
        public string Name { get; set; }
        public string ContainingType { get; set; }
        public string FieldName { get; set; }
        public string CapturedOutput { get; set; }

        public RemoteSpecificationInfo()
        {
        }

        public RemoteSpecificationInfo(string leader, string name, string containingType, string fieldName)
        {
            this.Leader = leader;
            this.Name = name;
            this.ContainingType = containingType;
            this.FieldName = fieldName;
        }
    }
}