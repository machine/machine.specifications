using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications.Formatting;

namespace Machine.Specifications.Reflection;

public class ObjectTypeComparer : INodeTypeComparer
{
    private readonly ITypeComparer comparer;

    public ObjectTypeComparer(ITypeComparer comparer)
    {
        this.comparer = comparer;
    }

    public NodeType Type => NodeType.Object;

    public void Compare(CompareContext context, Node node)
    {
        var valueMembers = GetMembers(node.Value).ToArray();
        var expectedMembers = GetMembers(node.Expected);

        foreach (var (name, expectedValue) in expectedMembers)
        {
            var fullName = string.IsNullOrEmpty(node.Name)
                ? name
                : $"{node.Name}.{name}";

            var actualMember = valueMembers.SingleOrDefault(k => k.Item1 == name);

            if (actualMember == null)
            {
                var errorMessage = string.Format("  Expected: {1}{0}  But was:  Not Defined", Environment.NewLine, expectedValue.Format());

                context.Exceptions.Add(Exceptions.Specification($"{{0}}:{Environment.NewLine}{errorMessage}", fullName));
            }
            else
            {
                comparer.Compare(context, new Node(fullName, actualMember.Item2, expectedValue));
            }
        }
    }

    private IEnumerable<Tuple<string, object>> GetMembers(object value)
    {
        var type = value.GetType();

        var properties = type
            .GetProperties()
            .Where(x => x.CanRead && !x.GetGetMethod().IsStatic)
            .Select(x => Tuple.Create(x.Name, x.GetValue(value, null)));

        var fields = type
            .GetFields()
            .Select(x => Tuple.Create(x.Name, x.GetValue(value)));

        return properties.Concat(fields).OrderBy(m => m.Item1);
    }
}
