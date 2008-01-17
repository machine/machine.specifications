using System;
using System.Collections.Generic;

using Castle.Windsor;

namespace Machine.BackgroundJobs.Services.Impl
{
  public class WindsorJobServicesLocator : AttributeAwareJobServicesLocator
  {
    #region Member Data
    private readonly IWindsorContainer _windsorContainer;
    #endregion

    #region WindsorJobServicesLocator()
    public WindsorJobServicesLocator(IWindsorContainer windsorContainer)
    {
      _windsorContainer = windsorContainer;
    }
    #endregion

    #region AttributeAwareJobServicesLocator
    protected override IJobRepository LocateRepository(Type jobType, BackgroundJobAttribute attribute)
    {
      object resolved = _windsorContainer.Resolve(attribute.RepositoryType);
      IJobRepository jobRepository = resolved as IJobRepository;
      if (jobRepository == null)
      {
        throw new ArgumentException(String.Format("{0} should be an IJobRepository", attribute.RepositoryType));
      }
      return jobRepository;
    }

    protected override IBackgroundJobHandler LocateJobHandler(Type jobType, BackgroundJobAttribute attribute)
    {
      object resolved = _windsorContainer.Resolve(attribute.HandlerType);
      IBackgroundJobHandler backgroundJobHandler = resolved as IBackgroundJobHandler;
      if (backgroundJobHandler == null)
      {
        throw new ArgumentException(String.Format("{0} should be an IBackgroundJobHandler", attribute.HandlerType));
      }
      return backgroundJobHandler;
    }
    #endregion
  }
}
