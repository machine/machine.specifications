using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Machine.Specifications.Analyzers;

internal static class SuppressionAnalysisContextExtensions
{
    public static T? GetSuppressibleNode<T>(this SuppressionAnalysisContext context, Diagnostic diagnostic)
        where T : SyntaxNode
    {
        return GetSuppressibleNode<T>(context, diagnostic, _ => true);
    }

    public static T? GetSuppressibleNode<T>(this SuppressionAnalysisContext context, Diagnostic diagnostic, Func<T, bool> predicate)
        where T : SyntaxNode
    {
        var root = diagnostic.Location.SourceTree?.GetRoot(context.CancellationToken);

        return root?
            .FindNode(diagnostic.Location.SourceSpan)
            .DescendantNodesAndSelf()
            .OfType<T>()
            .FirstOrDefault(predicate);
    }
}
