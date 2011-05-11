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

#if RESHARPER_6
      IFinder finder = clazz.GetSolution().GetPsiServices().Finder;
#else
      IFinder finder = clazz.GetManager().Finder;
#endif
      var searchDomain = clazz.GetSearchDomain();

      var findResult = new InheritedContextFinder();

      finder.FindInheritors(clazz,
                            searchDomain,
                            findResult.Consumer,
                            NullProgressIndicator.Instance);

      return findResult.Found;
    }

    class InheritedContextFinder
    {
      public InheritedContextFinder()
      {
        Found = false;

        Consumer = new FindResultConsumer(result =>
        {
          FindResultDeclaredElement foundElement = result as FindResultDeclaredElement;
          if (foundElement != null)
          {
            Found = foundElement.DeclaredElement.IsContext();
          }

          return Found ? FindExecution.Stop : FindExecution.Continue;
        });
      }

      public bool Found
      {
        get;
        private set;
      }

      public FindResultConsumer Consumer
      {
        get;
        private set;
      }
    }
  }
}