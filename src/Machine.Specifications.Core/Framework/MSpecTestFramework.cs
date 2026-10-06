using System.Reflection;
using Machine.Specifications.Execution;
using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Extensions.TestFramework;
using Microsoft.Testing.Platform.Messages;
using Microsoft.Testing.Platform.Requests;

namespace Machine.Specifications.Framework;

public class MSpecTestFramework(IExtension extension, Func<IEnumerable<Assembly>> assemblyProvider) : ITestFramework, IDataProducer
{
    public string Uid { get; } = extension.Uid;

    public string Version { get; } = extension.Version;

    public string DisplayName { get; } = extension.DisplayName;

    public string Description { get; } = extension.Description;

    public Type[] DataTypesProduced { get; } = [typeof(TestNodeUpdateMessage)];

    public Task<CreateTestSessionResult> CreateTestSessionAsync(CreateTestSessionContext context)
    {
        return Task.FromResult(new CreateTestSessionResult
        {
            IsSuccess = true
        });
    }

    public Task<CloseTestSessionResult> CloseTestSessionAsync(CloseTestSessionContext context)
    {
        return Task.FromResult(new CloseTestSessionResult
        {
            IsSuccess = true
        });
    }

    public async Task ExecuteRequestAsync(ExecuteRequestContext context)
    {
        try
        {
            if (context.Request is DiscoverTestExecutionRequest discoverRequest)
            {
                await DiscoverTestsAsync(discoverRequest, context.MessageBus, context.CancellationToken);

                return;
            }

            if (context.Request is RunTestExecutionRequest executionRequest)
            {
                await RunTestsAsync(executionRequest, context.MessageBus, context.CancellationToken);

                return;
            }

            throw new ArgumentOutOfRangeException(nameof(context.Request), context.Request.GetType().Name);
        }
        catch (Exception e)
        {
            var node = new TestNode
            {
                Uid = Guid.NewGuid().ToString(),
                DisplayName = $"Unhandled exception - {e.GetType().Name}: {e.Message}",
                Properties = new PropertyBag(new ErrorTestNodeStateProperty(e))
            };

            await context.MessageBus.PublishAsync(this, new TestNodeUpdateMessage(context.Request.Session.SessionUid, node));
        }
        finally
        {
            context.Complete();
        }
    }

    public Task<bool> IsEnabledAsync()
    {
        return extension.IsEnabledAsync();
    }

    private async Task DiscoverTestsAsync(DiscoverTestExecutionRequest request, IMessageBus messageBus, CancellationToken cancellationToken)
    {
        var nodes = assemblyProvider()
            .SelectMany(x => new TestCaseDiscoverer(x).GetTests())
            .Select(x => x.ToTestNode(DiscoveredTestNodeStateProperty.CachedInstance));

        foreach (var node in nodes)
        {
            await messageBus.PublishAsync(this, new TestNodeUpdateMessage(request.Session.SessionUid, node));
        }
    }

    private async Task RunTestsAsync(RunTestExecutionRequest request, IMessageBus messageBus, CancellationToken cancellationToken)
    {
        var tests = assemblyProvider()
            .SelectMany(x => new TestCaseDiscoverer(x).GetTests());

        foreach (var test in tests)
        {
            var inProgress = test.ToTestNode(InProgressTestNodeStateProperty.CachedInstance);

            await messageBus.PublishAsync(this, new TestNodeUpdateMessage(request.Session.SessionUid, inProgress));

            Thread.Sleep(5000);

            var passed = test.ToTestNode(PassedTestNodeStateProperty.CachedInstance);

            await messageBus.PublishAsync(this, new TestNodeUpdateMessage(request.Session.SessionUid, passed));
        }
    }
}
