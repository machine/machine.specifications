using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Machine.Specifications.Analyzers;

public static class TypeDeclarationSyntaxExtensions
{
    public static bool IsSpecificationClass(this TypeDeclarationSyntax type, SyntaxNodeAnalysisContext context)
    {
        return type
            .DescendantNodesAndSelf()
            .OfType<TypeDeclarationSyntax>()
            .Any(x => x.HasSpecificationMember(context));
    }

    public static bool HasSpecificationMember(this TypeDeclarationSyntax declaration, SyntaxNodeAnalysisContext context)
    {
        return declaration.Members
            .OfType<FieldDeclarationSyntax>()
            .Any(x => x.IsSpecification(context));
    }
}
