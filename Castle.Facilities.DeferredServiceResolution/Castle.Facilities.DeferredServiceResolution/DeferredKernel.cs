using System;
using System.Collections.Generic;

using Castle.Core;
using Castle.MicroKernel;

namespace Castle.Facilities.DeferredServiceResolution
{
  public class DeferredKernel : DefaultKernel
  {
    private DeferredServiceResolutionFacility _facility;

    public DeferredServiceResolutionFacility DeferredFacility
    {
      get { return _facility; }
    }

    public override void AddFacility(string key, IFacility facility)
    {
      if (facility is DeferredServiceResolutionFacility)
      {
        _facility = facility as DeferredServiceResolutionFacility;
      }
      base.AddFacility(key, facility);
    }

    public override IHandler GetHandler(Type service)
    {
      IHandler handler = base.GetHandler(service);
      if (handler == null)
      {
        ComponentModel model = _facility.ServiceResolver.ResolveModel(service);
        if (model != null)
        {
          handler = GetHandler(model.Name);
        }
      }
      return handler;
    }

    public override IHandler[] GetHandlers(Type service)
    {
      List<IHandler> handlers = new List<IHandler>(base.GetHandlers(service));
      ComponentModel model = _facility.ServiceResolver.ResolveModel(service);
      if (model != null)
      {
        handlers.Add(GetHandler(model.Name));
      }
      return handlers.ToArray();
    }

    public override bool HasComponent(Type serviceType)
    {
      if (base.HasComponent(serviceType))
      {
        return true;
      }
      return _facility.ServiceResolver.CanResolve(serviceType);
    }
  }
}
