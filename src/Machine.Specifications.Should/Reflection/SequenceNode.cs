using System;
using System.Collections.Generic;

namespace Machine.Specifications.Reflection
{
    internal class SequenceNode : INode
    {
        public IEnumerable<Func<object>> ValueGetters { get; set; }
    }
}
