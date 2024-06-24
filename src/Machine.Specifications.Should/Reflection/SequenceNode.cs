using System;

namespace Machine.Specifications.Reflection
{
    internal class SequenceNode : INode
    {
        public SequenceNode(Func<object>[] valueGetters)
        {
            ValueGetters = valueGetters;
        }

        public Func<object>[] ValueGetters { get; }
    }
}
