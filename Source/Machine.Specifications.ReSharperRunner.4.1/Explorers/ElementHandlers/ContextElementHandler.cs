using System.Collections.Generic;

using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
#if RESHARPER_5
using JetBrains.ReSharper.UnitTestFramework;
#else
using JetBrains.ReSharper.UnitTestExplorer;
#endif

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

    public IEnumerable<UnitTestElementDisposition> AcceptElement(IElement element, IFile file)
    {
      IDeclaration declaration = (IDeclaration)element;
      var contextElement = _contextFactory.CreateContext((ITypeElement)declaration.DeclaredElement);

      if (contextElement == null)
      {
        yield break;
      }

      yield return new UnitTestElementDisposition(contextElement,
                                                  file.ProjectFile,
                                                  declaration.GetNavigationRange().TextRange,
                                                  declaration.GetDocumentRange().TextRange);
    }
    #endregion
  }
}