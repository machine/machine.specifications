using System;
using System.Collections.Generic;

using Machine.Container.Lifestyles;
using Machine.Container.Model;

namespace Machine.Container.Services.Impl
{
  public class LifestyleFactory : ILifestyleFactory
  {
    #region Member Data
    private readonly IActivatorStrategy _activatorStrategy;
    #endregion

    #region LifestyleFactory()
    public LifestyleFactory(IActivatorStrategy activatorStrategy)
    {
      _activatorStrategy = activatorStrategy;
    }
    #endregion

    #region ILifestyleFactory Members
    public ILifestyle CreateSingletonLifestyle(ServiceEntry entry)
    {
      ILifestyle lifestyle = new SingletonLifestyle(_activatorStrategy, entry);
      lifestyle.Initialize();
      return lifestyle;
    }

    public ILifestyle CreateTransientLifestyle(ServiceEntry entry)
    {
      ILifestyle lifestyle = new TransientLifestyle(_activatorStrategy, entry);
      lifestyle.Initialize();
      return lifestyle;
    }

    public ILifestyle CreateLifestyle(ServiceEntry entry)
    {
      switch (entry.LifestyleType)
      {
        case LifestyleType.Singleton:
          return CreateSingletonLifestyle(entry);
        case LifestyleType.Transient:
          return CreateTransientLifestyle(entry);
      }
      throw new ArgumentException("entry");
    }
    #endregion
  }
}
