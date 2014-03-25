namespace Machine.Specifications.Runner.Utility.SpecsRunner
{
    using System;

    [Serializable]
    public class RemoteAssemblyInfo
    {
        public RemoteAssemblyInfo(string name, string location)
        {
            this.Name = name;
            this.Location = location;
        }

        public RemoteAssemblyInfo()
        {
        }

        public string Name { get; private set; }
        public string Location { get; private set; }
        public string CapturedOutput { get; set; }
    }
}