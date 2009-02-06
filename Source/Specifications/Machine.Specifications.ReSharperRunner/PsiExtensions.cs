using System.Linq;

using JetBrains.ReSharper.Psi;

namespace Machine.Specifications.ReSharperRunner
{
  internal static class PsiExtensions
  {
    public static bool IsContext(this IDeclaredElement element)
    {
      var clazz = element as IClass;
      if (clazz == null)
      {
        return false;
      }

      return !clazz.IsAbstract &&
             clazz.GetAccessRights() == AccessRights.PUBLIC &&
             clazz.GetMembers().Any(x => IsSpecification(x) || IsBehavior(x));
    }

    public static bool IsSpecification(this IDeclaredElement element)
    {
      var field = element as IField;
      if (field == null)
      {
        return false;
      }

      // HACK: String comparison.
      return field.IsValid() && field.Type.ToString() == typeof(It).FullName;
    }

    public static bool IsBehavior(this IDeclaredElement element)
    {
      var field = element as IField;
      if (field == null)
      {
        return false;
      }

      // HACK: String comparison.
      return field.IsValid() && field.Type.ToString().StartsWith(typeof(Behaves_like<>).FullName);
    }
  }
}