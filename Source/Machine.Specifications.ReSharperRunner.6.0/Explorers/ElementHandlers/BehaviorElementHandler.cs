using System.Collections.Generic;

using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;

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

    public bool Accepts(ITreeNode element)
    {
      IDeclaration declaration = element as IDeclaration;
      if (declaration == null)
      {
        return false;
      }

      return declaration.DeclaredElement.IsBehavior();
    }

    public IEnumerable<UnitTestElementDisposition> AcceptElement(ITreeNode element, IFile file)
    {
      IDeclaration declaration = (IDeclaration)element;
      var behaviorElement = _behaviorFactory.CreateBehavior(declaration.DeclaredElement);
      
      if (behaviorElement == null)
      {
        yield break;
      }

      yield return new UnitTestElementDisposition(behaviorElement,
                                                  file.GetSourceFile().ToProjectFile(),
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

    public void Cleanup(ITreeNode element)
    {
    }
  }
}