using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestExplorer;

using Machine.Specifications.ReSharperRunner.Factories;
using Machine.Specifications.ReSharperRunner.Presentation;

namespace Machine.Specifications.ReSharperRunner.Explorers.ElementHandlers
{
  internal class SpecificationElementHandler : IElementHandler
  {
    readonly ElementFactory _elementFactory;

    public SpecificationElementHandler(ElementFactory elementFactory)
    {
      _elementFactory = elementFactory;
    }

    #region Implementation of IElementHandler
    public bool Accepts(IElement element)
    {
      IDeclaration declaration = element as IDeclaration;
      if (declaration == null)
      {
        return false;
      }

      return declaration.DeclaredElement.IsSpecification();
    }

    public UnitTestElementDisposition AcceptElement(IElement element, IFile file)
    {
      IDeclaration declaration = (IDeclaration) element;
      Element unitTestElement = _elementFactory.CreateSpecificationElement(declaration.DeclaredElement);

      return new UnitTestElementDisposition(unitTestElement,
                                            file.ProjectFile,
                                            declaration.GetNameRange(),
                                            declaration.GetDocumentRange().TextRange);
    }
    #endregion
  }
}