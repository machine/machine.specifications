using System.Collections.Generic;

using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;

using Machine.Specifications.ReSharperRunner.Factories;

namespace Machine.Specifications.ReSharperRunner.Explorers.ElementHandlers
{
  class ContextSpecificationElementHandler : IElementHandler
  {
    readonly ContextSpecificationFactory _factory;

    public ContextSpecificationElementHandler(ElementFactories factories)
    {
      _factory = factories.ContextSpecifications;
    }

    public bool Accepts(ITreeNode element)
    {
      var declaration = element as IDeclaration;
      if (declaration == null)
      {
        return false;
      }

      return declaration.DeclaredElement.IsSpecification();
    }

    public IEnumerable<UnitTestElementDisposition> AcceptElement(string assemblyPath, IFile file, ITreeNode element)
    {
      var declaration = (IDeclaration) element;
      var contextSpecification = _factory.CreateContextSpecification(declaration.DeclaredElement);

      if (contextSpecification == null)
      {
        yield break;
      }

      yield return new UnitTestElementDisposition(contextSpecification,
                                                  file.GetSourceFile().ToProjectFile(),
                                                  declaration.GetNavigationRange().TextRange,
                                                  declaration.GetDocumentRange().TextRange);
    }

    public void Cleanup(ITreeNode element)
    {
    }
  }
}