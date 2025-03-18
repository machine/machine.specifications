using Machine.Specifications.Capabilities;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Capabilities.TestFramework;
using Microsoft.Testing.Platform.Helpers;

namespace Machine.Specifications.Framework;

public static class TestApplicationBuilderExtensions
{
    public static void AddMSpec(this ITestApplicationBuilder builder)
    {
        var extension = new MSpecExtension();

        builder.RegisterTestFramework(
            services => new TestFrameworkCapabilities(new TrxReportCapability()),
            (capabilities, services) => new MSpecTestFramework(extension, services));

        builder.AddTreeNodeFilterService(extension);
    }
}
