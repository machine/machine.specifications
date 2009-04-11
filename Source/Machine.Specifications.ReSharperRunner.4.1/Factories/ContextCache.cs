using JetBrains.ReSharper.Psi;
using JetBrains.Util.DataStructures;

using Machine.Specifications.ReSharperRunner.Presentation;

namespace Machine.Specifications.ReSharperRunner.Factories
{
  internal class ContextCache
  {
    public ContextCache()
    {
      Classes = new Dictionary2<ITypeElement, ContextElement>();
    }

    public Dictionary2<ITypeElement, ContextElement> Classes
    {
      get;
      private set;
    }
  }
}
