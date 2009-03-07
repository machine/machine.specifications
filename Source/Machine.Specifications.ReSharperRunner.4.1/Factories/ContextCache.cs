using JetBrains.ReSharper.Psi;
using JetBrains.Util.DataStructures;

using Machine.Specifications.ReSharperRunner.Presentation;

namespace Machine.Specifications.ReSharperRunner.Factories
{
  internal static class ContextCache
  {
    public static Dictionary2<ITypeElement, ContextElement> Classes = new Dictionary2<ITypeElement, ContextElement>();
  }
}