using JetBrains.CommonControls;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.TreeModels;
using JetBrains.UI.TreeView;

using Machine.Specifications.ReSharperRunner.Presentation;

namespace Machine.Specifications.ReSharperRunner
{
  [UnitTestPresenter]
  public class MSpecUnitTestPresenter : IUnitTestPresenter 
  {
    Presenter _presenter;

    public MSpecUnitTestPresenter() { _presenter = new Presenter(); }
      
    public void Present(IUnitTestElement element, IPresentableItem item, TreeModelNode node, PresentationState state)
    {
      _presenter.UpdateItem(element, node, item, state);
    }
  }
}