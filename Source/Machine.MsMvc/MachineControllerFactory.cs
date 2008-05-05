using System;
using System.Web.Mvc;
using System.Web.Routing;

using Machine.Container.Services;

namespace Machine.MsMvc
{
  public class MachineControllerFactory : IControllerFactory
  {
    private readonly IHighLevelContainer _container;
    private readonly IControllerPreparer _controllerPreparer;
    private readonly IControllerTypeMap _controllerTypeMap;

    public MachineControllerFactory(IHighLevelContainer container, IControllerPreparer controllerPreparer, IControllerTypeMap controllerTypeMap)
    {
      _container = container;
      _controllerTypeMap = controllerTypeMap;
      _controllerPreparer = controllerPreparer;
    }

    #region IControllerFactory Members
    public IController CreateController(RequestContext context, string controllerName)
    {
      Type controllerType = _controllerTypeMap.LookupControllerType(controllerName);
      if (controllerType == null)
      {
        throw new ArgumentNullException("Unable to find controller type: " + controllerName);
      }
      IController controller = (IController)_container.Resolve(controllerType);
      return _controllerPreparer.PrepareController(controllerType, controller);
    }

    public void DisposeController(IController controller)
    {
    }
    #endregion
  }
}
