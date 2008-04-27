using System;
using System.Collections.Generic;
using Castle.Core;
using Castle.Windsor;

using Machine.Container.Services;

namespace Machine.WindsorExtensions
{
  public class MachineToWindsorBridge : IHighLevelContainer
  {
    private readonly IWindsorContainer _windsor;
    private readonly WindsorWrapper _wrapper;

    public MachineToWindsorBridge(IWindsorContainer windsor)
    {
      _windsor = windsor;
      _wrapper = new WindsorWrapper(_windsor);
    }

    #region IHighLevelContainer Members
    public void AddService(Type serviceType, Machine.Container.Model.LifestyleType lifestyleType)
    {
      _wrapper.AddService(serviceType, Convert(lifestyleType));
    }

    public void AddService<TService>()
    {
      _wrapper.AddService<TService>();
    }

    public void AddService<TService>(Type implementation)
    {
      _wrapper.AddService<TService>(implementation);
    }

    public void AddService<TService, TImpl>(Machine.Container.Model.LifestyleType lifestyleType)
    {
      _wrapper.AddService<TService, TImpl>(Convert(lifestyleType));
    }

    public void AddService<TService, TImpl>()
    {
      _wrapper.AddService<TService, TImpl>();
    }

    public void AddService<TService>(Machine.Container.Model.LifestyleType lifestyleType)
    {
      _wrapper.AddService(typeof(TService), Convert(lifestyleType));
    }

    public void AddService<TService>(object instance)
    {
      _wrapper.AddService<TService>(instance);
    }

    public T Resolve<T>()
    {
      return _windsor.Resolve<T>();
    }

    public object Resolve(Type type)
    {
      return _windsor.Resolve(type);
    }

    public T ResolveWithOverrides<T>(params object[] serviceOverrides)
    {
      throw new NotImplementedException();
    }

    public T New<T>(params object[] serviceOverrides)
    {
      throw new NotImplementedException();
    }

    public bool HasService<T>()
    {
      return _windsor.Kernel.HasComponent(typeof(T));
    }

    public IEnumerable<Machine.Container.Model.ServiceRegistration> RegisteredServices
    {
      get { throw new NotImplementedException(); }
    }

    public void Initialize()
    {
    }
    #endregion

    #region IDisposable Members
    public void Dispose()
    {
      _windsor.Dispose();
    }
    #endregion

    private LifestyleType Convert(Machine.Container.Model.LifestyleType lifestyleType)
    {
      switch (lifestyleType)
      {
        case Machine.Container.Model.LifestyleType.Singleton:
          return LifestyleType.Singleton;
        case Machine.Container.Model.LifestyleType.Transient:
          return LifestyleType.Transient;
      }
      throw new ArgumentException("lifestyleType");
    }
  }
}
