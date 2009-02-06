using System.Drawing;

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
      item.RichText = element.GetTitle();

      SetStateTextColor(item, element);
      SetStateImage(item, state, UnitTestElementImage.TestContainer);
      AppendOccurencesCount(item, modelNode, "test");
    }

    protected virtual void PresentSpecification(SpecificationElement element,
                                                IPresentableItem item,
                                                TreeModelNode modelNode,
                                                PresentationState state)
    {
      item.RichText = element.GetTitle();

      SetStateTextColor(item, element);
      SetStateImage(item, state, UnitTestElementImage.Test);
    }

    protected virtual void PresentBehavior(BehaviorElement element,
                                           IPresentableItem item,
                                           TreeModelNode modelNode,
                                           PresentationState state)
    {
    }

    static void SetStateTextColor(IPresentableItem item, UnitTestElement element)
    {
      if (element.IsExplicit)
      {
        item.RichText.SetForeColor(SystemColors.GrayText);
      }
    }

    static void SetStateImage(IPresentableItem item, PresentationState state, UnitTestElementImage imageType)
    {
      Image stateImage = UnitTestManager.GetStateImage(state);
      Image typeImage = UnitTestManager.GetStandardImage(imageType);

      if (stateImage != null)
      {
        item.Images.Add(stateImage);
      }
      else if (typeImage != null)
      {
        item.Images.Add(typeImage);
      }
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