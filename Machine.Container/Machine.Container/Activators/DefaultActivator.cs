using System;
using System.Collections.Generic;

using Machine.Container.Model;
using Machine.Container.Services;
using Machine.Container.Services.Impl;
using Machine.Container.Utility;

namespace Machine.Container.Activators
{
  public class DefaultActivator : IActivator
  {
    #region Member Data
    private readonly IObjectFactory _objectFactory;
    private readonly IServiceDependencyInspector _serviceDependencyInspector;
    private readonly IServiceEntryResolver _serviceEntryResolver;
    private readonly ServiceEntry _entry;
    #endregion

    #region DefaultActivator()
    public DefaultActivator(IObjectFactory objectFactory, IServiceDependencyInspector serviceDependencyInspector, IServiceEntryResolver serviceEntryResolver, ServiceEntry entry)
    {
      _objectFactory = objectFactory;
      _serviceEntryResolver = serviceEntryResolver;
      _serviceDependencyInspector = serviceDependencyInspector;
      _entry = entry;
    }
    #endregion

    #region IActivator Members
    public bool CanActivate(ICreationServices services)
    {
      if (services.Progress.Contains(_entry))
      {
        throw new CircularDependencyException(new ResolutionMessageBuilder(_entry, services.Progress).ToString());
      }
      using (StackPopper<ServiceEntry>.Push(services.Progress, _entry))
      {
        if (_entry.ConcreteType == null || _entry.ConcreteType.IsPrimitive)
        {
          return false;
        }
        ConstructorCandidate candidate = _serviceDependencyInspector.SelectConstructor(_entry.ConcreteType);
        foreach (ServiceDependency dependency in candidate.Dependencies)
        {
          ServiceEntry dependencyEntry = _serviceEntryResolver.ResolveEntry(services, dependency.DependencyType);
          if (dependencyEntry == null)
          {
            _entry.Dependencies.Clear();
            return false;
          }
          _entry.Dependencies.Add(dependencyEntry);
        }
        _entry.ConstructorCandidate = candidate;
        _entry.AreDependenciesResolved = true;
        return true;
      }
    }

    public object Create(ICreationServices services)
    {
      if (_entry.ConstructorCandidate == null)
      {
        throw new InvalidOperationException("How did you do this?");
      }
      List<object> parameters = new List<object>();
      foreach (ServiceEntry dependency in _entry.Dependencies)
      {
        parameters.Add(dependency.Activator.Create(services));
      }
      return _objectFactory.CreateObject(_entry.ConstructorCandidate, parameters.ToArray());
    }
    #endregion
  }
}