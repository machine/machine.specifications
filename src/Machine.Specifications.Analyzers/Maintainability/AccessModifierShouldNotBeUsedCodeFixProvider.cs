using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Machine.Specifications.Analyzers.Maintainability;

[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp)]
public class AccessModifierShouldNotBeUsedCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(DiagnosticIds.Maintainability.AccessModifierShouldNotBeUsed);

    public override FixAllProvider GetFixAllProvider()
    {
        return WellKnownFixAllProviders.BatchFixer;
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
            .ConfigureAwait(false);

        if (root == null)
        {
            return;
        }

        foreach (var diagnostic in context.Diagnostics)
        {
            var node = root.FindNode(diagnostic.Location.SourceSpan, true);

            if (node.IsMissing)
            {
                continue;
            }

            var declaration = GetParentDeclaration(node);

            if (declaration == null)
            {
                continue;
            }

            context.RegisterCodeFix(
                CodeAction.Create(
                    "Remove access modifier",
                    _ => TransformAsync(context.Document, root, declaration),
                    nameof(AccessModifierShouldNotBeUsedCodeFixProvider)),
                diagnostic);
        }
    }

    private Task<Document> TransformAsync(Document document, SyntaxNode root, SyntaxNode declaration)
    {
        var fixedDeclaration = declaration.Kind() switch
        {
            SyntaxKind.ClassDeclaration => HandleDeclaration((ClassDeclarationSyntax) declaration),
            SyntaxKind.FieldDeclaration => HandleDeclaration((FieldDeclarationSyntax) declaration),
            _ => declaration
        };

        var fixedRoot = root.ReplaceNode(declaration, fixedDeclaration);

        return Task.FromResult(document.WithSyntaxRoot(fixedRoot));
    }

    private SyntaxNode HandleDeclaration(MemberDeclarationSyntax declaration)
    {
        if (!declaration.Modifiers.Any())
        {
            return declaration;
        }

        var trivia = declaration.Modifiers.First().LeadingTrivia;

        var modifiers = declaration.Modifiers
            .Where(x => !IsAccessModifer(x))
            .ToArray();

        return declaration
            .WithModifiers(SyntaxFactory.TokenList(modifiers))
            .WithLeadingTrivia(trivia);
    }

    private SyntaxNode? GetParentDeclaration(SyntaxNode? declaration)
    {
        while (declaration != null)
        {
            if (declaration.IsKind(SyntaxKind.ClassDeclaration) || declaration.IsKind(SyntaxKind.FieldDeclaration))
            {
                return declaration;
            }

            declaration = declaration.Parent;
        }

        return null;
    }

    private bool IsAccessModifer(SyntaxToken token)
    {
        return token.IsKind(SyntaxKind.PublicKeyword) ||
               token.IsKind(SyntaxKind.PrivateKeyword) ||
               token.IsKind(SyntaxKind.ProtectedKeyword) ||
               token.IsKind(SyntaxKind.InternalKeyword);
    }
}
