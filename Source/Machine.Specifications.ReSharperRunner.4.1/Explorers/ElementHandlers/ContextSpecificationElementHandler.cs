using System.Collections.Generic;

using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
#if RESHARPER_5 || RESHARPER_6
using JetBrains.ReSharper.UnitTestFramework;
#else
using JetBrains.ReSharper.UnitTestExplorer;
#endif

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

#if RESHARPER_6
    public bool Accepts(ITreeNode element)
#else
    public bool Accepts(IElement element)
#endif
    {
      IDeclaration declaration = element as IDeclaration;
      if (declaration == null)
      {
        return false;
      }

      return declaration.DeclaredElement.IsSpecification();
    }

#if RESHARPER_6
    public IEnumerable<UnitTestElementDisposition> AcceptElement(ITreeNode element, IFile file)
#else
    public IEnumerable<UnitTestElementDisposition> AcceptElement(IElement element, IFile file)
#endif 
    {
      IDeclaration declaration = (IDeclaration)element;
      var contextSpecificationElement =
        _contextSpecificationFactory.CreateContextSpecification(declaration.DeclaredElement);

      if (contextSpecificationElement == null)
      {
        yield break;
      }

      yield return new UnitTestElementDisposition(contextSpecificationElement,
#if RESHARPER_6
                                                  file.GetSourceFile().ToProjectFile(),
#else
                                                  file.ProjectFile,
#endif
                                                  declaration.GetNavigationRange().TextRange,
                                                  declaration.GetDocumentRange().TextRange);
    }
  }
}