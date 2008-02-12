using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Castle.Windsor;
using Castle.Windsor.Configuration;

namespace Machine.IoC
{
  public class MachineContainer : WindsorContainer
  {
    public MachineContainer(IConfigurationInterpreter interpreter)
     : base(interpreter)
    {
    }

    public MachineContainer()
    {
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
      AddComponent(MakeKey(type), attributes[0].ServiceType, type);
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

    private static string MakeKey(Type implementation)
    {
      return implementation.FullName;
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
