using System;
using System.Collections.Generic;

using Machine.Container.Model;

namespace Machine.Container.Services
{
  public interface IHighLevelContainer : IDisposable
  {
    void AddService(Type serviceType, LifestyleType lifestyleType);
    void AddService<TService>(Type implementation);
    void AddService<TService, TImpl>(LifestyleType lifestyleType);
    void AddService<TService>(LifestyleType lifestyleType);
    void Add<TService>(object instance);
    T Resolve<T>();
    T ResolveWithOverrides<T>(params object[] serviceOverrides);
    T New<T>(params object[] serviceOverrides);
    bool HasService<T>();
  }
}