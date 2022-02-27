namespace Machine.Specifications.Reflection
{
    internal class KeyValueNode : INode
    {
        public KeyValueNode(Member[] keyValues)
        {
            KeyValues = keyValues;
        }

        public Member[] KeyValues { get; }
    }
}
