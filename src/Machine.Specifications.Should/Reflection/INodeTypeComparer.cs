namespace Machine.Specifications.Reflection;

public interface INodeTypeComparer : ITypeComparer
{
    NodeType Type { get; }
}
