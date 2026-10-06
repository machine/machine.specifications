using Microsoft.Testing.Extensions.TrxReport.Abstractions;

namespace Machine.Specifications.Capabilities;

internal class TrxReportCapability : ITrxReportCapability
{
    public bool IsSupported { get; } = true;

    public void Enable()
    {
    }
}
