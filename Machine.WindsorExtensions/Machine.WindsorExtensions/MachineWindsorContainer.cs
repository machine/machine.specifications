using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Castle.MicroKernel;
using Castle.Windsor;
using Castle.Windsor.Configuration;

namespace Machine.WindsorExtensions
{
  public class MachineWindsorContainer : WindsorContainer
  {
    private long _counter;

    public MachineWindsorContainer(IConfigurationInterpreter interpreter)
     : base(interpreter)
    {
    }

    public MachineWindsorContainer()
    {
    }

    public virtual void AddFacility(IFacility facility)
    {
      AddFacility(facility.GetType().Name, facility);
    }

    public void AddService<TService>(Type implementation)
    {
      AddComponent(MakeKey(implementation), typeof(TService), implementation);
    }

    public void AddService<TService, TImpl>() where TImpl : TService
    {
      AddService<TService>(typeof(TImpl));
    }

    public void AddService<TService>(TService implementation)
    {
      Kernel.AddComponentInstance(MakeKey(implementation.GetType()), typeof(TService), implementation);
    }

    public void AddService<TImpl>()
    {
      AddComponent(MakeKey(typeof(TImpl)), typeof(TImpl));
    }

    public void AddServiceWithProvides<TImpl>()
    {
      AddServiceWithProvides(typeof(TImpl));
    }

    public void AddServiceWithProvides(Type type)
    {
      ProvidesServiceAttribute[] attributes = ((ProvidesServiceAttribute[])type.GetCustomAttributes(typeof (ProvidesServiceAttribute), true));
      if (attributes.Length == 0)
      {
        return;
      }
      Type serviceType = attributes[0].ServiceType;
      if (!serviceType.IsAssignableFrom(type))
      {
        throw new InvalidCastException(String.Format("Can't cast {0} to {1}", type, serviceType));
      }
      AddComponent(MakeKey(type), serviceType, type);
    }

    public void AddServicesWithProvides(Assembly assembly)
    {
      foreach (Type type in assembly.GetTypes())
      {
        AddServiceWithProvides(type);
      }
    }

    public void RemoveService<TService>()
    {
      Kernel.RemoveComponent(MakeKey(typeof(TService)));
    }

    public bool HasService<TService>()
    {
      return Kernel.HasComponent(MakeKey(typeof(TService)));
    }

    private string MakeKey(Type implementation)
    {
      return implementation.FullName + (_counter++);
    }

    public virtual void AddController(Type type)
    {
      AddComponent(MakeControllerKey(type), type);
    }

    public virtual string MakeControllerKey(Type type)
    {
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < type.Name.Length; i++)
      {
        char c = type.Name[i];
        if (i != 0 && Char.IsUpper(c))
        {
          sb.Append('.');
        }
        sb.Append(Char.ToLower(c));
      }
      return sb.ToString();
    }
  }
}
