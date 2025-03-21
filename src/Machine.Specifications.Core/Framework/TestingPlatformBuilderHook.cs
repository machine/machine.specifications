using System.Reflection;
using Microsoft.Testing.Platform.Builder;

namespace Machine.Specifications.Framework;

public class TestingPlatformBuilderHook
{
    public static void AddExtensions(ITestApplicationBuilder builder, string[] _)
    {
        builder.AddMSpec(Assembly.GetEntryAssembly()!);
    }
}
