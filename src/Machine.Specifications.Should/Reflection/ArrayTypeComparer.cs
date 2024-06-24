using System;
using System.Collections;
using System.Linq;

namespace Machine.Specifications.Reflection;

public class ArrayTypeComparer : INodeTypeComparer
{
    private readonly ITypeComparer comparer;

    public ArrayTypeComparer(ITypeComparer comparer)
    {
        this.comparer = comparer;
    }

    public NodeType Type => NodeType.Array;

    public void Compare(CompareContext context, Node node)
    {
        if (node.Value is IEnumerable value && node.Expected is IEnumerable expected)
        {
            var values = value.Cast<object>().ToArray();
            var expectedValues = expected.Cast<object>().ToArray();

            if (values.Length != expectedValues.Length)
            {
                var errorMessage = string.Format(
                    "  Expected: Sequence length of {1}{0}  But was:  {2}",
                    Environment.NewLine,
                    expectedValues.Length,
                    values.Length);

                context.Exceptions.Add(Exceptions.Specification($"{{0}}:{Environment.NewLine}{errorMessage}", node.Name));
            }
            else
            {
                for (var i = 0; i < values.Length; i++)
                {
                    comparer.Compare(context, new Node($"{node.Name}[{i}]", values[i], expectedValues[i]));
                }
            }
        }
    }
}
