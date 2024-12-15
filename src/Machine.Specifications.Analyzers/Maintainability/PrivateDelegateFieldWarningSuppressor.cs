using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Machine.Specifications.Analyzers.Maintainability;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class PrivateDelegateFieldWarningSuppressor : DiagnosticSuppressor
{
    private static readonly SuppressionDescriptor Descriptor =
        new SuppressionDescriptor(DiagnosticIds.Maintainability.PrivateDelegateFieldWarning, "IDE0051", "Private delegate field used by Machine.Specifications runner");

    private static readonly Type[] SuppressedAttributeTypes =
    [
        typeof(SetupDelegateAttribute),
        typeof(ActDelegateAttribute),
        typeof(BehaviorDelegateAttribute),
        typeof(AssertDelegateAttribute),
        typeof(CleanupDelegateAttribute)
    ];

    public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions { get; } = [Descriptor];

    public override void ReportSuppressions(SuppressionAnalysisContext context)
    {
        foreach (var diagnostic in context.ReportedDiagnostics)
        {
            AnalyzeDiagnostic(diagnostic, context);
        }
    }

    private void AnalyzeDiagnostic(Diagnostic diagnostic, SuppressionAnalysisContext context)
    {
        var fieldDeclarationSyntax = context.GetSuppressibleNode<VariableDeclaratorSyntax>(diagnostic);

        if (fieldDeclarationSyntax == null)
        {
            return;
        }

        var syntaxTree = diagnostic.Location.SourceTree;

        if (syntaxTree == null)
        {
            return;
        }

        var model = context.GetSemanticModel(syntaxTree);

        if (model.GetDeclaredSymbol(fieldDeclarationSyntax) is not IFieldSymbol fieldSymbol)
        {
            return;
        }

        if (!IsSuppressable(fieldSymbol))
        {
            return;
        }

        foreach (var descriptor in SupportedSuppressions.Where(d => d.SuppressedDiagnosticId == diagnostic.Id))
        {
            context.ReportSuppression(Suppression.Create(descriptor, diagnostic));
        }
    }

    private static bool IsSuppressable(IFieldSymbol fieldSymbol)
    {
        var fieldTypeAttributes = fieldSymbol.Type.GetAttributes()
            .Where(x => x.AttributeClass != null)
            .Select(x => x.AttributeClass!);


        if (fieldTypeAttributes.Any(x => SuppressedAttributeTypes.Any(y => x.Matches(y))))
        {
            return true;
        }

        if (fieldSymbol.DeclaredAccessibility == Accessibility.Public && fieldSymbol.ContainingType.Extends(typeof(object)))
        {
            return true;
        }

        return false;
    }
}
