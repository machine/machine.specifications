namespace Machine.Specifications.ReSharperProvider.Factories
{
    using System.Collections.Generic;

    using JetBrains.ProjectModel;
    using JetBrains.ReSharper.Psi;

    using Machine.Specifications.ReSharperProvider.Presentation;

    [SolutionComponent]
    public class ElementCache
    {
        readonly IDictionary<ITypeElement, ContextElement> _contexts;
        readonly IDictionary<IDeclaredElement, BehaviorElement> _behaviors;

        public ElementCache()
        {
            this._contexts = new Dictionary<ITypeElement, ContextElement>();
            this._behaviors = new Dictionary<IDeclaredElement, BehaviorElement>();
        }

        public void AddContext(ITypeElement type, ContextElement context)
        {
            if (!this._contexts.ContainsKey(type))
            {
                this._contexts.Add(type, context);
            }
            else
            {
                this._contexts[type] = context;
            }
        }

        public void AddBehavior(IDeclaredElement type, BehaviorElement behavior)
        {
            if (!this._behaviors.ContainsKey(type))
            {
                this._behaviors.Add(type, behavior);
            }
            else
            {
                this._behaviors[type] = behavior;
            }
        }

        public ContextElement TryGetContext(ITypeElement clazz)
        {
            ContextElement context;
            return this._contexts.TryGetValue(clazz, out context) ? context : null;
        }

        public BehaviorElement TryGetBehavior(IDeclaredElement field)
        {
            BehaviorElement behavior;
            return this._behaviors.TryGetValue(field, out behavior) ? behavior : null;
        }
    }
}