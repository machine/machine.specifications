using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;

using Machine.Container;

namespace Machine.MsWcf
{
  public class MachineServiceHostFactory : ServiceHostFactory
  {
    #region Logging
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(MachineServiceHostFactory));
    #endregion

    #region MachineServiceHostFactory()
    public MachineServiceHostFactory()
    {
      _log.Info("MachineServiceHostFactory.ctor()");
    }
    #endregion

    #region ServiceHostFactory Members
    protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
    {
      _log.Info("CreateServiceHost: " + serviceType);
      object service = IoC.Container.Resolve(serviceType);
      return new ServiceHost(service, baseAddresses);
    }
    #endregion
  }
}
