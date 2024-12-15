using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Machine.Specifications.Analyzers;

public static class FieldDeclarationSyntaxExtensions
{
    public static bool IsSpecification(this FieldDeclarationSyntax syntax, SyntaxNodeAnalysisContext context)
    {
        var symbolInfo = context.SemanticModel.GetSymbolInfo(syntax.Declaration.Type, context.CancellationToken);
        var symbol = symbolInfo.Symbol as ITypeSymbol;

        if (!symbol.IsMember())
        {
            return false;
        }

        return symbol?.Name is
            MetadataNames.MspecItDelegate or
            MetadataNames.MspecBehavesLikeDelegate or
            MetadataNames.MspecBecause or
            MetadataNames.MspecEstablishDelegate;
    }
}
