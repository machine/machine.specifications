using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Machine.MsMvc
{
  public class NullControllerPreparer : IControllerPreparer
  {
    #region IControllerPreparer Members
    public IController PrepareController(IController controller)
    {
      return controller;
    }
    #endregion
  }
}
