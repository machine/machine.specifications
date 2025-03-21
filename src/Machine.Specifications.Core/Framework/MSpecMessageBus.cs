using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Extensions.TestFramework;

namespace Machine.Specifications.Framework;

public class MSpecMessageBus(IExtension extension, ExecuteRequestContext context) : IMSpecMessageBus, IDataProducer
{
    public string Uid => extension.Uid;

    public string Version => extension.Version;

    public string DisplayName => extension.DisplayName;

    public string Description => extension.Description;

    public Type[] DataTypesProduced { get; } =
    [
        typeof(TestNodeUpdateMessage),
        typeof(SessionFileArtifact),
        typeof(TestNodeFileArtifact)
    ];

    public async ValueTask Discovered()
    {
        var node = new TestNode
        {
            Uid = new TestNodeUid("testId"),
            DisplayName = "display"
        };

        await context.MessageBus.PublishAsync(this, new TestNodeUpdateMessage(context.Request.Session.SessionUid, node));
    }

    public ValueTask InProgress()
    {
        throw new NotImplementedException();
    }

    public ValueTask Passed()
    {
        throw new NotImplementedException();
    }

    public ValueTask Failed()
    {
        throw new NotImplementedException();
    }

    public ValueTask Skipped()
    {
        throw new NotImplementedException();
    }

    public ValueTask Cancelled()
    {
        throw new NotImplementedException();
    }

    public ValueTask SessionArtifact()
    {
        throw new NotImplementedException();
    }

    public ValueTask TestArtifact()
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsEnabledAsync()
    {
        throw new NotImplementedException();
    }
}
