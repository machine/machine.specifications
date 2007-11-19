using System;
using System.Collections.Generic;
using System.Reflection;

using Castle.Core;
using Castle.MicroKernel;

namespace Castle.Facilities.DeferredServiceResolution
{
  public class AttributeBasedRegisterer
  {
    #region Logging
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(AttributeBasedRegisterer));
    #endregion

    public void RegisterAssembly(IKernel kernel, Assembly assembly)
    {
      foreach (Type type in assembly.GetTypes())
      {
        if (HasAttribute(type))
        {
          RegisterType(kernel, type);
        }
      }
    }

    public void RegisterType(IKernel kernel, Type type)
    {
      if (type.IsAbstract || type.IsInterface)
      {
        throw new InvalidOperationException("Can't register abstract type: " + type);
      }
      if (!kernel.HasComponent(type) && !IsManualRegistered(type))
      {
        WindsorServiceAttribute details = GetWindsorServiceAttribute(type);
        string key = CreateKey(type);
        if (details != null && !String.IsNullOrEmpty(details.Name))
        {
          key = details.Name;
        }
        if (details != null && details.ServiceType != null)
        {
          _log.DebugFormat("Registering {0} as {1} providing {2}", type, key, details.ServiceType);
          kernel.AddComponent(key, details.ServiceType, type);
        }
        else
        {
          _log.DebugFormat("Registering {0} as {1}", type, key);
          kernel.AddComponent(key, type);
        }
      }
    }

    private static bool IsManualRegistered(Type type)
    {
      return AttributeHelper.GetSingleUseAttribute<ManualWindsorRegistrationAttribute>(type) != null;
    }

    private static bool HasAttribute(Type type)
    {
      return AttributeHelper.GetSingleUseAttribute<LifestyleAttribute>(type) != null;
    }

    private static WindsorServiceAttribute GetWindsorServiceAttribute(Type type)
    {
      return AttributeHelper.GetSingleUseAttribute<WindsorServiceAttribute>(type);
    }
    
    private static string CreateKey(Type serviceType)
    {
      return serviceType.FullName;
    }
  }
  public static class AttributeHelper
  {
    public static T GetSingleUseAttribute<T>(Type type) where T : Attribute
    {
      T[] attributes = (T[]) type.GetCustomAttributes(typeof(T), true);
      if (attributes.Length == 0)
      {
        return null;
      }
      return attributes[0];
    }
  }
}
