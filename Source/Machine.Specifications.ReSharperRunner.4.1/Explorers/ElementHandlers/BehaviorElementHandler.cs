using System.Collections.Generic;

using JetBrains.ReSharper.Psi.Tree;
#if RESHARPER_5 || RESHARPER_6
using JetBrains.ReSharper.UnitTestFramework;
#else
using JetBrains.ReSharper.UnitTestExplorer;
#endif

using Machine.Specifications.ReSharperRunner.Factories;

using JetBrains.ReSharper.Psi;

namespace Machine.Specifications.ReSharperRunner.Explorers.ElementHandlers
{
  internal class BehaviorElementHandler : IElementHandler
  {
    readonly BehaviorFactory _behaviorFactory;
    readonly BehaviorSpecificationFactory _behaviorSpecificationFactory;

    public BehaviorElementHandler(BehaviorFactory behaviorFactory,
                                  BehaviorSpecificationFactory behaviorSpecificationFactory)
    {
      _behaviorFactory = behaviorFactory;
      _behaviorSpecificationFactory = behaviorSpecificationFactory;
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

      return declaration.DeclaredElement.IsBehavior();
    }

#if RESHARPER_6
    public IEnumerable<UnitTestElementDisposition> AcceptElement(ITreeNode element, IFile file)
#else
    public IEnumerable<UnitTestElementDisposition> AcceptElement(IElement element, IFile file)
#endif
    {
      IDeclaration declaration = (IDeclaration)element;
      var behaviorElement = _behaviorFactory.CreateBehavior(declaration.DeclaredElement);

      if (behaviorElement == null)
      {
        yield break;
      }

      yield return new UnitTestElementDisposition(behaviorElement,
#if RESHARPER_6
                                                  file.GetSourceFile().ToProjectFile(),
#else
                                                  file.ProjectFile,
#endif
                                                  declaration.GetNavigationRange().TextRange,
                                                  declaration.GetDocumentRange().TextRange);

      var behaviorSpecifications =
        _behaviorSpecificationFactory.CreateBehaviorSpecificationsFromBehavior(behaviorElement,
                                                                               declaration.DeclaredElement);

      foreach (var behaviorSpecificationElement in behaviorSpecifications)
      {
        yield return new UnitTestElementDisposition(new UnitTestElementLocation[0],
                                                    behaviorSpecificationElement);
      }
    }
  }
}