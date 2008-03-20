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
    #region Logging
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(DefaultActivator));
    #endregion

    #region Member Data
    private readonly IObjectFactory _objectFactory;
    private readonly IServiceDependencyInspector _serviceDependencyInspector;
    private readonly IServiceEntryResolver _serviceEntryResolver;
    private readonly ServiceEntry _entry;
    private readonly List<ResolvedServiceEntry> _resolvedDependencies = new List<ResolvedServiceEntry>();
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
        throw new CircularDependencyException(ResolutionMessageBuilder.BuildMessage(_entry, services.Progress));
      }
      using (StackPopper<ServiceEntry>.Push(services.Progress, _entry))
      {
        if (_entry.ConcreteType == null || _entry.ConcreteType.IsPrimitive)
        {
          return false;
        }
        _resolvedDependencies.Clear();
        ConstructorCandidate candidate = _serviceDependencyInspector.SelectConstructor(_entry.ConcreteType);
        foreach (ServiceDependency dependency in candidate.Dependencies)
        {
          ResolvedServiceEntry dependencyEntry = _serviceEntryResolver.ResolveEntry(services, dependency.DependencyType);
          if (dependencyEntry == null)
          {
            return false;
          }
          _log.Info("Dependency: " + dependencyEntry);
          _resolvedDependencies.Add(dependencyEntry);
        }
        _entry.ConstructorCandidate = candidate;
        return true;
      }
    }

    public object Activate(ICreationServices services)
    {
      if (_entry.ConstructorCandidate == null)
      {
        throw new YouFoundABugException();
      }
      List<object> parameters = new List<object>();
      foreach (ResolvedServiceEntry dependency in _resolvedDependencies)
      {
        parameters.Add(dependency.Activator.Activate(services));
      }
      return _objectFactory.CreateObject(_entry.ConstructorCandidate, parameters.ToArray());
    }
    #endregion
  }
}