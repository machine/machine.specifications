using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

using Machine.Container;
using Machine.Container.Services;

namespace Machine.MsMvc
{
  public class MachineControllerFactory : IControllerFactory
  {
    private readonly IHighLevelContainer _container;
    private readonly IControllerPreparer _controllerPreparer;

    public MachineControllerFactory(IHighLevelContainer container, IControllerPreparer controllerPreparer)
    {
      _container = container;
      _controllerPreparer = controllerPreparer;
    }

    #region IControllerFactory Members
    public IController CreateController(RequestContext context, string controllerName)
    {
      string name = "CardQueue.Server.Controllers." + controllerName + "Controller, CardQueue.Server";
      Type type = Type.GetType(name);
      if (type == null)
      {
        throw new ArgumentNullException("Unable to find controller type: " + controllerName);
      }
      IController controller = (IController)_container.Resolve(type);
      return _controllerPreparer.PrepareController(controller);
      /*
      Controller hardController = controller as Controller;
      if (hardController != null)
      {
        hardController.ViewEngine = new NHamlViewFactory();
      }
      */
    }

    public void DisposeController(IController controller)
    {
    }
    #endregion
  }
}
