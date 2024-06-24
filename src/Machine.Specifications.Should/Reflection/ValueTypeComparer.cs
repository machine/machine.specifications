using System;

namespace Machine.Specifications.Reflection;

public class ValueTypeComparer : INodeTypeComparer
{
    public NodeType Type => NodeType.Value;

    public void Compare(CompareContext context, Node node)
    {
        try
        {
            try
            {
                node.Value.ShouldEqual(node.Expected);
            }
            catch (SpecificationException ex)
            {
                context.Exceptions.Add(Exceptions.Specification($"{{0}}:{Environment.NewLine}{ex.Message}", node.Name));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
