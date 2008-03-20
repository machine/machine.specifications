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
    private readonly IActivatorStrategy _activatorStrategy;
    private readonly IActivatorStore _activatorStore;
    private readonly IOverrideLookup _overrideLookup;
    #endregion

    #region CreationServices()
    public CreationServices(IActivatorStrategy activatorStrategy, IActivatorStore activatorStore, IOverrideLookup overrideLookup)
    {
      _activatorStrategy = activatorStrategy;
      _activatorStore = activatorStore;
      _overrideLookup = overrideLookup;
    }
    #endregion

    #region ICreationServices Members
    public IActivatorStrategy ActivatorStrategy
    {
      get { return _activatorStrategy; }
    }

    public IActivatorStore ActivatorStore
    {
      get { return _activatorStore; }
    }

    public Stack<ServiceEntry> Progress
    {
      get { return _progress; }
    }

    public IOverrideLookup Overrides
    {
      get { return _overrideLookup; }
    }
    #endregion
  }
}
