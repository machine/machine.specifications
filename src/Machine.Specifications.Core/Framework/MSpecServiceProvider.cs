using Microsoft.Testing.Extensions.TrxReport.Abstractions;
using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Extensions.TestFramework;
using Microsoft.Testing.Platform.Logging;
using Microsoft.Testing.Platform.Messages;
using Microsoft.Testing.Platform.OutputDevice;
using Microsoft.Testing.Platform.Services;

namespace Machine.Specifications.Framework;

public class MSpecServiceProvider : IServiceProvider, IAsyncDisposable
{
    public MSpecServiceProvider(
        IExtension extension,
        ExecuteRequestContext context,
        IMessageBus messageBus,
        IServiceProvider serviceProvider)
    {
        LoggerFactory = serviceProvider.GetLoggerFactory();
        OutputDevice = serviceProvider.GetOutputDevice();
        MessageBus = new MSpecMessageBus(messageBus);
    }

    public ILoggerFactory LoggerFactory { get; }

    public IOutputDevice OutputDevice { get; }

    public IMSpecMessageBus MessageBus { get; }

    public object? GetService(Type serviceType)
    {
        throw new NotImplementedException();
    }

    public ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }
}
