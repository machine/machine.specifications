using System.Drawing;

using JetBrains.CommonControls;
using JetBrains.ReSharper.Features.Common.TreePsiBrowser;
#if RESHARPER_5
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.UI;
#else
using JetBrains.ReSharper.UnitTestExplorer;
#endif
#if RESHARPER_6
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.UI;
#endif
using JetBrains.TreeModels;
using JetBrains.UI.TreeView;

namespace Machine.Specifications.ReSharperRunner.Presentation
{
  internal class Presenter : TreeModelBrowserPresenter
  {
    public Presenter()
    {
      Present<ContextElement>(PresentContext);
      Present<FieldElement>(PresentSpecification);
      Present<BehaviorElement>(PresentBehavior);
      Present<BehaviorSpecificationElement>(PresentBehaviorSpecification);
    }

    protected virtual void PresentContext(ContextElement element,
                                          IPresentableItem item,
                                          TreeModelNode modelNode,
                                          PresentationState state)
    {
      PresentItem(item, element, state, UnitTestElementImage.TestContainer);
    }

    protected virtual void PresentSpecification(FieldElement element,
                                                IPresentableItem item,
                                                TreeModelNode modelNode,
                                                PresentationState state)
    {
      PresentItem(item, element, state, UnitTestElementImage.Test);
    }

    protected virtual void PresentBehavior(BehaviorElement element,
                                           IPresentableItem item,
                                           TreeModelNode modelNode,
                                           PresentationState state)
    {
      PresentItem(item, element, state, UnitTestElementImage.TestContainer);
    }

    protected virtual void PresentBehaviorSpecification(BehaviorSpecificationElement element,
                                                        IPresentableItem item,
                                                        TreeModelNode modelNode,
                                                        PresentationState state)
    {
      PresentItem(item, element, state, UnitTestElementImage.Test);
    }

    static void PresentItem(IPresentableItem item, Element element, PresentationState state, UnitTestElementImage type)
    {
      item.RichText = element.GetTitle();

      SetTextColor(item, element);
      SetImage(item, state, type);
    }

    static void SetTextColor(IPresentableItem item, Element element)
    {
      if (element.IsExplicit)
      {
        item.RichText.SetForeColor(SystemColors.GrayText);
      }

      item.RichText.SetForeColor(SystemColors.GrayText, 0, element.GetTitlePrefix().Length);
    }

    static void SetImage(IPresentableItem item, PresentationState state, UnitTestElementImage imageType)
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
      var specification = value as FieldElement;
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