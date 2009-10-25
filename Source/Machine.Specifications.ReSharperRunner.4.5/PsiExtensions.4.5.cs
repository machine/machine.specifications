using JetBrains.Application.Progress;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Search;

namespace Machine.Specifications.ReSharperRunner
{
  internal static partial class PsiExtensions
  {
    static bool IsInvalid(this IExpressionType type)
    {
      return type == null || !type.IsValid();
    }

    public static bool IsContextBaseClass(this IDeclaredElement element)
    {
      var clazz = element as IClass;
      if (clazz == null)
      {
        return false;
      }

      var contextBaseCandidate = !element.IsContext() &&
                                 clazz.IsValid() &&
                                 clazz.GetContainingType() == null &&
                                 clazz.GetAccessRights() == AccessRights.PUBLIC;
      if (!contextBaseCandidate)
      {
        return false;
      }

      var foundInheritedContexts = false;

      IFinder finder = clazz.GetManager().Finder;
      var searchDomain = clazz.GetSearchDomain();

      finder.FindInheritors(clazz,
                            searchDomain,
                            result =>
                            {
                              FindResultDeclaredElement foundElement = result as FindResultDeclaredElement;
                              if (foundElement != null)
                              {
                                foundInheritedContexts = foundElement.DeclaredElement.IsContext();
                              }

                              return foundInheritedContexts ? FindExecution.Stop : FindExecution.Continue;
                            },
                            NullProgressIndicator.Instance);

      return foundInheritedContexts;
    }
  }
}