using System.Collections.Generic;

using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;

using Machine.Specifications.ReSharperRunner.Factories;

namespace Machine.Specifications.ReSharperRunner.Explorers.ElementHandlers
{
  class BehaviorElementHandler : IElementHandler
  {
    readonly BehaviorFactory _factory;
    readonly BehaviorSpecificationFactory _behaviorSpecifications;

    public BehaviorElementHandler(BehaviorFactory behaviorFactory, BehaviorSpecificationFactory behaviorSpecificationFactory)
    {
      _factory = behaviorFactory;
      _behaviorSpecifications = behaviorSpecificationFactory;
    }

    public bool Accepts(ITreeNode element)
    {
      var declaration = element as IDeclaration;
      if (declaration == null)
      {
        return false;
      }

      return declaration.DeclaredElement.IsBehavior();
    }

    public IEnumerable<UnitTestElementDisposition> AcceptElement(ITreeNode element, IFile file)
    {
      var declaration = (IDeclaration) element;
      var behavior = _factory.CreateBehavior(declaration.DeclaredElement);

      if (behavior == null)
      {
        yield break;
      }

      yield return new UnitTestElementDisposition(behavior,
                                                  file.GetSourceFile().ToProjectFile(),
                                                  declaration.GetNavigationRange().TextRange,
                                                  declaration.GetDocumentRange().TextRange);

      var behaviorContainer = declaration.DeclaredElement.GetFirstGenericArgument();
      if (!behaviorContainer.IsBehaviorContainer())
      {
        yield break;
      }

      foreach (var field in behaviorContainer.Fields)
      {
        if (!field.IsSpecification())
        {
          continue;
        }

        var behaviorSpecification = _behaviorSpecifications.CreateBehaviorSpecification(behavior, field);

        yield return new UnitTestElementDisposition(behaviorSpecification,
                                                    field.GetSourceFiles()[0].ToProjectFile(),
                                                    new TextRange(),
                                                    field.GetDeclarations()[0].GetDocumentRange().TextRange);
      }
    }

    public void Cleanup(ITreeNode element)
    {
      var declaration = (IDeclaration) element;
      _factory.UpdateChildState(declaration.DeclaredElement);
    }
  }
}