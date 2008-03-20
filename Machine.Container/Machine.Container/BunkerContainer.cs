using System;
using System.Collections.Generic;

using Machine.Container.Model;
using Machine.Container.Services;
using Machine.Container.Services.Impl;

namespace Machine.Container
{
  public class BunkerContainer : IHighLevelContainer
  {
    #region Member Data
    private IServiceEntryResolver _resolver;
    private IActivatorStrategy _activatorStrategy;
    private IActivatorStore _activatorStore;
    private ILifestyleStore _lifestyleStore;
    private IActivatorResolver _activatorResolver;
    #endregion

    #region Methods
    public virtual void Initialize()
    {
      IActivatorResolver activatorResolver = CreateDependencyResolver();
      IServiceEntryFactory entryFactory = new ServiceEntryFactory();
      IServiceDependencyInspector inspector = new ServiceDependencyInspector();
      IServiceGraph serviceGraph = new ServiceGraph();
      _resolver = new ServiceEntryResolver(serviceGraph, entryFactory, activatorResolver);
      _activatorResolver = activatorResolver;
      _activatorStrategy = new DefaultActivatorStrategy(new DotNetObjectFactory(), _resolver, inspector);
      ILifestyleFactory lifestyleFactory = new LifestyleFactory(_activatorStrategy);
      _activatorStore = new ActivatorStore(_activatorStrategy, lifestyleFactory);
      _lifestyleStore = new LifestyleStore(lifestyleFactory);
    }

    public virtual IActivatorResolver CreateDependencyResolver()
    {
      return new RootActivatorResolver(new StaticLookupActivatorResolver(), new ActivatorLookupDependencyResolver(), new ThrowsPendingActivatorResolver());
    }
    #endregion

    #region IHighLevelContainer Members
    public void AddService<TService>(Type implementationType)
    {
      _resolver.CreateEntryIfMissing(typeof(TService), implementationType);
    }

    public void AddService<TService, TImpl>(LifestyleType lifestyleType)
    {
      _resolver.CreateEntryIfMissing(typeof(TService), typeof(TImpl)).LifestyleType = lifestyleType;
    }

    public void AddService<TService>(LifestyleType lifestyleType)
    {
      _resolver.CreateEntryIfMissing(typeof(TService)).LifestyleType = lifestyleType;
    }

    public void Add<TService>(object instance)
    {
      ServiceEntry entry = _resolver.CreateEntryIfMissing(typeof(TService));
      _activatorStore.AddActivator(entry, _activatorStrategy.CreateStaticActivator(entry, instance));
      entry.AreDependenciesResolved = true;
    }

    public T Resolve<T>()
    {
      return ResolveWithOverrides<T>();
    }

    public T ResolveWithOverrides<T>(params object[] serviceOverrides)
    {
      IOverrideLookup overrides = new StaticOverrideLookup(serviceOverrides);
      ICreationServices services = new CreationServices(_activatorStore, _lifestyleStore, _activatorStrategy, overrides);
      ServiceEntry entry = _resolver.ResolveEntry(services, typeof(T));
      return (T)entry.Activator.Create(services);
    }

    public T New<T>(params object[] serviceOverrides)
    {
      _resolver.CreateEntryIfMissing(typeof(T)).LifestyleType = LifestyleType.Transient;
      return ResolveWithOverrides<T>(serviceOverrides);
    }

    public bool HasService<T>()
    {
      ICreationServices services = new CreationServices(_activatorStore, _lifestyleStore, _activatorStrategy, new EmptyOverrides());
      return _resolver.ResolveEntry(services, typeof(T)).AreDependenciesResolved;
    }
    #endregion

    #region IDisposable Members
    public void Dispose()
    {
    }
    #endregion
  }
}
