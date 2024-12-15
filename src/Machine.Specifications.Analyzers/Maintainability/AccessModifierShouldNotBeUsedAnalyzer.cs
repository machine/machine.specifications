using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Machine.Specifications.Analyzers.Maintainability;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AccessModifierShouldNotBeUsedAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.Maintainability.AccessModifierShouldNotBeUsed,
        "Access modifier should not be declared",
        "Element '{0}' should not declare an access modifier",
        DiagnosticCategories.Maintainability,
        DiagnosticSeverity.Warning,
        true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterSyntaxNodeAction(AnalyzeTypeSyntax, SyntaxKind.ClassDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeFieldSyntax, SyntaxKind.FieldDeclaration);
    }

    private void AnalyzeTypeSyntax(SyntaxNodeAnalysisContext context)
    {
        var type = (TypeDeclarationSyntax) context.Node;

        if (!type.IsSpecificationClass(context))
        {
            return;
        }

        CheckAccessModifier(context, type.Identifier, type.Modifiers);
    }

    private void AnalyzeFieldSyntax(SyntaxNodeAnalysisContext context)
    {
        var field = (FieldDeclarationSyntax) context.Node;

        if (!field.Parent.IsKind(SyntaxKind.ClassDeclaration))
        {
            return;
        }

        if (field.Parent is not TypeDeclarationSyntax type || !type.IsSpecificationClass(context))
        {
            return;
        }

        var variable = field.Declaration.Variables
            .FirstOrDefault(x => !x.Identifier.IsMissing);

        if (variable == null)
        {
            return;
        }

        CheckAccessModifier(context, variable.Identifier, field.Modifiers);
    }

    private void CheckAccessModifier(SyntaxNodeAnalysisContext context, SyntaxToken identifier, SyntaxTokenList modifiers)
    {
        if (identifier.IsMissing)
        {
            return;
        }

        var modifier = modifiers
            .FirstOrDefault(x => x.IsKind(SyntaxKind.PublicKeyword) ||
                                 x.IsKind(SyntaxKind.PrivateKeyword) ||
                                 x.IsKind(SyntaxKind.InternalKeyword) ||
                                 x.IsKind(SyntaxKind.ProtectedKeyword));

        if (!modifier.IsKind(SyntaxKind.None))
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule, identifier.GetLocation(), identifier));
        }
    }
}
