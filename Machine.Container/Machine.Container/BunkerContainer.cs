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
    private IDependencyResolver _dependencyResolver;
    #endregion

    #region Methods
    public virtual void Initialize()
    {
      IDependencyResolver dependencyResolver = CreateDependencyResolver();
      IServiceEntryFactory entryFactory = new ServiceEntryFactory();
      IServiceDependencyInspector inspector = new ServiceDependencyInspector();
      IServiceGraph serviceGraph = new ServiceGraph();
      _resolver = new ServiceEntryResolver(serviceGraph, entryFactory, dependencyResolver);
      _dependencyResolver = dependencyResolver;
      _activatorStrategy = new DefaultActivatorStrategy(new DotNetObjectFactory(), _resolver, inspector);
      _activatorStore = new ActivatorStore(_activatorStrategy, new LifestyleFactory(_activatorStrategy));
    }

    public virtual IDependencyResolver CreateDependencyResolver()
    {
      return new RootDependencyResolver(new OverridableDependencyResolver(), new ActivatorLookupDependencyResolver(), new ThrowingDependencyResolver());
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
      ICreationServices services = new CreationServices(_activatorStrategy, _activatorStore, overrides);
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
      ICreationServices services = new CreationServices(_activatorStrategy, _activatorStore, new EmptyOverrides());
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
