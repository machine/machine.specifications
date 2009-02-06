using JetBrains.CommonControls;
using JetBrains.ReSharper.CodeView.TreePsiBrowser;
using JetBrains.ReSharper.UnitTestExplorer;
using JetBrains.TreeModels;
using JetBrains.UI.TreeView;

namespace Machine.Specifications.ReSharperRunner.Presentation
{
  internal class Presenter : TreeModelBrowserPresenter
  {
    public Presenter()
    {
      Present<ContextElement>(PresentContext);
      Present<SpecificationElement>(PresentSpecification);
      Present<BehaviorElement>(PresentBehavior);
    }

    protected virtual void PresentContext(ContextElement element,
                                          IPresentableItem item,
                                          TreeModelNode modelNode,
                                          PresentationState state)
    {
    }

    protected virtual void PresentSpecification(SpecificationElement element,
                                                IPresentableItem item,
                                                TreeModelNode modelNode,
                                                PresentationState state)
    {
    }

    protected virtual void PresentBehavior(BehaviorElement element,
                                           IPresentableItem item,
                                           TreeModelNode modelNode,
                                           PresentationState state)
    {
    }

    protected override bool IsNaturalParent(object parentValue, object childValue)
    {
      var @namespace = parentValue as UnitTestNamespace;
      var context = childValue as ContextElement;

      if (context != null && @namespace != null)
      {
        return @namespace.Equals(context.GetNamespace());
      }

      return base.IsNaturalParent(parentValue, childValue);
    }

    protected override object Unwrap(object value)
    {
      var specification = value as SpecificationElement;
      if (specification != null)
      {
        value = specification.GetDeclaredElement();
      }

      var context = value as ContextElement;
      if (context != null)
      {
        value = context.GetDeclaredElement();
      }
      return base.Unwrap(value);
    }
  }
}