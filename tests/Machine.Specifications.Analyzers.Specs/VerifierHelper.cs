using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;

namespace Machine.Specifications.Analyzers.Tests;

internal static class VerifierHelper
{
    internal static ImmutableDictionary<string, ReportDiagnostic> NullableWarnings { get; } = GetNullableWarningsFromCompiler();

    internal static ReferenceAssemblies MspecAssemblies { get; } = ReferenceAssemblies.Default.AddPackages(
        ImmutableArray.Create(
            new PackageIdentity("Machine.Specifications", "1.0.0"),
            new PackageIdentity("Machine.Specifications.Should", "1.0.0")));

    public static Solution GetNullableTransform(Solution solution, ProjectId projectId)
    {
        var project = solution.GetProject(projectId);

        var options = project!.CompilationOptions!.WithSpecificDiagnosticOptions(
            project.CompilationOptions.SpecificDiagnosticOptions.SetItems(NullableWarnings));

        return solution.WithProjectCompilationOptions(projectId, options);
    }

    private static ImmutableDictionary<string, ReportDiagnostic> GetNullableWarningsFromCompiler()
    {
        string[] args = { "/warnaserror:nullable" };
        var commandLineArguments = CSharpCommandLineParser.Default.Parse(args, Environment.CurrentDirectory, Environment.CurrentDirectory);
        var nullableWarnings = commandLineArguments.CompilationOptions.SpecificDiagnosticOptions;

        return nullableWarnings
            .SetItem("CS8632", ReportDiagnostic.Error)
            .SetItem("CS8669", ReportDiagnostic.Error);
    }
}
