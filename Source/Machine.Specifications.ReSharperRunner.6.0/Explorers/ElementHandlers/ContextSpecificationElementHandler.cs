using System.Collections.Generic;

using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;

using Machine.Specifications.ReSharperRunner.Factories;

namespace Machine.Specifications.ReSharperRunner.Explorers.ElementHandlers
{
  internal class ContextSpecificationElementHandler : IElementHandler
  {
    readonly ContextSpecificationFactory _contextSpecificationFactory;

    public ContextSpecificationElementHandler(ContextSpecificationFactory contextSpecificationFactory)
    {
      _contextSpecificationFactory = contextSpecificationFactory;
    }

    public bool Accepts(ITreeNode element)
    {
      IDeclaration declaration = element as IDeclaration;
      if (declaration == null)
      {
        return false;
      }

      return declaration.DeclaredElement.IsSpecification();
    }

    public IEnumerable<UnitTestElementDisposition> AcceptElement(ITreeNode element, IFile file)
    {
      IDeclaration declaration = (IDeclaration)element;
      var contextSpecificationElement =
        _contextSpecificationFactory.CreateContextSpecification(declaration.DeclaredElement);

      if (contextSpecificationElement == null)
      {
        yield break;
      }

      yield return new UnitTestElementDisposition(contextSpecificationElement,
                                                  file.GetSourceFile().ToProjectFile(),
                                                  declaration.GetNavigationRange().TextRange,
                                                  declaration.GetDocumentRange().TextRange);
    }

    public void Cleanup(ITreeNode element)
    {
    }
  }
}