using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Extensions.TestFramework;
using Microsoft.Testing.Platform.Requests;

namespace Machine.Specifications.Framework;

public class MSpecTestFramework(IExtension extension, IServiceProvider serviceProvider, Assembly assembly) : ITestFramework, IDataProducer
{
    private static readonly ConcurrentDictionary<string, MSpecServiceProvider> ServiceProviders = [];

    public string Uid => extension.Uid;

    public string Version => extension.Version;

    public string DisplayName => extension.DisplayName;

    public string Description => extension.Description;

    public Type[] DataTypesProduced { get; } = [typeof(TestNodeUpdateMessage)];

    public Task<CloseTestSessionResult> CloseTestSessionAsync(CloseTestSessionContext context)
    {
        return Task.FromResult(new CloseTestSessionResult
        {
            IsSuccess = true
        });
    }

    public Task<CreateTestSessionResult> CreateTestSessionAsync(CreateTestSessionContext context)
    {
        return Task.FromResult(new CreateTestSessionResult
        {
            IsSuccess = true
        });
    }

    public async Task ExecuteRequestAsync(ExecuteRequestContext context)
    {
        var services = ServiceProviders.GetOrAdd(context.Request.Session.SessionUid.Value,
            _ => new MSpecServiceProvider(extension, context, context.MessageBus, serviceProvider));

        try
        {
            if (context.Request is DiscoverTestExecutionRequest discoverRequest)
            {
            }
            else if (context.Request is RunTestExecutionRequest executionRequest)
            {
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(context.Request), context.Request.GetType().Name);
            }
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
}
