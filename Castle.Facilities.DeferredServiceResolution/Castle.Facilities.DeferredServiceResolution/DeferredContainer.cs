using System;
using System.Collections.Generic;

using Castle.Windsor;
using Castle.Windsor.Installer;

namespace Castle.Facilities.DeferredServiceResolution
{
  public class DeferredContainer : WindsorContainer
  {
    #region DeferredContainer()
    public DeferredContainer()
     : base(new DeferredKernel(), new DefaultComponentInstaller())
    {
    }
    #endregion

    public virtual void AddDeferredFacility()
    {
      AddFacility("DeferredServiceResolutionFacility", new DeferredServiceResolutionFacility());
    }
  }
}
