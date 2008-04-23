using System;
using System.Collections.Generic;
using System.Reflection;
using Machine.Container.Model;
using Machine.Core.Utility;

namespace Machine.Container
{
  public class ContainerRegistrationHelper
  {
    private readonly MachineContainer _container;

    public ContainerRegistrationHelper(MachineContainer container)
    {
      _container = container;
    }

    public virtual void AddAttributedServices(Assembly assembly)
    {
      foreach (Type type in assembly.GetTypes())
      {
        LifestyleAttribute lifestyleAttribute = ReflectionHelper.GetAttribute<LifestyleAttribute>(type, true);
        if (lifestyleAttribute != null)
        {
          switch (lifestyleAttribute.Lifestyle)
          {
            case LifestyleType.Transient:
              _container.AddService(type, Machine.Container.Model.LifestyleType.Transient);
              break;
            case LifestyleType.Singleton:
              _container.AddService(type, Machine.Container.Model.LifestyleType.Singleton);
              break;
            default:
              throw new NotSupportedException("Not supported lifestyle: " + lifestyleAttribute.Lifestyle);
          }
        }
      }
    }
  }

}
