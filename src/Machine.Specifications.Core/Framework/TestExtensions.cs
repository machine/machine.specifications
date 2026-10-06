using Machine.Specifications.Execution;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Machine.Specifications.Framework;

public static class TestExtensions
{
    public static TestNode ToTestNode(this TestContext context, TestNodeStateProperty stateProperty)
    {
        var properties = new PropertyBag();
        properties.Add(stateProperty);

        return new TestNode
        {
            Uid = new TestNodeUid(context.TestId),
            DisplayName = context.GetDisplayName(),
            Properties = properties
        };
    }
}
