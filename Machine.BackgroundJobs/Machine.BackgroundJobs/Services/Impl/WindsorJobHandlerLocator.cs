using System;

using Castle.Windsor;

namespace Machine.BackgroundJobs.Services.Impl
{
  public class WindsorJobHandlerLocator : AttributeAwareJobServicesLocator, IJobHandlerLocator
  {
    #region WindsorJobHandlerLocator()
    public WindsorJobHandlerLocator(IWindsorContainer windsorContainer)
      : base(windsorContainer)
    {
    }
    #endregion

    #region IJobHandlerLocator Members
    public IBackgroundJobHandler LocateJobHandler(Type jobType)
    {
      BackgroundJobAttribute attribute = FindAttribute(jobType);
      return Resolve(attribute.HandlerType);
    }
    #endregion
  }
}