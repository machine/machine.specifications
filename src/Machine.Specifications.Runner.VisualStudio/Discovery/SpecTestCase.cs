using System;

namespace Machine.Specifications.Runner.VisualStudio.Discovery
{
#if NETFRAMEWORK
    [Serializable]
#endif
    public class SpecTestCase
    {
        public string Subject { get; set; }

        public string ContextFullType { get; set; }

        public object ContextDisplayName { get; set; }

        public string ClassName { get; set; }

        public string SpecificationDisplayName { get; set; }

        public string SpecificationName { get; set; }

        public string BehaviorFieldName { get; set; }

        public string BehaviorFieldType { get; set; }

        public string CodeFilePath { get; set; }

        public int LineNumber { get; set; }

        public string[] Tags { get; set; }
    }
}
