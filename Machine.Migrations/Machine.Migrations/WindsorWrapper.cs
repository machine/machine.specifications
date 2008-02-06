using System;
using Castle.Windsor;

namespace Machine.Migrations
{
  public class WindsorWrapper
  {
    private readonly IWindsorContainer _container;

    public WindsorWrapper(IWindsorContainer container)
    {
      _container = container;
    }

    public void AddService<TService>(Type implementation)
    {
      _container.AddComponent(MakeKey(implementation), typeof(TService), implementation);
    }

    public void AddService<TService, TImpl>() where TImpl : TService
    {
      AddService<TService>(typeof(TImpl));
    }

    public void AddService<TService>(TService implementation)
    {
      _container.Kernel.AddComponentInstance(MakeKey(implementation.GetType()), typeof(TService), implementation);
    }

    public void AddService<TImpl>()
    {
      _container.AddComponent(MakeKey(typeof(TImpl)), typeof(TImpl));
    }

    public void RemoveService<TService>()
    {
      _container.Kernel.RemoveComponent(MakeKey(typeof(TService)));
    }

    public bool HasService<TService>()
    {
      return _container.Kernel.HasComponent(MakeKey(typeof(TService)));
    }

    public TService Resolve<TService>()
    {
      return _container.Resolve<TService>();
    }

    private static string MakeKey(Type implementation)
    {
      return implementation.FullName;
    }
  }
}