using System;
using System.Collections.Generic;

namespace Machine.MsMvc
{
  public interface IControllerTypeMap
  {
    Type LookupControllerType(string controllerName);
  }
}
