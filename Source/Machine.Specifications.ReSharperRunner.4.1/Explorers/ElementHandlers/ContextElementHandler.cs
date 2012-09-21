using System.Collections.Generic;

using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;

using Machine.Specifications.ReSharperRunner.Factories;

namespace Machine.Specifications.ReSharperRunner.Explorers.ElementHandlers
{
  internal class ContextElementHandler : IElementHandler
  {
    readonly ContextFactory _contextFactory;

    public ContextElementHandler(ContextFactory contextFactory)
    {
      _contextFactory = contextFactory;
    }

    public bool Accepts(ITreeNode element)
    {
      IDeclaration declaration = element as IDeclaration;
      if (declaration == null)
      {
        return false;
      }

      return declaration.DeclaredElement.IsContext();
    }

    public IEnumerable<UnitTestElementDisposition> AcceptElement(ITreeNode element, IFile file)
    {
      IDeclaration declaration = (IDeclaration)element;
      var contextElement = _contextFactory.CreateContext((ITypeElement)declaration.DeclaredElement);

      if (contextElement == null)
      {
        yield break;
      }

      yield return new UnitTestElementDisposition(contextElement,
                                                  file.GetSourceFile().ToProjectFile(),
                                                  declaration.GetNavigationRange().TextRange,
                                                  declaration.GetDocumentRange().TextRange);
    }

    public void Cleanup(ITreeNode element)
    {
      var declaration = (IDeclaration)element;
      _contextFactory.UpdateChildState((IClass)declaration.DeclaredElement);
    }
  }
}