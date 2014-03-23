using System.Collections.Generic;

using JetBrains.ProjectModel;
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

    public BehaviorElementHandler(ElementFactories factories)
    {
      _factory = factories.Behaviors;
      _behaviorSpecifications = factories.BehaviorSpecifications;
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

    public IEnumerable<UnitTestElementDisposition> AcceptElement(string assemblyPath, IFile file, ITreeNode element)
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

        var projectFile = GetProjectFile(field);
        if (projectFile != null)
        {
          yield return new UnitTestElementDisposition(behaviorSpecification,
                                                      projectFile,
                                                      new TextRange(),
                                                      GetTextRange(field));
        }
        else
        {
          yield return new UnitTestElementDisposition(new UnitTestElementLocation[] {}, behaviorSpecification);
        }
      }
    }

    static IProjectFile GetProjectFile(IDeclaredElement field)
    {
      var sourceFile = field.GetSourceFiles();
      if (sourceFile.Count > 0)
      {
        return sourceFile[0].ToProjectFile();
      }
      return null;
    }

    static TextRange GetTextRange(IDeclaredElement field)
    {
      var declarations = field.GetDeclarations();
      if (declarations.Count > 0)
      {
        return declarations[0].GetDocumentRange().TextRange;
      }

      return new TextRange();
    }

    public void Cleanup(ITreeNode element)
    {
      var declaration = (IDeclaration) element;
      _factory.UpdateChildState(declaration.DeclaredElement);
    }
  }
}