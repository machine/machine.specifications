using Microsoft.Testing.Platform.Extensions;

namespace Machine.Specifications.Framework;

public class MSpecExtension : IExtension
{
    public string Uid { get; } = nameof(MSpecExtension);

    public string Version { get; } = typeof(MSpecExtension).Assembly.GetName().Version?.ToString() ?? "1.0.0";

    public string DisplayName { get; } = "MSpec";

    public string Description { get; } = "MSpec Framework for Microsoft Testing Platform";

    public Task<bool> IsEnabledAsync()
    {
        return Task.FromResult(true);
    }
}
