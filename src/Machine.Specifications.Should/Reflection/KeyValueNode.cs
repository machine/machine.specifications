using System.Collections.Generic;

namespace Machine.Specifications.Reflection
{
    internal class KeyValueNode : INode
    {
        public IEnumerable<Member> KeyValues { get; set; }
    }
}
