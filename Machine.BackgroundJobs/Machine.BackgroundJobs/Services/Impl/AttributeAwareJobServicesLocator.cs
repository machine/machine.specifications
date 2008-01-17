using System;
using System.Collections.Generic;
using System.Reflection;

using Castle.Windsor;

namespace Machine.BackgroundJobs.Services.Impl
{
  public abstract class AttributeAwareJobServicesLocator
  {
    #region Member Data
    private readonly IWindsorContainer _windsorContainer;
    #endregion

    #region AttributeAwareJobServicesLocator()
    protected AttributeAwareJobServicesLocator(IWindsorContainer windsorContainer)
    {
      _windsorContainer = windsorContainer;
    }
    #endregion

    #region Methods
    protected virtual BackgroundJobAttribute FindAttribute(ICustomAttributeProvider type)
    {
      BackgroundJobAttribute[] attributes = (BackgroundJobAttribute[]) type.GetCustomAttributes(typeof (BackgroundJobAttribute), true);
      if (attributes.Length != 1)
      {
        return null;
      }
      return attributes[0];
    }

    protected virtual IBackgroundJobHandler Resolve(Type type)
    {
      object resolved = _windsorContainer.Resolve(type);
      IBackgroundJobHandler service = resolved as IBackgroundJobHandler;
      if (service == null)
      {
        throw new ArgumentException(String.Format("{0} should be an {1}", type, typeof(IBackgroundJobHandler)));
      }
      return service;
    }
    #endregion
  }
}
