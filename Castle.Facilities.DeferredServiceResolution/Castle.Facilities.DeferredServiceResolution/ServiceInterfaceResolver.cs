using System;
using System.Collections.Generic;
using System.Text;

using Castle.Core;

namespace Castle.Facilities.DeferredServiceResolution
{
  public class ServiceInterfaceResolver : IServiceInterfaceResolver
  {
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(ServiceInterfaceResolver));

    private readonly Dictionary<Type, ComponentModel> _cache = new Dictionary<Type, ComponentModel>();

    public ComponentModel Resolve(Type serviceType, ICollection<ComponentModel> models, bool throwOnError)
    {
      List<ComponentModel> candidates = new List<ComponentModel>();
      foreach (ComponentModel model in models)
      {
        if (IsImplementationOf(serviceType, model.Implementation))
        {
          candidates.Add(model);
        }
      }
      if (candidates.Count == 0)
      {
        if (!throwOnError) return null;
        ThrowZeroCandidates(serviceType);
      }
      if (candidates.Count > 1)
      {
        StringBuilder sb = new StringBuilder();
        foreach (ComponentModel candidate in candidates) sb.AppendFormat(" {0}", candidate.Name);
        _log.InfoFormat("Resolving {0} yielded {1} candidates ({2})", serviceType, candidates.Count, sb);
        if (!throwOnError) return null;
        ThrowTooManyCandidates(serviceType, candidates);
      }
      return candidates[0];
    }

    public ComponentModel AttemptResolve(Type serviceType, ICollection<ComponentModel> models)
    {
      if (_cache.ContainsKey(serviceType))
      {
        return _cache[serviceType];
      }
      ComponentModel model = Resolve(serviceType, models, false);
      if (model != null)
      {
        _cache[serviceType] = model;
      }
      return model;
    }

    public ComponentModel Resolve(Type serviceType, ICollection<ComponentModel> models)
    {
      if (_cache.ContainsKey(serviceType))
      {
        return _cache[serviceType];
      }
      ComponentModel model = Resolve(serviceType, models, true);
      if (model != null)
      {
        _cache[serviceType] = model;
      }
      return model;
    }

    private static bool IsImplementationOf(Type serviceType, Type implementation)
    {
      return serviceType.IsAssignableFrom(implementation);
    }

    private static void ThrowZeroCandidates(Type serviceType)
    {
      throw new ServiceResolutionException(String.Format("Can't resolve {0}, no candidates were found!", serviceType));
    }

    private static void ThrowTooManyCandidates(Type serviceType, IEnumerable<ComponentModel> candidates)
    {
      StringBuilder sb = new StringBuilder();
      foreach (ComponentModel candidate in candidates)
      {
        sb.AppendFormat(" {0}", candidate.Name);
      }
      throw new ServiceResolutionException(String.Format("Can't resolve {0}, too many candidates are found:\n{1}", serviceType, sb));
    }
  }
}