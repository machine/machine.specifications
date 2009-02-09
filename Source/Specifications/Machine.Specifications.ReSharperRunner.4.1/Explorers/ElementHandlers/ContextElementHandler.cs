using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestExplorer;

using Machine.Specifications.ReSharperRunner.Factories;
using Machine.Specifications.ReSharperRunner.Presentation;

namespace Machine.Specifications.ReSharperRunner.Explorers.ElementHandlers
{
  internal class ContextElementHandler : IElementHandler
  {
    readonly ContextFactory _contextFactory;

    public ContextElementHandler(ContextFactory contextFactory)
    {
      _contextFactory = contextFactory;
    }

    #region Implementation of IElementHandler
    public bool Accepts(IElement element)
    {
      IDeclaration declaration = element as IDeclaration;
      if (declaration == null)
      {
        return false;
      }

      return declaration.DeclaredElement.IsContext();
    }

    public UnitTestElementDisposition AcceptElement(IElement element, IFile file)
    {
      IDeclaration declaration = (IDeclaration) element;
      Element unitTestElement = _contextFactory.CreateContextElement((ITypeElement) declaration.DeclaredElement);

      if (unitTestElement == null)
      {
        return null;
      }

      return new UnitTestElementDisposition(unitTestElement,
                                            file.ProjectFile,
                                            declaration.GetNameRange(),
                                            declaration.GetDocumentRange().TextRange);
    }
    #endregion
  }
}