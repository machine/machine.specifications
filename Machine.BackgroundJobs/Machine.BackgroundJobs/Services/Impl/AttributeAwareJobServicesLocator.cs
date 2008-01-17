using System;
using System.Collections.Generic;
using System.Reflection;

namespace Machine.BackgroundJobs.Services.Impl
{
  public abstract class AttributeAwareJobServicesLocator : IJobServicesLocator
  {
    #region IJobServicesLocator Members
    public IJobRepository LocateRepository(IBackgroundJob job)
    {
      return LocateRepository(job, FindAttribute(job.GetType()));
    }

    public IBackgroundJobHandler LocateJobHandler(IBackgroundJob job)
    {
      return LocateJobHandler(job, FindAttribute(job.GetType()));
    }
    #endregion

    #region Methods
    protected abstract IJobRepository LocateRepository(IBackgroundJob job, BackgroundJobAttribute attribute);

    protected abstract IBackgroundJobHandler LocateJobHandler(IBackgroundJob job, BackgroundJobAttribute attribute);

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
