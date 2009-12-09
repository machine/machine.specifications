using System.Collections.Generic;

using JetBrains.ReSharper.Psi;

using Machine.Specifications.ReSharperRunner.Presentation;

namespace Machine.Specifications.ReSharperRunner.Factories
{
  internal class ContextCache
  {
    public ContextCache()
    {
      Classes = new Dictionary<ITypeElement, ContextElement>();
    }

    public IDictionary<ITypeElement, ContextElement> Classes
    {
      get;
      private set;
    }
  }
}
