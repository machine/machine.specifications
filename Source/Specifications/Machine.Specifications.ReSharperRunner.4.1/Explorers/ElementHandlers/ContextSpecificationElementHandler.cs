using System.Collections.Generic;

using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestExplorer;

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

    public IEnumerable<UnitTestElementDisposition> AcceptElement(IElement element, IFile file)
    {
      IDeclaration declaration = (IDeclaration)element;
      var contextSpecificationElement =
        _contextSpecificationFactory.CreateContextSpecification(declaration.DeclaredElement);

      if (contextSpecificationElement == null)
      {
        yield break;
      }

      yield return new UnitTestElementDisposition(contextSpecificationElement,
                                                  file.ProjectFile,
                                                  declaration.GetNameRange(),
                                                  declaration.GetDocumentRange().TextRange);
    }
    #endregion
  }
}