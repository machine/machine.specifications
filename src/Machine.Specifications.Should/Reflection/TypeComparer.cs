using System.Collections;
using System.Linq;

namespace Machine.Specifications.Reflection;

public class TypeComparer : ITypeComparer
{
    private readonly INodeTypeComparer[] comparers;

    public TypeComparer()
    {
        comparers = new INodeTypeComparer[]
        {
            new ObjectTypeComparer(this),
            new ArrayTypeComparer(this),
            new ValueTypeComparer()
        };
    }

    public void Compare(CompareContext context, Node node)
    {
        var current = (node.Value, node.Expected);

        if (context.Visited.Add(current))
        {
            var valueType = GetNodeType(node.Value);
            var expectedType = GetNodeType(node.Expected);

            var comparer = comparers.First(x => x.Type == valueType);

            comparer.Compare(context, node);
        }
    }

    private NodeType GetNodeType(object value)
    {
        var type = value.GetType();

        return value switch
        {
            IEnumerable when type != typeof(string) => NodeType.Array,
            not null when type.IsClass && type != typeof(string) => NodeType.Object,
            _ => NodeType.Value
        };
    }
}
