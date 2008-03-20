using System;
using System.Collections.Generic;

using Machine.Container.Model;

namespace Machine.Container.Services.Impl
{
  [CoverageExclude]
  public class CreationServices : ICreationServices
  {
    #region Member Data
    private readonly Stack<ServiceEntry> _progress = new Stack<ServiceEntry>();
    private readonly IActivatorStore _activatorStore;
    private readonly ILifestyleStore _lifestyleStore;
    private readonly IActivatorStrategy _activatorStrategy;
    private readonly IOverrideLookup _overrideLookup;
    #endregion

    #region CreationServices()
    public CreationServices(IActivatorStore activatorStore, ILifestyleStore lifestyleStore, IActivatorStrategy activatorStrategy, IOverrideLookup overrideLookup)
    {
      _activatorStore = activatorStore;
      _lifestyleStore = lifestyleStore;
      _activatorStrategy = activatorStrategy;
      _overrideLookup = overrideLookup;
    }
    #endregion

    #region ICreationServices Members
    public Stack<ServiceEntry> Progress
    {
      get { return _progress; }
    }

    public IActivatorStore ActivatorStore
    {
      get { return _activatorStore; }
    }

    public ILifestyleStore LifestyleStore
    {
      get { return _lifestyleStore; }
    }

    public IActivatorStrategy ActivatorStrategy
    {
      get { return _activatorStrategy; }
    }

    public IOverrideLookup Overrides
    {
      get { return _overrideLookup; }
    }
    #endregion
  }
}
