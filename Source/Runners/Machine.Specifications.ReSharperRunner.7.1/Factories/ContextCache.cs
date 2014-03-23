using System.Collections.Generic;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;

using Machine.Specifications.ReSharperRunner.Presentation;

namespace Machine.Specifications.ReSharperRunner.Factories
{
  [SolutionComponent]
  public class ElementCache
  {
    readonly IDictionary<ITypeElement, ContextElement> _contexts;
    readonly IDictionary<IDeclaredElement, BehaviorElement> _behaviors;

    public ElementCache()
    {
      _contexts = new Dictionary<ITypeElement, ContextElement>();
      _behaviors = new Dictionary<IDeclaredElement, BehaviorElement>();
    }

    public void AddContext(ITypeElement type, ContextElement context)
    {
      if (!_contexts.ContainsKey(type))
      {
        _contexts.Add(type, context);
      }
      else
      {
        _contexts[type] = context;
      }
    }

    public void AddBehavior(IDeclaredElement type, BehaviorElement behavior)
    {
      if (!_behaviors.ContainsKey(type))
      {
        _behaviors.Add(type, behavior);
      }
      else
      {
        _behaviors[type] = behavior;
      }
    }

    public ContextElement TryGetContext(ITypeElement clazz)
    {
      ContextElement context;
      return _contexts.TryGetValue(clazz, out context) ? context : null;
    }

    public BehaviorElement TryGetBehavior(IDeclaredElement field)
    {
      BehaviorElement behavior;
      return _behaviors.TryGetValue(field, out behavior) ? behavior : null;
    }
  }
}