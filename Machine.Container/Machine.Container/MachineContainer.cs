using System;
using System.Collections.Generic;

using Machine.Container.Model;
using Machine.Container.Services;
using Machine.Container.Services.Impl;

namespace Machine.Container
{
  public class MachineContainer : IHighLevelContainer
  {
    #region Member Data
    private IServiceEntryResolver _resolver;
    private IActivatorStrategy _activatorStrategy;
    private IActivatorStore _activatorStore;
    private ILifestyleFactory _lifestyleFactory;
    #endregion

    #region Methods
    public virtual void Initialize()
    {
      IActivatorResolver activatorResolver = CreateDependencyResolver();
      IServiceEntryFactory serviceEntryFactory = new ServiceEntryFactory();
      IServiceDependencyInspector serviceDependencyInspector = new ServiceDependencyInspector();
      IServiceGraph serviceGraph = new ServiceGraph();
      _resolver = new ServiceEntryResolver(serviceGraph, serviceEntryFactory, activatorResolver);
      _activatorStrategy = new DefaultActivatorStrategy(new DotNetObjectFactory(), _resolver, serviceDependencyInspector);
      _activatorStore = new ActivatorStore();
      _lifestyleFactory = new LifestyleFactory(_activatorStrategy);
    }

    public virtual IActivatorResolver CreateDependencyResolver()
    {
      return new RootActivatorResolver(new StaticLookupActivatorResolver(), new DefaultLifestyleAwareActivatorResolver(), new ThrowsPendingActivatorResolver());
    }
    #endregion

    #region IHighLevelContainer Members
    public void AddService<TService>()
    {
      AddService(typeof(TService), LifestyleType.Singleton);
    }

    public void AddService(Type serviceType, LifestyleType lifestyleType)
    {
      ServiceEntry entry = _resolver.CreateEntryIfMissing(serviceType);
      entry.LifestyleType = lifestyleType;
    }

    public void AddService<TService>(Type implementationType)
    {
      ServiceEntry entry = _resolver.CreateEntryIfMissing(typeof(TService), implementationType);
    }

    public void AddService<TService, TImpl>(LifestyleType lifestyleType)
    {
      ServiceEntry entry = _resolver.CreateEntryIfMissing(typeof(TService), typeof(TImpl));
      entry.LifestyleType = lifestyleType;
    }

    public void AddService<TService>(LifestyleType lifestyleType)
    {
      ServiceEntry entry = _resolver.CreateEntryIfMissing(typeof(TService));
      entry.LifestyleType = lifestyleType;
    }

    public void Add<TService>(object instance)
    {
      ServiceEntry entry = _resolver.CreateEntryIfMissing(typeof(TService));
      IActivator activator = _activatorStrategy.CreateStaticActivator(entry, instance);
      _activatorStore.AddActivator(entry, activator);
    }

    public T Resolve<T>()
    {
      return ResolveWithOverrides<T>();
    }

    public T New<T>(params object[] serviceOverrides)
    {
      AddService<T>(LifestyleType.Transient);
      return ResolveWithOverrides<T>(serviceOverrides);
    }

    public T ResolveWithOverrides<T>(params object[] serviceOverrides)
    {
      ICreationServices services = CreateCreationServices(serviceOverrides);
      ResolvedServiceEntry entry = _resolver.ResolveEntry(services, typeof(T), true);
      return (T)entry.Activator.Activate(services);
    }

    public bool HasService<T>()
    {
      ServiceEntry entry = _resolver.LookupEntry(typeof(T));
      return entry != null;
    }
    #endregion

    #region IDisposable Members
    public void Dispose()
    {
    }
    #endregion

    #region Methods
    protected virtual ICreationServices CreateCreationServices(params object[] serviceOverrides)
    {
      IOverrideLookup overrides = new StaticOverrideLookup(serviceOverrides);
      return new CreationServices(_activatorStrategy, _activatorStore, _lifestyleFactory, overrides);
    }
    #endregion
  }
}
