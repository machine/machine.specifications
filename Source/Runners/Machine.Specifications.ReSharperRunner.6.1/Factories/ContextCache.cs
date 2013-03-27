using System.Collections.Generic;

using JetBrains.ReSharper.Psi;

using Machine.Specifications.ReSharperRunner.Presentation;

namespace Machine.Specifications.ReSharperRunner.Factories
{
  public class ElementCache
  {
    public ElementCache()
    {
      Contexts = new Dictionary<ITypeElement, ContextElement>();
      Behaviors = new Dictionary<IDeclaredElement, BehaviorElement>();
    }

    public IDictionary<ITypeElement, ContextElement> Contexts
    {
      get;
      private set;
    }

    public IDictionary<IDeclaredElement, BehaviorElement> Behaviors
    {
      get;
      private set;
    }
  }
}