using System;
using System.Collections.Generic;
using System.Reflection;

namespace Machine.BackgroundJobs.Services.Impl
{
  public abstract class AttributeAwareJobServicesLocator : IJobServicesLocator
  {
    #region IJobServicesLocator Members
    public IJobRepository LocateRepository(Type jobType)
    {
      return LocateRepository(jobType, FindAttribute(jobType));
    }

    public IBackgroundJobHandler LocateJobHandler(Type jobType)
    {
      return LocateJobHandler(jobType, FindAttribute(jobType));
    }
    #endregion

    #region Methods
    protected abstract IJobRepository LocateRepository(Type jobType, BackgroundJobAttribute attribute);

    protected abstract IBackgroundJobHandler LocateJobHandler(Type jobType, BackgroundJobAttribute attribute);

    protected virtual BackgroundJobAttribute FindAttribute(ICustomAttributeProvider type)
    {
      BackgroundJobAttribute[] attributes = (BackgroundJobAttribute[]) type.GetCustomAttributes(typeof (BackgroundJobAttribute), true);
      if (attributes.Length != 1)
      {
        return null;
      }
      return attributes[0];
    }
    #endregion
  }
}
