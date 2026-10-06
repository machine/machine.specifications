using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Machine.Specifications.Core.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public class SpecificationMetadataGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var enabled = context.AnalyzerConfigOptionsProvider
            .Select((options, _) =>
            {
                options.GlobalOptions.TryGetValue("build_property.EnableMSpecSourceGeneration", out var value);

                return !string.Equals(value, "false", StringComparison.OrdinalIgnoreCase);
            });

        var contexts = context.SyntaxProvider.CreateSyntaxProvider(
                (node, _) => IsClassWithFields(node),
                (syntaxContext, _) => GetContextInfo(syntaxContext))
            .Where(x => x != null)
            .Combine(enabled);

        context.RegisterSourceOutput(
            contexts,
            (producitonContext, info) =>
            {

            });
    }

    private bool IsClassWithFields(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax classDeclaration &&
               classDeclaration.Members.OfType<FieldDeclarationSyntax>().Any();
    }

    private ContextInfo? GetContextInfo(GeneratorSyntaxContext context)
    {
        var specifications = new List<string>();

        var specificationType = context.SemanticModel.Compilation.GetTypeByMetadataName("Machine.Specifications.It");

        if (context.Node is ClassDeclarationSyntax classDeclaration)
        {
            var variables = classDeclaration.Members.OfType<FieldDeclarationSyntax>()
                .SelectMany(x => x.Declaration.Variables);

            foreach (var variable in variables)
            {
                if (context.SemanticModel.GetDeclaredSymbol(variable) is IFieldSymbol field &&
                    SymbolEqualityComparer.Default.Equals(field.Type, specificationType))
                {
                    specifications.Add(field.Name);
                }
            }

            if (specifications.Any())
            {
                return new ContextInfo
                {
                    ContextType = classDeclaration.Identifier.Text,
                    Specifications = specifications
                };
            }
        }

        return null;
    }

    private class ContextInfo
    {
        public string ContextType { get; set; } = string.Empty;

        public IReadOnlyCollection<string> Specifications { get; set; } = [];
    }
}
