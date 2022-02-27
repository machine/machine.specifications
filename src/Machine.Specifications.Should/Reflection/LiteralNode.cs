namespace Machine.Specifications.Reflection
{
    internal class LiteralNode : INode
    {
        public LiteralNode(object value)
        {
            Value = value;
        }

        public object Value { get; }
    }
}
